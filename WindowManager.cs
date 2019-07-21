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
    public class WindowManager
    {
        public enum WindowManagerResult
        {
            Success,
            Failure,
            SaveFileMissing
        }

        public static WindowManagerResult RestoreWindows(string savedWindowsFileName)
        {
            try
            {
                string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string savedWindowsFile = Path.Combine(myDocumentsPath, savedWindowsFileName);

                if (!File.Exists(savedWindowsFile))
                    return WindowManagerResult.SaveFileMissing;

                List<WindowDetails> savedWindowsDetails;
                var serializer = new XmlSerializer(typeof(List<WindowDetails>));

                using (XmlReader reader = XmlReader.Create(savedWindowsFile))
                {
                    savedWindowsDetails = (List<WindowDetails>)serializer.Deserialize(reader);
                }

                EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
                {
                    StringBuilder titleBuilder = new StringBuilder(255);
                    int unused = GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity + 1);
                    string windowTitle = titleBuilder.ToString();

                    if (IsWindowVisible(hWnd) && !string.IsNullOrEmpty(windowTitle))
                    {
                        var snapshottedWindowDetails = savedWindowsDetails.FirstOrDefault(w => w.windowTitle == windowTitle);

                        // If window was maximised, restore it as non-maximised first otherwise it may not end up on the original screen
                        if (snapshottedWindowDetails.Windowplacement.showCmd == (int)showCmdFlags.SW_MAXIMIZE)
                        {
                            var tempWindowPlacement = snapshottedWindowDetails.Windowplacement;
                            tempWindowPlacement.showCmd = (int)showCmdFlags.SW_SHOWNORMAL;
                            SetWindowPlacement(hWnd, ref tempWindowPlacement);
                        }

                        SetWindowPlacement(hWnd, ref snapshottedWindowDetails.Windowplacement);
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

        public static WindowManagerResult SnapshotWindows(string savedWindowsFileName)
        {
            try
            {
                var windowDetailsList = new List<WindowDetails>();
                EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
                {
                    StringBuilder titleBuilder = new StringBuilder(255);
                    int unused = GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity + 1);
                    string windowTitle = titleBuilder.ToString();

                    if (IsWindowVisible(hWnd) && !string.IsNullOrEmpty(windowTitle))
                    {
                        WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                        placement.length = System.Runtime.InteropServices.Marshal.SizeOf(placement);
                        GetWindowPlacement(hWnd, ref placement);
                        windowDetailsList.Add(new WindowDetails
                        {
                            windowTitle = windowTitle,
                            Windowplacement = placement
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
    }
}
