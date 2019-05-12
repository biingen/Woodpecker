using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using jini;
using System.Windows.Forms;

namespace AutoTest
{
    public partial class FormLog : Form
    {
        public FormLog()
        {
            InitializeComponent();
        }

        string sPath = Application.StartupPath + "\\Config.ini";
        string MailPath = Application.StartupPath + "\\Mail.ini";

        public void FormLog_Load(object sender, EventArgs e)
        {
            if (ini12.INIRead(sPath, "LogSearch", "TextNum", "") != "")
            {
                ItemcomboBox.Text = ini12.INIRead(sPath, "LogSearch", "TextNum", "");
                int caseSwitch = Convert.ToInt32(ini12.INIRead(sPath, "LogSearch", "TextNum", ""));
                switch (caseSwitch)
                {
                    case 0:
                        Textlabel.Visible = false;
                        Timeslabel.Visible = false;
                        Searchtext0.Visible = false;
                        Timestext0.Visible = false;
                        Searchtext1.Visible = false;
                        Timestext1.Visible = false;
                        Searchtext2.Visible = false;
                        Timestext2.Visible = false;
                        Searchtext3.Visible = false;
                        Timestext3.Visible = false;
                        Searchtext4.Visible = false;
                        Timestext4.Visible = false;
                        Searchtext5.Visible = false;
                        Timestext5.Visible = false;
                        Searchtext6.Visible = false;
                        Timestext6.Visible = false;
                        Searchtext7.Visible = false;
                        Timestext7.Visible = false;
                        Searchtext8.Visible = false;
                        Timestext8.Visible = false;
                        Searchtext9.Visible = false;
                        Timestext9.Visible = false;
                        Sendmail.Visible = false;
                        ACcontrol.Visible = false;
                        break;
                    case 1:
                        Textlabel.Visible = true;
                        Timeslabel.Visible = true;
                        Searchtext0.Visible = true;
                        Timestext0.Visible = true;
                        Searchtext1.Visible = false;
                        Timestext1.Visible = false;
                        Searchtext2.Visible = false;
                        Timestext2.Visible = false;
                        Searchtext3.Visible = false;
                        Timestext3.Visible = false;
                        Searchtext4.Visible = false;
                        Timestext4.Visible = false;
                        Searchtext5.Visible = false;
                        Timestext5.Visible = false;
                        Searchtext6.Visible = false;
                        Timestext6.Visible = false;
                        Searchtext7.Visible = false;
                        Timestext7.Visible = false;
                        Searchtext8.Visible = false;
                        Timestext8.Visible = false;
                        Searchtext9.Visible = false;
                        Timestext9.Visible = false;
                        Sendmail.Visible = true;
                        ACcontrol.Visible = true;
                        break;
                    case 2:
                        Textlabel.Visible = true;
                        Timeslabel.Visible = true;
                        Searchtext0.Visible = true;
                        Timestext0.Visible = true;
                        Searchtext1.Visible = true;
                        Timestext1.Visible = true;
                        Searchtext2.Visible = false;
                        Timestext2.Visible = false;
                        Searchtext3.Visible = false;
                        Timestext3.Visible = false;
                        Searchtext4.Visible = false;
                        Timestext4.Visible = false;
                        Searchtext5.Visible = false;
                        Timestext5.Visible = false;
                        Searchtext6.Visible = false;
                        Timestext6.Visible = false;
                        Searchtext7.Visible = false;
                        Timestext7.Visible = false;
                        Searchtext8.Visible = false;
                        Timestext8.Visible = false;
                        Searchtext9.Visible = false;
                        Timestext9.Visible = false;
                        Sendmail.Visible = true;
                        ACcontrol.Visible = true;
                        break;
                    case 3:
                        Textlabel.Visible = true;
                        Timeslabel.Visible = true;
                        Searchtext0.Visible = true;
                        Timestext0.Visible = true;
                        Searchtext1.Visible = true;
                        Timestext1.Visible = true;
                        Searchtext2.Visible = true;
                        Timestext2.Visible = true;
                        Searchtext3.Visible = false;
                        Timestext3.Visible = false;
                        Searchtext4.Visible = false;
                        Timestext4.Visible = false;
                        Searchtext5.Visible = false;
                        Timestext5.Visible = false;
                        Searchtext6.Visible = false;
                        Timestext6.Visible = false;
                        Searchtext7.Visible = false;
                        Timestext7.Visible = false;
                        Searchtext8.Visible = false;
                        Timestext8.Visible = false;
                        Searchtext9.Visible = false;
                        Timestext9.Visible = false;
                        Sendmail.Visible = true;
                        ACcontrol.Visible = true;
                        break;
                    case 4:
                        Textlabel.Visible = true;
                        Timeslabel.Visible = true;
                        Searchtext0.Visible = true;
                        Timestext0.Visible = true;
                        Searchtext1.Visible = true;
                        Timestext1.Visible = true;
                        Searchtext2.Visible = true;
                        Timestext2.Visible = true;
                        Searchtext3.Visible = true;
                        Timestext3.Visible = true;
                        Searchtext4.Visible = false;
                        Timestext4.Visible = false;
                        Searchtext5.Visible = false;
                        Timestext5.Visible = false;
                        Searchtext6.Visible = false;
                        Timestext6.Visible = false;
                        Searchtext7.Visible = false;
                        Timestext7.Visible = false;
                        Searchtext8.Visible = false;
                        Timestext8.Visible = false;
                        Searchtext9.Visible = false;
                        Timestext9.Visible = false;
                        Sendmail.Visible = true;
                        ACcontrol.Visible = true;
                        break;
                    case 5:
                        Textlabel.Visible = true;
                        Timeslabel.Visible = true;
                        Searchtext0.Visible = true;
                        Timestext0.Visible = true;
                        Searchtext1.Visible = true;
                        Timestext1.Visible = true;
                        Searchtext2.Visible = true;
                        Timestext2.Visible = true;
                        Searchtext3.Visible = true;
                        Timestext3.Visible = true;
                        Searchtext4.Visible = true;
                        Timestext4.Visible = true;
                        Searchtext5.Visible = false;
                        Timestext5.Visible = false;
                        Searchtext6.Visible = false;
                        Timestext6.Visible = false;
                        Searchtext7.Visible = false;
                        Timestext7.Visible = false;
                        Searchtext8.Visible = false;
                        Timestext8.Visible = false;
                        Searchtext9.Visible = false;
                        Timestext9.Visible = false;
                        Sendmail.Visible = true;
                        ACcontrol.Visible = true;
                        break;
                    case 6:
                        Textlabel.Visible = true;
                        Timeslabel.Visible = true;
                        Searchtext0.Visible = true;
                        Timestext0.Visible = true;
                        Searchtext1.Visible = true;
                        Timestext1.Visible = true;
                        Searchtext2.Visible = true;
                        Timestext2.Visible = true;
                        Searchtext3.Visible = true;
                        Timestext3.Visible = true;
                        Searchtext4.Visible = true;
                        Timestext4.Visible = true;
                        Searchtext5.Visible = true;
                        Timestext5.Visible = true;
                        Searchtext6.Visible = false;
                        Timestext6.Visible = false;
                        Searchtext7.Visible = false;
                        Timestext7.Visible = false;
                        Searchtext8.Visible = false;
                        Timestext8.Visible = false;
                        Searchtext9.Visible = false;
                        Timestext9.Visible = false;
                        Sendmail.Visible = true;
                        ACcontrol.Visible = true;
                        break;
                    case 7:
                        Textlabel.Visible = true;
                        Timeslabel.Visible = true;
                        Searchtext0.Visible = true;
                        Timestext0.Visible = true;
                        Searchtext1.Visible = true;
                        Timestext1.Visible = true;
                        Searchtext2.Visible = true;
                        Timestext2.Visible = true;
                        Searchtext3.Visible = true;
                        Timestext3.Visible = true;
                        Searchtext4.Visible = true;
                        Timestext4.Visible = true;
                        Searchtext5.Visible = true;
                        Timestext5.Visible = true;
                        Searchtext6.Visible = true;
                        Timestext6.Visible = true;
                        Searchtext7.Visible = false;
                        Timestext7.Visible = false;
                        Searchtext8.Visible = false;
                        Timestext8.Visible = false;
                        Searchtext9.Visible = false;
                        Timestext9.Visible = false;
                        Sendmail.Visible = true;
                        ACcontrol.Visible = true;
                        break;
                    case 8:
                        Textlabel.Visible = true;
                        Timeslabel.Visible = true;
                        Searchtext0.Visible = true;
                        Timestext0.Visible = true;
                        Searchtext1.Visible = true;
                        Timestext1.Visible = true;
                        Searchtext2.Visible = true;
                        Timestext2.Visible = true;
                        Searchtext3.Visible = true;
                        Timestext3.Visible = true;
                        Searchtext4.Visible = true;
                        Timestext4.Visible = true;
                        Searchtext5.Visible = true;
                        Timestext5.Visible = true;
                        Searchtext6.Visible = true;
                        Timestext6.Visible = true;
                        Searchtext7.Visible = true;
                        Timestext7.Visible = true;
                        Searchtext8.Visible = false;
                        Timestext8.Visible = false;
                        Searchtext9.Visible = false;
                        Timestext9.Visible = false;
                        Sendmail.Visible = true;
                        ACcontrol.Visible = true;
                        break;
                    case 9:
                        Textlabel.Visible = true;
                        Timeslabel.Visible = true;
                        Searchtext0.Visible = true;
                        Timestext0.Visible = true;
                        Searchtext1.Visible = true;
                        Timestext1.Visible = true;
                        Searchtext2.Visible = true;
                        Timestext2.Visible = true;
                        Searchtext3.Visible = true;
                        Timestext3.Visible = true;
                        Searchtext4.Visible = true;
                        Timestext4.Visible = true;
                        Searchtext5.Visible = true;
                        Timestext5.Visible = true;
                        Searchtext6.Visible = true;
                        Timestext6.Visible = true;
                        Searchtext7.Visible = true;
                        Timestext7.Visible = true;
                        Searchtext8.Visible = true;
                        Timestext8.Visible = true;
                        Searchtext9.Visible = false;
                        Timestext9.Visible = false;
                        Sendmail.Visible = true;
                        ACcontrol.Visible = true;
                        break;
                    case 10:
                        Textlabel.Visible = true;
                        Timeslabel.Visible = true;
                        Searchtext0.Visible = true;
                        Timestext0.Visible = true;
                        Searchtext1.Visible = true;
                        Timestext1.Visible = true;
                        Searchtext2.Visible = true;
                        Timestext2.Visible = true;
                        Searchtext3.Visible = true;
                        Timestext3.Visible = true;
                        Searchtext4.Visible = true;
                        Timestext4.Visible = true;
                        Searchtext5.Visible = true;
                        Timestext5.Visible = true;
                        Searchtext6.Visible = true;
                        Timestext6.Visible = true;
                        Searchtext7.Visible = true;
                        Timestext7.Visible = true;
                        Searchtext8.Visible = true;
                        Timestext8.Visible = true;
                        Searchtext9.Visible = true;
                        Timestext9.Visible = true;
                        Sendmail.Visible = true;
                        ACcontrol.Visible = true;
                        break;
                }
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "TextNum", "0");
                Textlabel.Visible = false;
                Timeslabel.Visible = false;
                Searchtext0.Visible = false;
                Timestext0.Visible = false;
                Searchtext1.Visible = false;
                Timestext1.Visible = false;
                Searchtext2.Visible = false;
                Timestext2.Visible = false;
                Searchtext3.Visible = false;
                Timestext3.Visible = false;
                Searchtext4.Visible = false;
                Timestext4.Visible = false;
                Searchtext5.Visible = false;
                Timestext5.Visible = false;
                Searchtext6.Visible = false;
                Timestext6.Visible = false;
                Searchtext7.Visible = false;
                Timestext7.Visible = false;
                Searchtext8.Visible = false;
                Timestext8.Visible = false;
                Searchtext9.Visible = false;
                Timestext9.Visible = false;
                Sendmail.Visible = false;
                ACcontrol.Visible = false;
            }

            Searchtext0.Text = ini12.INIRead(sPath, "LogSearch", "Text0", "");
            Timestext0.Text = ini12.INIRead(sPath, "LogSearch", "Times0", "");
            Searchtext1.Text = ini12.INIRead(sPath, "LogSearch", "Text1", "");
            Timestext1.Text = ini12.INIRead(sPath, "LogSearch", "Times1", "");
            Searchtext2.Text = ini12.INIRead(sPath, "LogSearch", "Text2", "");
            Timestext2.Text = ini12.INIRead(sPath, "LogSearch", "Times2", "");
            Searchtext3.Text = ini12.INIRead(sPath, "LogSearch", "Text3", "");
            Timestext3.Text = ini12.INIRead(sPath, "LogSearch", "Times3", "");
            Searchtext4.Text = ini12.INIRead(sPath, "LogSearch", "Text4", "");
            Timestext4.Text = ini12.INIRead(sPath, "LogSearch", "Times4", "");
            Searchtext5.Text = ini12.INIRead(sPath, "LogSearch", "Text5", "");
            Timestext5.Text = ini12.INIRead(sPath, "LogSearch", "Times5", "");
            Searchtext6.Text = ini12.INIRead(sPath, "LogSearch", "Text6", "");
            Timestext6.Text = ini12.INIRead(sPath, "LogSearch", "Times6", "");
            Searchtext7.Text = ini12.INIRead(sPath, "LogSearch", "Text7", "");
            Timestext7.Text = ini12.INIRead(sPath, "LogSearch", "Times7", "");
            Searchtext8.Text = ini12.INIRead(sPath, "LogSearch", "Text8", "");
            Timestext8.Text = ini12.INIRead(sPath, "LogSearch", "Times8", "");
            Searchtext9.Text = ini12.INIRead(sPath, "LogSearch", "Text9", "");
            Timestext9.Text = ini12.INIRead(sPath, "LogSearch", "Times9", "");

            if (ini12.INIRead(sPath, "LogSearch", "Camerarecord", "") != "")
            {
                if (int.Parse(ini12.INIRead(sPath, "LogSearch", "Camerarecord", "")) == 1)
                {
                    Camerarecord.Checked = true;
                }
                else
                {
                    Camerarecord.Checked = false;
                }
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "Camerarecord", "0");
                Camerarecord.Checked = false;
            }

            if (ini12.INIRead(sPath, "LogSearch", "Camerashot", "") != "")
            {
                if (int.Parse(ini12.INIRead(sPath, "LogSearch", "Camerashot", "")) == 1)
                {
                    Camerashot.Checked = true;
                }
                else
                {
                    Camerashot.Checked = false;
                }
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "Camerashot", "0");
                Camerashot.Checked = false;
            }

            if (ini12.INIRead(sPath, "LogSearch", "Sendmail", "") != "")
            {
                if (int.Parse(ini12.INIRead(sPath, "LogSearch", "Sendmail", "")) == 1)
                {
                    Sendmail.Checked = true;
                }
                else
                {
                    Sendmail.Checked = false;
                }
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "Sendmail", "0");
                Sendmail.Checked = false;
            }

            if (ini12.INIRead(sPath, "LogSearch", "Savelog", "") != "")
            {
                if (int.Parse(ini12.INIRead(sPath, "LogSearch", "Savelog", "")) == 1)
                {
                    Savelog.Checked = true;
                }
                else
                {
                    Savelog.Checked = false;
                }
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "Savelog", "0");
                Savelog.Checked = false;
            }

            if (ini12.INIRead(sPath, "LogSearch", "Showmessage", "") != "")
            {
                if (int.Parse(ini12.INIRead(sPath, "LogSearch", "Showmessage", "")) == 1)
                {
                    Showmessage.Checked = true;
                }
                else
                {
                    Showmessage.Checked = false;
                }
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "Showmessage", "0");
                Showmessage.Checked = false;
            }

            if (ini12.INIRead(sPath, "LogSearch", "ACcontrol", "") != "")
            {
                if (int.Parse(ini12.INIRead(sPath, "LogSearch", "ACcontrol", "")) == 1)
                {
                    ACcontrol.Checked = true;
                }
                else
                {
                    ACcontrol.Checked = false;
                }
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "ACcontrol", "0");
                ACcontrol.Checked = false;
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            ini12.INIWrite(sPath, "LogSearch", "TextNum", ItemcomboBox.Text);

            if (Searchtext0.Visible == true)
            {
                if (string.IsNullOrEmpty(Searchtext0.Text))
                {
                    Searchtext0.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Keyword must exist !";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(Timestext0.Text) || Timestext0.Text == "0")
                {
                    Timestext0.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Times can't equal 0 or null !";
                    Global.FormLog = false;
                }
                else
                {
                    Searchtext0.BackColor = default(Color);
                    Timestext0.BackColor = default(Color);
                    ini12.INIWrite(sPath, "LogSearch", "Text0", Searchtext0.Text.Trim());
                    ini12.INIWrite(sPath, "LogSearch", "Times0", Timestext0.Text.Trim());
                }
            }
            else
            {
                Searchtext0.BackColor = default(Color);
                Timestext0.BackColor = default(Color);
            }

            if (Searchtext1.Visible == true)
            {
                if (string.IsNullOrEmpty(Searchtext1.Text))
                {
                    Searchtext1.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Keyword must exist !";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(Timestext1.Text) || Timestext1.Text == "0")
                {
                    Timestext1.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Times can't equal 0 or null !";
                    Global.FormLog = false;
                }
                else
                {
                    Searchtext1.BackColor = default(Color);
                    Timestext1.BackColor = default(Color);
                    ini12.INIWrite(sPath, "LogSearch", "Text1", Searchtext1.Text.Trim());
                    ini12.INIWrite(sPath, "LogSearch", "Times1", Timestext1.Text.Trim());
                }
            }
            else
            {
                Searchtext1.BackColor = default(Color);
                Timestext1.BackColor = default(Color);
            }

            if (Searchtext2.Visible == true)
            {
                if (string.IsNullOrEmpty(Searchtext2.Text))
                {
                    Searchtext2.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Keyword must exist !";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(Timestext2.Text) || Timestext2.Text == "0")
                {
                    Timestext2.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Times can't equal 0 or null !";
                    Global.FormLog = false;
                }
                else
                {
                    Searchtext2.BackColor = default(Color);
                    Timestext2.BackColor = default(Color);
                    ini12.INIWrite(sPath, "LogSearch", "Text2", Searchtext2.Text.Trim());
                    ini12.INIWrite(sPath, "LogSearch", "Times2", Timestext2.Text.Trim());
                }
            }
            else
            {
                Searchtext2.BackColor = default(Color);
                Timestext2.BackColor = default(Color);
            }

            if (Searchtext3.Visible == true)
            {
                if (string.IsNullOrEmpty(Searchtext3.Text))
                {
                    Searchtext3.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Keyword must exist !";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(Timestext3.Text) || Timestext3.Text == "0")
                {
                    Timestext3.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Times can't equal 0 or null !";
                    Global.FormLog = false;
                }
                else
                {
                    Searchtext3.BackColor = default(Color);
                    Timestext3.BackColor = default(Color);
                    ini12.INIWrite(sPath, "LogSearch", "Text3", Searchtext3.Text.Trim());
                    ini12.INIWrite(sPath, "LogSearch", "Times3", Timestext3.Text.Trim());
                }
            }
            else
            {
                Searchtext3.BackColor = default(Color);
                Timestext3.BackColor = default(Color);
            }

            if (Searchtext4.Visible == true)
            {
                if (string.IsNullOrEmpty(Searchtext4.Text))
                {
                    Searchtext4.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Keyword must exist !";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(Timestext4.Text) || Timestext4.Text == "0")
                {
                    Timestext4.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Times can't equal 0 or null !";
                    Global.FormLog = false;
                }
                else
                {
                    Searchtext4.BackColor = default(Color);
                    Timestext4.BackColor = default(Color);
                    ini12.INIWrite(sPath, "LogSearch", "Text4", Searchtext4.Text.Trim());
                    ini12.INIWrite(sPath, "LogSearch", "Times4", Timestext4.Text.Trim());
                }
            }
            else
            {
                Searchtext4.BackColor = default(Color);
                Timestext4.BackColor = default(Color);
            }

            if (Searchtext5.Visible == true)
            {
                if (string.IsNullOrEmpty(Searchtext5.Text))
                {
                    Searchtext5.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Keyword must exist !";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(Timestext5.Text) || Timestext5.Text == "0")
                {
                    Timestext5.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Times can't equal 0 or null !";
                    Global.FormLog = false;
                }
                else
                {
                    Searchtext5.BackColor = default(Color);
                    Timestext5.BackColor = default(Color);
                    ini12.INIWrite(sPath, "LogSearch", "Text5", Searchtext5.Text.Trim());
                    ini12.INIWrite(sPath, "LogSearch", "Times5", Timestext5.Text.Trim());
                }
            }
            else
            {
                Searchtext5.BackColor = default(Color);
                Timestext5.BackColor = default(Color);
            }

            if (Searchtext6.Visible == true)
            {
                if (string.IsNullOrEmpty(Searchtext6.Text))
                {
                    Searchtext6.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Keyword must exist !";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(Timestext6.Text) || Timestext6.Text == "0")
                {
                    Timestext6.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Times can't equal 0 or null !";
                    Global.FormLog = false;
                }
                else
                {
                    Searchtext6.BackColor = default(Color);
                    Timestext6.BackColor = default(Color);
                    ini12.INIWrite(sPath, "LogSearch", "Text6", Searchtext6.Text.Trim());
                    ini12.INIWrite(sPath, "LogSearch", "Times6", Timestext6.Text.Trim());
                }
            }
            else
            {
                Searchtext6.BackColor = default(Color);
                Timestext6.BackColor = default(Color);
            }

            if (Searchtext7.Visible == true)
            {
                if (string.IsNullOrEmpty(Searchtext7.Text))
                {
                    Searchtext7.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Keyword must exist !";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(Timestext7.Text) || Timestext7.Text == "0")
                {
                    Timestext7.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Times can't equal 0 or null !";
                    Global.FormLog = false;
                }
                else
                {
                    Searchtext7.BackColor = default(Color);
                    Timestext7.BackColor = default(Color);
                    ini12.INIWrite(sPath, "LogSearch", "Text7", Searchtext7.Text.Trim());
                    ini12.INIWrite(sPath, "LogSearch", "Times7", Timestext7.Text.Trim());
                }
            }
            else
            {
                Searchtext7.BackColor = default(Color);
                Timestext7.BackColor = default(Color);
            }

            if (Searchtext8.Visible == true)
            {
                if (string.IsNullOrEmpty(Searchtext8.Text))
                {
                    Searchtext8.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Keyword must exist !";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(Timestext8.Text) || Timestext8.Text == "0")
                {
                    Timestext8.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Times can't equal 0 or null !";
                    Global.FormLog = false;
                }
                else
                {
                    Searchtext8.BackColor = default(Color);
                    Timestext8.BackColor = default(Color);
                    ini12.INIWrite(sPath, "LogSearch", "Text8", Searchtext8.Text.Trim());
                    ini12.INIWrite(sPath, "LogSearch", "Times8", Timestext8.Text.Trim());
                }
            }
            else
            {
                Searchtext8.BackColor = default(Color);
                Timestext8.BackColor = default(Color);
            }

            if (Searchtext9.Visible == true)
            {
                if (string.IsNullOrEmpty(Searchtext9.Text))
                {
                    Searchtext9.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Keyword must exist !";
                    Global.FormLog = false;
                }
                else if (string.IsNullOrEmpty(Timestext9.Text) || Timestext9.Text == "0")
                {
                    Timestext9.BackColor = System.Drawing.Color.Yellow;
                    SavedLabel.Text = "Times can't equal 0 or null !";
                    Global.FormLog = false;
                }
                else
                {
                    Searchtext9.BackColor = default(Color);
                    Timestext9.BackColor = default(Color);
                    ini12.INIWrite(sPath, "LogSearch", "Text9", Searchtext9.Text.Trim());
                    ini12.INIWrite(sPath, "LogSearch", "Times9", Timestext9.Text.Trim());
                }
            }
            else
            {
                Searchtext9.BackColor = default(Color);
                Timestext9.BackColor = default(Color);
            }
          
            if (
                Searchtext0.BackColor != System.Drawing.Color.Yellow &&
                Timestext0.BackColor != System.Drawing.Color.Yellow &&
                Searchtext1.BackColor != System.Drawing.Color.Yellow &&
                Timestext1.BackColor != System.Drawing.Color.Yellow &&
                Searchtext2.BackColor != System.Drawing.Color.Yellow &&
                Timestext2.BackColor != System.Drawing.Color.Yellow &&
                Searchtext3.BackColor != System.Drawing.Color.Yellow &&
                Timestext3.BackColor != System.Drawing.Color.Yellow &&
                Searchtext4.BackColor != System.Drawing.Color.Yellow &&
                Timestext4.BackColor != System.Drawing.Color.Yellow &&
                Searchtext5.BackColor != System.Drawing.Color.Yellow &&
                Timestext5.BackColor != System.Drawing.Color.Yellow &&
                Searchtext6.BackColor != System.Drawing.Color.Yellow &&
                Timestext6.BackColor != System.Drawing.Color.Yellow &&
                Searchtext7.BackColor != System.Drawing.Color.Yellow &&
                Timestext7.BackColor != System.Drawing.Color.Yellow &&
                Searchtext8.BackColor != System.Drawing.Color.Yellow &&
                Timestext8.BackColor != System.Drawing.Color.Yellow &&
                Searchtext9.BackColor != System.Drawing.Color.Yellow &&
                Timestext9.BackColor != System.Drawing.Color.Yellow
                )
            {
                SavedLabel.Text = "Save Successfully !";
                Global.FormLog = true;
            }            
        }

        private void Camerarecord_CheckedChanged(object sender, EventArgs e)
        {
            if (Camerarecord.Checked == true)
            {
                ini12.INIWrite(sPath, "LogSearch", "Camerarecord", "1");
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "Camerarecord", "0");
            }
        }

        private void Camerashot_CheckedChanged(object sender, EventArgs e)
        {
            if (Camerashot.Checked == true)
            {
                ini12.INIWrite(sPath, "LogSearch", "Camerashot", "1");
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "Camerashot", "0");
            }
        }

        private void Sendmail_CheckedChanged(object sender, EventArgs e)
        {
            if (Sendmail.Checked == true)
            {
                if(ini12.INIRead(MailPath, "Send Mail", "value", "") == "0")
                    MessageBox.Show("Please enable the mail function in Mail Setting");

                ini12.INIWrite(sPath, "LogSearch", "Sendmail", "1");
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "Sendmail", "0");
            }
        }

        private void savelog_CheckedChanged(object sender, EventArgs e)
        {
            if (Savelog.Checked == true)
            {
                ini12.INIWrite(sPath, "LogSearch", "Savelog", "1");
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "Savelog", "0");
            }
        }

        private void Showmessage_CheckedChanged(object sender, EventArgs e)
        {
            if (Showmessage.Checked == true)
            {
                ini12.INIWrite(sPath, "LogSearch", "Showmessage", "1");
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "Showmessage", "0");
            }
        }

        private void ACcontrol_CheckedChanged(object sender, EventArgs e)
        {
            if (ACcontrol.Checked == true)
            {
                ini12.INIWrite(sPath, "LogSearch", "ACcontrol", "1");
            }
            else
            {
                ini12.INIWrite(sPath, "LogSearch", "ACcontrol", "0");
            }
        }

        private void ItemcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int caseSwitch = Convert.ToInt32(ItemcomboBox.Text.Trim());
            switch (caseSwitch)
            {
                case 1:
                    Textlabel.Visible = true;
                    Timeslabel.Visible = true;
                    Searchtext0.Visible = true;
                    Timestext0.Visible = true;
                    Searchtext1.Visible = false;
                    Timestext1.Visible = false;
                    Searchtext2.Visible = false;
                    Timestext2.Visible = false;
                    Searchtext3.Visible = false;
                    Timestext3.Visible = false;
                    Searchtext4.Visible = false;
                    Timestext4.Visible = false;
                    Searchtext5.Visible = false;
                    Timestext5.Visible = false;
                    Searchtext6.Visible = false;
                    Timestext6.Visible = false;
                    Searchtext7.Visible = false;
                    Timestext7.Visible = false;
                    Searchtext8.Visible = false;
                    Timestext8.Visible = false;
                    Searchtext9.Visible = false;
                    Timestext9.Visible = false;
                    Sendmail.Visible = true;
                    ACcontrol.Visible = true;
                    break;
                case 2:
                    Textlabel.Visible = true;
                    Timeslabel.Visible = true;
                    Searchtext0.Visible = true;
                    Timestext0.Visible = true;
                    Searchtext1.Visible = true;
                    Timestext1.Visible = true;
                    Searchtext2.Visible = false;
                    Timestext2.Visible = false;
                    Searchtext3.Visible = false;
                    Timestext3.Visible = false;
                    Searchtext4.Visible = false;
                    Timestext4.Visible = false;
                    Searchtext5.Visible = false;
                    Timestext5.Visible = false;
                    Searchtext6.Visible = false;
                    Timestext6.Visible = false;
                    Searchtext7.Visible = false;
                    Timestext7.Visible = false;
                    Searchtext8.Visible = false;
                    Timestext8.Visible = false;
                    Searchtext9.Visible = false;
                    Timestext9.Visible = false;
                    Sendmail.Visible = true;
                    ACcontrol.Visible = true;
                    break;
                case 3:
                    Textlabel.Visible = true;
                    Timeslabel.Visible = true;
                    Searchtext0.Visible = true;
                    Timestext0.Visible = true;
                    Searchtext1.Visible = true;
                    Timestext1.Visible = true;
                    Searchtext2.Visible = true;
                    Timestext2.Visible = true;
                    Searchtext3.Visible = false;
                    Timestext3.Visible = false;
                    Searchtext4.Visible = false;
                    Timestext4.Visible = false;
                    Searchtext5.Visible = false;
                    Timestext5.Visible = false;
                    Searchtext6.Visible = false;
                    Timestext6.Visible = false;
                    Searchtext7.Visible = false;
                    Timestext7.Visible = false;
                    Searchtext8.Visible = false;
                    Timestext8.Visible = false;
                    Searchtext9.Visible = false;
                    Timestext9.Visible = false;
                    Sendmail.Visible = true;
                    ACcontrol.Visible = true;
                    break;
                case 4:
                    Textlabel.Visible = true;
                    Timeslabel.Visible = true;
                    Searchtext0.Visible = true;
                    Timestext0.Visible = true;
                    Searchtext1.Visible = true;
                    Timestext1.Visible = true;
                    Searchtext2.Visible = true;
                    Timestext2.Visible = true;
                    Searchtext3.Visible = true;
                    Timestext3.Visible = true;
                    Searchtext4.Visible = false;
                    Timestext4.Visible = false;
                    Searchtext5.Visible = false;
                    Timestext5.Visible = false;
                    Searchtext6.Visible = false;
                    Timestext6.Visible = false;
                    Searchtext7.Visible = false;
                    Timestext7.Visible = false;
                    Searchtext8.Visible = false;
                    Timestext8.Visible = false;
                    Searchtext9.Visible = false;
                    Timestext9.Visible = false;
                    Sendmail.Visible = true;
                    ACcontrol.Visible = true;
                    break;
                case 5:
                    Textlabel.Visible = true;
                    Timeslabel.Visible = true;
                    Searchtext0.Visible = true;
                    Timestext0.Visible = true;
                    Searchtext1.Visible = true;
                    Timestext1.Visible = true;
                    Searchtext2.Visible = true;
                    Timestext2.Visible = true;
                    Searchtext3.Visible = true;
                    Timestext3.Visible = true;
                    Searchtext4.Visible = true;
                    Timestext4.Visible = true;
                    Searchtext5.Visible = false;
                    Timestext5.Visible = false;
                    Searchtext6.Visible = false;
                    Timestext6.Visible = false;
                    Searchtext7.Visible = false;
                    Timestext7.Visible = false;
                    Searchtext8.Visible = false;
                    Timestext8.Visible = false;
                    Searchtext9.Visible = false;
                    Timestext9.Visible = false;
                    Sendmail.Visible = true;
                    ACcontrol.Visible = true;
                    break;
                case 6:
                    Textlabel.Visible = true;
                    Timeslabel.Visible = true;
                    Searchtext0.Visible = true;
                    Timestext0.Visible = true;
                    Searchtext1.Visible = true;
                    Timestext1.Visible = true;
                    Searchtext2.Visible = true;
                    Timestext2.Visible = true;
                    Searchtext3.Visible = true;
                    Timestext3.Visible = true;
                    Searchtext4.Visible = true;
                    Timestext4.Visible = true;
                    Searchtext5.Visible = true;
                    Timestext5.Visible = true;
                    Searchtext6.Visible = false;
                    Timestext6.Visible = false;
                    Searchtext7.Visible = false;
                    Timestext7.Visible = false;
                    Searchtext8.Visible = false;
                    Timestext8.Visible = false;
                    Searchtext9.Visible = false;
                    Timestext9.Visible = false;
                    Sendmail.Visible = true;
                    ACcontrol.Visible = true;
                    break;
                case 7:
                    Textlabel.Visible = true;
                    Timeslabel.Visible = true;
                    Searchtext0.Visible = true;
                    Timestext0.Visible = true;
                    Searchtext1.Visible = true;
                    Timestext1.Visible = true;
                    Searchtext2.Visible = true;
                    Timestext2.Visible = true;
                    Searchtext3.Visible = true;
                    Timestext3.Visible = true;
                    Searchtext4.Visible = true;
                    Timestext4.Visible = true;
                    Searchtext5.Visible = true;
                    Timestext5.Visible = true;
                    Searchtext6.Visible = true;
                    Timestext6.Visible = true;
                    Searchtext7.Visible = false;
                    Timestext7.Visible = false;
                    Searchtext8.Visible = false;
                    Timestext8.Visible = false;
                    Searchtext9.Visible = false;
                    Timestext9.Visible = false;
                    Sendmail.Visible = true;
                    ACcontrol.Visible = true;
                    break;
                case 8:
                    Textlabel.Visible = true;
                    Timeslabel.Visible = true;
                    Searchtext0.Visible = true;
                    Timestext0.Visible = true;
                    Searchtext1.Visible = true;
                    Timestext1.Visible = true;
                    Searchtext2.Visible = true;
                    Timestext2.Visible = true;
                    Searchtext3.Visible = true;
                    Timestext3.Visible = true;
                    Searchtext4.Visible = true;
                    Timestext4.Visible = true;
                    Searchtext5.Visible = true;
                    Timestext5.Visible = true;
                    Searchtext6.Visible = true;
                    Timestext6.Visible = true;
                    Searchtext7.Visible = true;
                    Timestext7.Visible = true;
                    Searchtext8.Visible = false;
                    Timestext8.Visible = false;
                    Searchtext9.Visible = false;
                    Timestext9.Visible = false;
                    Sendmail.Visible = true;
                    ACcontrol.Visible = true;
                    break;
                case 9:
                    Textlabel.Visible = true;
                    Timeslabel.Visible = true;
                    Searchtext0.Visible = true;
                    Timestext0.Visible = true;
                    Searchtext1.Visible = true;
                    Timestext1.Visible = true;
                    Searchtext2.Visible = true;
                    Timestext2.Visible = true;
                    Searchtext3.Visible = true;
                    Timestext3.Visible = true;
                    Searchtext4.Visible = true;
                    Timestext4.Visible = true;
                    Searchtext5.Visible = true;
                    Timestext5.Visible = true;
                    Searchtext6.Visible = true;
                    Timestext6.Visible = true;
                    Searchtext7.Visible = true;
                    Timestext7.Visible = true;
                    Searchtext8.Visible = true;
                    Timestext8.Visible = true;
                    Searchtext9.Visible = false;
                    Timestext9.Visible = false;
                    Sendmail.Visible = true;
                    ACcontrol.Visible = true;
                    break;
                case 10:
                    Textlabel.Visible = true;
                    Timeslabel.Visible = true;
                    Searchtext0.Visible = true;
                    Timestext0.Visible = true;
                    Searchtext1.Visible = true;
                    Timestext1.Visible = true;
                    Searchtext2.Visible = true;
                    Timestext2.Visible = true;
                    Searchtext3.Visible = true;
                    Timestext3.Visible = true;
                    Searchtext4.Visible = true;
                    Timestext4.Visible = true;
                    Searchtext5.Visible = true;
                    Timestext5.Visible = true;
                    Searchtext6.Visible = true;
                    Timestext6.Visible = true;
                    Searchtext7.Visible = true;
                    Timestext7.Visible = true;
                    Searchtext8.Visible = true;
                    Timestext8.Visible = true;
                    Searchtext9.Visible = true;
                    Timestext9.Visible = true;
                    Sendmail.Visible = true;
                    ACcontrol.Visible = true;
                    break;
                default:
                    Textlabel.Visible = false;
                    Timeslabel.Visible = false;
                    Searchtext0.Visible = false;
                    Timestext0.Visible = false;
                    Searchtext1.Visible = false;
                    Timestext1.Visible = false;
                    Searchtext2.Visible = false;
                    Timestext2.Visible = false;
                    Searchtext3.Visible = false;
                    Timestext3.Visible = false;
                    Searchtext4.Visible = false;
                    Timestext4.Visible = false;
                    Searchtext5.Visible = false;
                    Timestext5.Visible = false;
                    Searchtext6.Visible = false;
                    Timestext6.Visible = false;
                    Searchtext7.Visible = false;
                    Timestext7.Visible = false;
                    Searchtext8.Visible = false;
                    Timestext8.Visible = false;
                    Searchtext9.Visible = false;
                    Timestext9.Visible = false;
                    Sendmail.Visible = false;
                    ACcontrol.Visible = false;
                    break;
            }
        }
    }
}
