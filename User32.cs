using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowSnapshotter
{
    public class User32
    {
        /// <summary>
        /// filter function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        /// <summary>
        /// check if windows visible
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// return windows text
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpWindowText"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowText",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        /// <summary>
        /// enumarator on all desktop windows
        /// </summary>
        /// <param name="hDesktop"></param>
        /// <param name="lpEnumCallbackFunction"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        public struct WINDOWPLACEMENT
        {
            public int length;                                  // The length of the structure, in bytes
            public int flags;                                   // Specifies flags that control the position of the minimized window and the method by which the window is restored
            public int showCmd;                                 // The current show state of the window
            public System.Drawing.Point ptMinPosition;          // The coordinates of the window's upper-left corner when the window is minimized
            public System.Drawing.Point ptMaxPosition;          // The coordinates of the window's upper-left corner when the window is maximized
            public System.Drawing.Rectangle rcNormalPosition;   // The window's coordinates when the window is in the restored position
        }

        public struct WindowDetails
        {
            public string WindowTitle;
            public Int32 WindowHandle32;
            public Int64 WindowHandle64;
            public WINDOWPLACEMENT Windowplacement;
        }

        public enum showCmdFlags
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            SW_HIDE = 0,

            /// <summary>
            /// Activates and displays a window. If the window is minimized or maximized, 
            /// the system restores it to its original size and position. An application 
            /// should specify this flag when displaying the window for the first time.
            /// </summary>
            SW_SHOWNORMAL = 1,

            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            SW_SHOWMINIMIZED = 2,

            /// <summary>
            /// 
            /// </summary>
            SW_MAXIMIZE = 3,

            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>
            SW_SHOWMAXIMIZED = 3,

            /// <summary>
            /// Displays a window in its most recent size and position.
            /// This value is similar to SW_SHOWNORMAL, except the window is not activated.
            /// </summary>
            SW_SHOWNOACTIVATE = 4,

            /// <summary>
            /// Activates the window and displays it in its current size and position.
            /// </summary>
            SW_SHOW = 5,

            /// <summary>
            /// Minimizes the specified window and activates the next top-level window in the z-order.
            /// </summary>
            SW_MINIMIZE = 6,

            /// <summary>
            /// Displays the window as a minimized window.
            /// This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
            /// </summary>
            SW_SHOWMINNOACTIVE = 7,

            /// <summary>
            /// Displays the window in its current size and position.
            /// This value is similar to SW_SHOW, except the window is not activated.
            /// </summary>
            SW_SHOWNA = 8,

            /// <summary>
            /// Activates and displays the window. If the window is minimized or maximized, 
            /// the system restores it to its original size and position. An application 
            /// should specify this flag when restoring a minimized window.
            /// </summary>
            SW_RESTORE = 9
        }
    }
}
