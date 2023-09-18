using System.Windows.Forms;

namespace WindowSnapshotter
{
    using System;
    using System.Collections.Generic;
    using Properties;

    public partial class OutputBox : Form
    {
        public OutputBox(IEnumerable<string> outputContents)
        {
            InitializeComponent();
            this.Icon = Resources.Window_Refresh_01;
            this.outputContent.Text = string.Join(Environment.NewLine, outputContents);
        }
    }
}
