
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Woodpecker
{
    public partial class FormScriptHelper : Form
    {
        public FormScriptHelper()
        {
            InitializeComponent();
        }

        private string RCKey;
        public string RCKeyForm1
        {
            set
            {
                RCKey = value;
            }
        }

        Button[] Buttons;
        public void SetValue(string HeaderText)
        {
            List<string> CmdList = new List<string> { };

            switch (RCKey + " + " + HeaderText)
            {
                case "_ascii + >COM  >Pin":
                    CmdList.Add("A");
                    CmdList.Add("B");
                    CmdList.Add("C");
                    CmdList.Add("D");
                    CmdList.Add("E");
                    break;

                case "_cmd + AC/USB Switch":
                    CmdList.Add("_off");
                    CmdList.Add("_on");
                    CmdList.Add("_AC1_ON");
                    CmdList.Add("_AC1_OFF");
                    CmdList.Add("_AC2_ON");
                    CmdList.Add("_AC2_OFF");
                    CmdList.Add("_USB1_DUT");
                    CmdList.Add("_USB1_PC");
                    CmdList.Add("_USB2_DUT");
                    CmdList.Add("_USB2_PC");
                    break;

                case "_ascii + >SerialPort                   >I/O cmd":
                    CmdList.Add("_save");
                    CmdList.Add("_clear");
                    break;

                case "_ascii + AC/USB Switch":
                    CmdList.Add(@"\n");
                    CmdList.Add(@"\r");
                    CmdList.Add(@"\n\r");
                    CmdList.Add(@"\r\n");
                    break;

                case "_HEX + >COM  >Pin":
                    CmdList.Add("A");
                    CmdList.Add("B");
                    CmdList.Add("C");
                    CmdList.Add("D");
                    CmdList.Add("E");
                    CmdList.Add("Canbus");
                    break;

                case "_HEX + >SerialPort                   >I/O cmd":
                    CmdList.Add("_save");
                    CmdList.Add("_clear");
                    break;

                case "_Pin + >COM  >Pin":
                    CmdList.Add("_PA10_0");
                    CmdList.Add("_PA10_1");
                    CmdList.Add("_PA11_0");
                    CmdList.Add("_PA11_1");
                    CmdList.Add("_PA14_0");
                    CmdList.Add("_PA14_1");
                    CmdList.Add("_PA15_0");
                    CmdList.Add("_PA15_1");
                    CmdList.Add("_PB01_0");
                    CmdList.Add("_PB01_1");
                    CmdList.Add("_PB07_0");
                    CmdList.Add("_PB07_1");
                    break;

                case "_Pin + >SerialPort                   >I/O cmd":
                    CmdList.Add("_pause");
                    CmdList.Add("_stop");
                    CmdList.Add("_ac_restart");
                    CmdList.Add("_shot");
                    CmdList.Add("_accumulate");
                    CmdList.Add("_mail");
                    CmdList.Add("_rc_");
                    CmdList.Add("_logcmd");
                    break;

                case "_keyword + >Times >Keyword#":
                    CmdList.Add("1");
                    CmdList.Add("2");
                    CmdList.Add("3");
                    CmdList.Add("4");
                    CmdList.Add("5");
                    CmdList.Add("6");
                    CmdList.Add("7");
                    CmdList.Add("8");
                    CmdList.Add("9");
                    CmdList.Add("10");
                    break;

                case "_keyword + >SerialPort                   >I/O cmd":
                    CmdList.Add("_pause");
                    CmdList.Add("_stop");
                    CmdList.Add("_ac_restart");
                    CmdList.Add("_shot");
                    CmdList.Add("_mail");
                    CmdList.Add("_savelog1");
                    CmdList.Add("_savelog2");
                    CmdList.Add("_rc_");
                    CmdList.Add("_logcmd");
                    break;

                default:
                    CmdList.Add("");
                    break;
            }

            Graphics graphics = CreateGraphics();
            float dpiX = graphics.DpiX;
            float dpiY = graphics.DpiY;
            int width, height;
            if (dpiX == 120 && dpiY == 120)
            {
                this.Width = 620;
                this.Height = 90;
                width = 100;
                height = 30;
            }
            else
            {
                this.Width = 415;
                this.Height = 75;
                width = 75;
                height = 25;
            }

            Buttons = new Button[CmdList.Count];

            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i] = new Button
                {
                    Size = new Size(width, height),
                    Text = CmdList[i],
                    AutoSize = false,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    FlatStyle = FlatStyle.System
                };

                if (i <= 5)
                {
                    Buttons[i].Location = new Point(10 + (i * width), 10);
                }
                else if (i > 5 && i <= 11)
                {
                    Buttons[i].Location = new Point(10 + ((i - 6) * width), 50);
                }

                int index = i;
                Buttons[i].Click += (sender1, ex) => Sand_Key(index + 1);
                this.Controls.Add(Buttons[i]);
            }
        }

        private void Sand_Key(int i)
        {
            Form1 lForm1 = (Form1)this.Owner;
            lForm1.StrValue = Buttons[i - 1].Text;
            Close();
        }

        private void FormScriptHelper_Load(object sender, EventArgs e)
        {
            var _point = new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y);
            Top = _point.Y + 10;
            Left = _point.X - 50;
        }

        private void FormScriptHelper_Deactivate(object sender, EventArgs e)
        {
            Close();
        }
    }
}
