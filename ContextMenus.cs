using System;
using System.Windows.Forms;
using WindowSnapshotter.Properties;
using System.IO;

namespace WindowSnapshotter
{
	/// <summary>
	/// 
	/// </summary>
	class ContextMenus
	{
        const string SavedWindowsFileName = "WindowDetails.xml";

        /// <summary>
        /// Is the About box displayed?
        /// </summary>
        bool _isAboutLoaded = false;

        NotifyIcon _notifyIcon;

		/// <summary>
		/// Creates this instance.
		/// </summary>
		/// <returns>ContextMenuStrip</returns>
		public ContextMenuStrip Create(NotifyIcon notifyIcon)
		{
            _notifyIcon = notifyIcon;

			// Add the default menu options.
			ContextMenuStrip menu = new ContextMenuStrip();
			ToolStripMenuItem item;
			ToolStripSeparator sep;

			// Restore saved window positions.
			item = new ToolStripMenuItem();
			item.Text = "Restore";
            item.ToolTipText = "Restore window positions from saved file";
			item.Click += new EventHandler(Restore_Click);
			item.Image = Resources.Open_32x;
            //item.Enabled = SaveFileExists();
			menu.Items.Add(item);

            // Save window positions.
            item = new ToolStripMenuItem();
            item.Text = "Save";
            item.ToolTipText = "Save window positions to file";
            item.Click += new EventHandler(Save_Click);
            item.Image = Resources.Save_32x;
            menu.Items.Add(item);

            // About.
            item = new ToolStripMenuItem();
			item.Text = "About";
			item.Click += new EventHandler(About_Click);
			item.Image = Resources.StatusInformation_32x_exp;
			menu.Items.Add(item);

			// Separator.
			sep = new ToolStripSeparator();
			menu.Items.Add(sep);

			// Exit.
			item = new ToolStripMenuItem();
			item.Text = "Exit";
			item.Click += new System.EventHandler(Exit_Click);
			//item.Image = Resources.Exit;
			menu.Items.Add(item);

			return menu;
		}

        private bool SaveFileExists()
        {
            return File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SavedWindowsFileName));
        }

        /// <summary>
        /// Handles the Click event of the Save control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Save_Click(object sender, EventArgs e)
        {
            switch(WindowManager.SnapshotWindows(SavedWindowsFileName))
            {
                case WindowManager.WindowManagerResult.Success:
                    ShowMessage("Save successful", "Window positions saved.");
                    break;
                default:
                    ShowMessage("Save error", "There was an error saving the window poistiions.", true);
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the Restore control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Restore_Click(object sender, EventArgs e)
        {
            switch (WindowManager.RestoreWindows(SavedWindowsFileName))
            {
                case WindowManager.WindowManagerResult.Success:
                    ShowMessage("Restore successful", "Window positions restored.");
                    break;
                case WindowManager.WindowManagerResult.SaveFileMissing:
                    ShowMessage("Windows not restored", "A saved window positions file could not be found. Please save window positions before attempting to restore them.");
                    break;
                default:
                    ShowMessage("Restore error", "There was an error restoring the window poistiions.", true);
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the About control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void About_Click(object sender, EventArgs e)
		{
			if (!_isAboutLoaded)
			{
				_isAboutLoaded = true;
				new AboutBox().ShowDialog();
				_isAboutLoaded = false;
			}
		}

		/// <summary>
		/// Processes a menu item.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Exit_Click(object sender, EventArgs e)
		{
			// Quit without further ado.
			Application.Exit();
		}

        private void ShowMessage(string message)
        {
            ShowMessage(string.Empty, message);
        }

        private void ShowMessage(string title, string message)
        {
            ShowMessage(title, message, false);
        }

        private void ShowMessage(string message, bool isError)
        {
            ShowMessage(string.Empty, message, isError);
        }

        private void ShowMessage(string title, string message, bool isError)
        {
            _notifyIcon.BalloonTipTitle = title;
            _notifyIcon.BalloonTipText = message;
            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(0);
        }
	}
}