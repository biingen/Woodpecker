using jini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTest
{
    public partial class FormMonkeyTest : Form
    {
        public FormMonkeyTest()
        {
            InitializeComponent();
        }

        string MonkeyTestPath = Application.StartupPath + "\\Monkey_Test.ini";

        private void buttonRunAll_Click(object sender, EventArgs e)
        {
            Add_ons MonkeyTest_All = new Add_ons();
            MonkeyTest_All.MonkeyTest();
        }

        private void buttonLoadApps_Click(object sender, EventArgs e)
        {
            string ReadLine;
            string[] array;
            string Path = @"D:\apk.txt";

            this.comboxQcProName.Items.Clear();
            StreamReader reader = new StreamReader(Path, System.Text.Encoding.GetEncoding("GB2312"));
            while (reader.Peek() >= 0)
            {
                try
                {
                    ReadLine = reader.ReadLine();
                    if (ReadLine != "")
                    {
                        ReadLine = ReadLine.Replace("\"", "").Substring(8);
                        array = ReadLine.Split(',');
                        Console.WriteLine(array);
                        if (array.Length == 0)
                        {
                            MessageBox.Show("您选择的导入数据类型有误，请重试！");
                            return;
                        }
                        this.comboxQcProName.Items.Add(array[0]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                comboxQcProName.Text = "Select apk";
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            ini12.INIWrite(MonkeyTestPath, "Monket Test", "package", comboxQcProName.Text.Trim());
        }
    }
}
