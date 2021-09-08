using System;
using System.Windows.Forms;

namespace WindowSnapshotter
{
    using System.IO;

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
            string savedWindowsFile;
            if (args.Length > 1)
            {
                savedWindowsFile = args[1];
            }
            else
            {
                savedWindowsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "WindowDetails.xml");
            }

            switch (args[0].ToLower())
            {
                case "s":
                case "save":
                    WindowManager.SnapshotWindows(savedWindowsFile);
                    break;
                case "r":
                case "restore":
                    WindowManager.RestoreWindows(savedWindowsFile);
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
                Application.Run(new HotKeyReceiverForm());
            }
        }
    }
}