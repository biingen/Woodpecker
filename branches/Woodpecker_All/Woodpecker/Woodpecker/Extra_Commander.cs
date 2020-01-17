using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using KWP_2000;

namespace Woodpecker
{
    public partial class Extra_Commander : Form
    {
        private Autokit_Command Autokit_Command_1 = new Autokit_Command();

        public Extra_Commander()
        {
            InitializeComponent();
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            Autokit_Command_1.Autokit_Commander(textBox_command.Text);
        }
    }
}
