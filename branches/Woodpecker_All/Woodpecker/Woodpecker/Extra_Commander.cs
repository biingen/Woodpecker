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

        public Extra_Commander(string value)
        {
            InitializeComponent();
        }

        private void Extra_Commander_Load(object sender, EventArgs e)
        {
            Add_ons Add_ons = new Add_ons();
            Add_ons.CreateConfig();//如果根目錄沒有Config.ini則創建//
            Add_ons.USB_Read();//讀取USB設備的Pid, Vid//
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            Autokit_Command_1.Autokit_Commander(textBox_command.Text, 5);
        }

        private void button_schedule_Click(object sender, EventArgs e)
        {
            SchOpen.Filter = "CSV files (*.csv)|*.CSV";
            SchOpen.ShowDialog();
            if (SchOpen.FileName == "Schedule")
                textBox_command.Text = textBox_command.Text;
            else
                textBox_command.Text = SchOpen.FileName;
        }

        private void button_setting_Click(object sender, EventArgs e)
        {
            FormTabControl FormTabControl = new FormTabControl();

            //關閉SETTING以後會讀這段>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            if (FormTabControl.ShowDialog() == DialogResult.OK)
            {

            }

            FormTabControl.Dispose();
        }
    }
}
