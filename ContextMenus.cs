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
        private const string DefaultSaveFileName = "WindowDetails.xml";

        private static readonly string DefaultSaveFile =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DefaultSaveFileName);

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
			var menu = new ContextMenuStrip();

            // Restore saved window positions.
            var item = new ToolStripMenuItem {Text = "Restore", ToolTipText = "Restore window positions from saved file"};
            item.Click += Restore_Click;
			item.Image = Resources.Open_32x;
			menu.Items.Add(item);

            // Save window positions.
            item = new ToolStripMenuItem {Text = "Save", ToolTipText = "Save window positions to file"};
            item.Click += Save_Click;
            item.Image = Resources.Save_32x;
            menu.Items.Add(item);

			// Separator.
			menu.Items.Add(new ToolStripSeparator());

            // Restore saved window positions.
            item = new ToolStripMenuItem { Text = "Restore from ...", ToolTipText = "Restore window positions from saved file" };
            item.Click += RestoreFrom_Click;
            item.Image = Resources.Open_32x;
            menu.Items.Add(item);

            // Save window positions.
            item = new ToolStripMenuItem {Text = "Save to ...", ToolTipText = "Save window positions to file"};
            item.Click += SaveTo_Click;
            item.Image = Resources.Save_32x;
            menu.Items.Add(item);

			// Separator.
			menu.Items.Add(new ToolStripSeparator());

            // About.
            item = new ToolStripMenuItem {Text = "About"};
            item.Click += About_Click;
			item.Image = Resources.StatusInformation_32x_exp;
			menu.Items.Add(item);

			// Separator.
			menu.Items.Add(new ToolStripSeparator());

			// Exit.
            item = new ToolStripMenuItem {Text = "Exit"};
            item.Click += Exit_Click;
			menu.Items.Add(item);

			return menu;
		}

        /// <summary>
        /// Handles the Click event of the Save control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Save_Click(object sender, EventArgs e)
        {
            switch(WindowManager.SnapshotWindows(DefaultSaveFile))
            {
                case WindowManager.WindowManagerResult.Success:
                    ShowMessage("Save successful", "Window positions saved.");
                    break;
                default:
                    ShowMessage("Save error", "There was an error saving the window positions.", true);
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the Save To control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void SaveTo_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = DefaultSaveFileName
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            switch (WindowManager.SnapshotWindows(saveFileDialog.FileName))
            {
                case WindowManager.WindowManagerResult.Success:
                    ShowMessage("Save successful", "Window positions saved.");
                    break;
                default:
                    ShowMessage("Save error", "There was an error saving the window positions.", true);
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
            switch (WindowManager.RestoreWindows(DefaultSaveFile))
            {
                case WindowManager.WindowManagerResult.Success:
                    ShowMessage("Restore successful", "Window positions restored.");
                    break;
                case WindowManager.WindowManagerResult.SaveFileMissing:
                    ShowMessage("Windows not restored", "A saved window positions file could not be found. Please save window positions before attempting to restore them.");
                    break;
                default:
                    ShowMessage("Restore error", "There was an error restoring the window positions.", true);
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the Restore From control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void RestoreFrom_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = DefaultSaveFileName
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            switch (WindowManager.RestoreWindows(openFileDialog.FileName))
            {
                case WindowManager.WindowManagerResult.Success:
                    ShowMessage("Restore successful", "Window positions restored.");
                    break;
                case WindowManager.WindowManagerResult.SaveFileMissing:
                    ShowMessage("Windows not restored", "A saved window positions file could not be found. Please save window positions before attempting to restore them.");
                    break;
                default:
                    ShowMessage("Restore error", "There was an error restoring the window positions.", true);
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