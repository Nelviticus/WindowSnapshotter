using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using static WindowSnapshotter.User32;

namespace WindowSnapshotter
{
    using System.Threading.Tasks;

    public class WindowManager
    {
        public enum WindowManagerResult
        {
            Success,
            Failure,
            SaveFileMissing
        }

        public static WindowManagerResult RestoreWindows(string savedWindowsFile)
        {
            try
            {
                if (!File.Exists(savedWindowsFile))
                    return WindowManagerResult.SaveFileMissing;

                List<WindowDetails> savedWindows;
                var serializer = new XmlSerializer(typeof(List<WindowDetails>));

                using (XmlReader reader = XmlReader.Create(savedWindowsFile))
                {
                    savedWindows = (List<WindowDetails>)serializer.Deserialize(reader);
                }

                EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
                {
                    if (IsWindowVisible(hWnd) && !string.IsNullOrEmpty(WindowTitle(hWnd)))
                    {
                        var savedWindow = GetSavedWindow(savedWindows, hWnd);

                        // If window was maximised, restore it as non-maximised first otherwise it may not end up on the original screen
                        if (savedWindow.WindowPlacement.showCmd == (int)showCmdFlags.SW_MAXIMIZE)
                        {
                            var tempWindowPlacement = savedWindow.WindowPlacement;
                            tempWindowPlacement.showCmd = (int)showCmdFlags.SW_SHOWNORMAL;
                            SetWindowPlacement(hWnd, ref tempWindowPlacement);
                        }

                        var setWindowPlacemenTask = Task.Run(() =>
                        {
                            SetWindowPlacement(hWnd, ref savedWindow.WindowPlacement);
                        });

                        if (!setWindowPlacemenTask.Wait(TimeSpan.FromMilliseconds(500)))
                        {
                            return false;
                        }
                    }
                    return true;
                };

                EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero);
            }
            catch(Exception)
            {
                return WindowManagerResult.Failure;
            }

            return WindowManagerResult.Success;
        }

        private static string WindowTitle(IntPtr hWnd)
        {
            StringBuilder titleBuilder = new StringBuilder(255);
            int unused = GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity + 1);
            string windowTitle = titleBuilder.ToString();
            return windowTitle;
        }

        private static WindowDetails GetSavedWindow(List<WindowDetails> savedWindows, IntPtr hWnd)
        {
            var matchingWindowsByHandle = savedWindows.FindAll(w => w.WindowHandle == (long) hWnd);
            return matchingWindowsByHandle.Count != 1
                ? savedWindows.FirstOrDefault(w => w.WindowTitle == WindowTitle(hWnd))
                : matchingWindowsByHandle.FirstOrDefault();
        }

        public static WindowManagerResult SnapshotWindows(string savedWindowsFileName)
        {
            try
            {
                var windowDetailsList = new List<WindowDetails>();
                EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
                {
                    string windowTitle = WindowTitle(hWnd);

                    if (IsWindowVisible(hWnd) && !string.IsNullOrEmpty(windowTitle))
                    {
                        WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                        placement.length = System.Runtime.InteropServices.Marshal.SizeOf(placement);
                        GetWindowPlacement(hWnd, ref placement);
                        windowDetailsList.Add(new WindowDetails
                        {
                            WindowTitle = windowTitle,
                            WindowHandle = (long)hWnd,
                            WindowPlacement = placement
                        });
                    }
                    return true;
                };

                if (EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(windowDetailsList.GetType());

                    string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string outputFile = Path.Combine(myDocumentsPath, savedWindowsFileName);
                    using (XmlWriter writer = new XmlTextWriter(outputFile, Encoding.Default))
                    {
                        xmlSerializer.Serialize(writer, windowDetailsList);
                    }
                }
            }
            catch(Exception)
            {
                return WindowManagerResult.Failure;
            }

            return WindowManagerResult.Success;
        }

        public static IEnumerable<string> ListSavedWindows(string savedWindowsFile)
        {
            var windowList = new List<string>();

            if (!File.Exists(savedWindowsFile))
                return null;

            try
            {
                List<WindowDetails> savedWindows;
                var serializer = new XmlSerializer(typeof(List<WindowDetails>));

                using (var reader = XmlReader.Create(savedWindowsFile))
                {
                    savedWindows = (List<WindowDetails>)serializer.Deserialize(reader);
                }

                windowList.AddRange(savedWindows.Select(savedWindow => savedWindow.WindowTitle));
            }
            catch (Exception)
            {
                return null;
            }

            return windowList;
        }
    }
}
