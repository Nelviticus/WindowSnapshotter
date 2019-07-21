using System;
using System.Windows.Forms;

namespace WindowSnapshotter
{
	/// <summary>
	/// 
	/// </summary>
	static class Program
	{
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main(string[] args)
		{
            if (args.Length > 0)
            {
                RunAsConsole(args);
            }
            else
            {
                RunAsForms();
            }
		}

        private static void RunAsConsole(string[] args)
        {
            string savedWindowsFileName = "WindowDetails.xml";

            switch (args[0].ToLower())
            {
                case "s":
                case "save":
                    WindowManager.SnapshotWindows(savedWindowsFileName);
                    break;
                case "r":
                case "restore":
                    WindowManager.RestoreWindows(savedWindowsFileName);
                    break;
            }
        }

        private static void RunAsForms()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show the system tray icon.					
            using (ProcessIcon pi = new ProcessIcon())
            {
                pi.Display();

                // Make sure the application runs!
                Application.Run();
            }
        }
    }
}