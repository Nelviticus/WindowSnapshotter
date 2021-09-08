namespace WindowSnapshotter
{
    using System;
    using System.Windows.Forms;

    public partial class HotKeyReceiverForm : Form
    {
        private int _saveHotKeyId;
        private int _restoreHotKeyId;

        public HotKeyReceiverForm()
        {
            InitializeComponent();
            RegisterHotKeys();
            this.Load += new EventHandler(Form_Load);
            this.FormClosing += new FormClosingEventHandler(Form_Closing);
        }

        private void Form_Load(object sender, EventArgs e)
        {
        }

        private void RegisterHotKeys()
        {
            _saveHotKeyId = HotKeyManager.RegisterHotKey(Properties.Settings.Default.HotKeySave,
                KeyModifiers.Alt & KeyModifiers.Control & KeyModifiers.Shift);
            _restoreHotKeyId = HotKeyManager.RegisterHotKey(Properties.Settings.Default.HotKeyRestore,
                KeyModifiers.Alt & KeyModifiers.Control & KeyModifiers.Shift);

            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyEventHandler);
        }

        private void HotKeyEventHandler(object sender, HotKeyEventArgs e)
        {
            if (e.Key == Properties.Settings.Default.HotKeySave)
                MessageBox.Show($"Save {_saveHotKeyId}");
            else if (e.Key == Properties.Settings.Default.HotKeyRestore)
                MessageBox.Show($"Restore {_restoreHotKeyId}");
            else
                MessageBox.Show("WTF?");
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
        }

        private void UnregisterHotKeys()
        {
            HotKeyManager.UnregisterHotKey(_saveHotKeyId);
            HotKeyManager.UnregisterHotKey(_restoreHotKeyId);
            Properties.Settings.Default.Save();
        }

        protected override void SetVisibleCore(bool value)
        {
            // Quick and dirty to keep the main window invisible      
            base.SetVisibleCore(false);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            //UnregisterHotKeys();

            base.Dispose(disposing);
        }
    }
}