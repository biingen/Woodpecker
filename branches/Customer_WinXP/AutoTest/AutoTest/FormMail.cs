﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using jini;
using System.Xml.Linq;

namespace AutoTest
{
    public partial class FormMail : Form
    {
        public FormMail()
        {
            InitializeComponent();
        }

        string MailPath = Application.StartupPath + "\\Mail.ini";

        public void send()
        {
            string MailPath = Application.StartupPath + "\\Mail.ini";
            string ConfigPath = Application.StartupPath + "\\Config.ini";

            string To = ini12.INIRead(MailPath, "Mail Info", "To", "") + ",";
            int z = 0;
            string[] to = To.Split(new char[] { ',' });
            List<string> MailList = new List<string> { };

            while (to[z] != "")
            {
                MailList.Add(to[z]);
                z++;
            }

            string schedule2 = "", schedule3 = "", schedule4 = "", schedule5 = "";
            if (Global.Schedule_Num2_Exist == 1)
                schedule2 = ini12.INIRead(MailPath, "Test Case", "TestCase2", "") + ", " + "Test Loop: " + ini12.INIRead(ConfigPath, "Schedule2 Loop", "value", "") + ", " + "Test Time: " + ini12.INIRead(MailPath, "Total Test Time", "value2", "");
            else if (Global.Schedule_Num2_Exist == 0)
                schedule2 = "";

            if (Global.Schedule_Num3_Exist == 1)
                schedule3 = ini12.INIRead(MailPath, "Test Case", "TestCase3", "") + ", " + "Test Loop: " + ini12.INIRead(ConfigPath, "Schedule3 Loop", "value", "") + ", " + "Test Time: " + ini12.INIRead(MailPath, "Total Test Time", "value3", "");
            else if (Global.Schedule_Num3_Exist == 0)
                schedule3 = "";

            if (Global.Schedule_Num4_Exist == 1)
                schedule4 = ini12.INIRead(MailPath, "Test Case", "TestCase4", "") + ", " + "Test Loop: " + ini12.INIRead(ConfigPath, "Schedule4 Loop", "value", "") + ", " + "Test Time: " + ini12.INIRead(MailPath, "Total Test Time", "value4", "");
            else if (Global.Schedule_Num4_Exist == 0)
                schedule4 = "";

            if (Global.Schedule_Num5_Exist == 1)
                schedule5 = ini12.INIRead(MailPath, "Test Case", "TestCase5", "") + ", " + "Test Loop: " + ini12.INIRead(ConfigPath, "Schedule5 Loop", "value", "") + ", " + "Test Time: " + ini12.INIRead(MailPath, "Total Test Time", "value5", "");
            else if (Global.Schedule_Num5_Exist == 0)
                schedule5 = "";
            
            string Subject = "Stress test report";
            string Body =
                                    "Test Result" + "<br>" + "<br>" +

                                    "Test case : " + "<br>" +
                                    "1. " + ini12.INIRead(MailPath, "Test Case", "TestCase1", "") + ", " + "Test Loop: " + ini12.INIRead(ConfigPath, "Schedule1 Loop", "value", "") + ", " + "Test Time: " + ini12.INIRead(MailPath, "Total Test Time", "value1", "") + "<br>" +
                                    "2. " + schedule2 + "<br>" +
                                    "3. " + schedule3 + "<br>" +
                                    "4. " + schedule4 + "<br>" +
                                    "5. " + schedule5 + "<br>" + "<br>" +

                                    "Urtrack link : " + "http://fwtrack.tpvaoc.com/pts/issuelist.aspx?project=" + ini12.INIRead(MailPath, "Data Info", "ProjectNumber", "") + "<br>" +
                                    "Project name : " + ini12.INIRead(MailPath, "Mail Info", "ProjectName", "") + "<br>" +
                                    "Model name : " + ini12.INIRead(MailPath, "Mail Info", "ModelName", "") + "<br>" +
                                    "Version : " + ini12.INIRead(MailPath, "Mail Info", "Version", "") + "<br>" + "<br>" +

                                    "Tester : " + ini12.INIRead(MailPath, "Mail Info", "Tester", "") + "<br>" +
                                    "Total test time : " + ini12.INIRead(MailPath, "Total Test Time", "value", "") + "<br>" + "<br>" + "<br>" + "<br>" +
                                    "Please note this E-mail is sent by Alpha mail system automatically, do not reply. If you have any questions please contact system administrator.";

            SendMail(MailList, Subject, Body);
        }

        public void logsend()
        {
            string MailPath = Application.StartupPath + "\\Mail.ini";
            string ConfigPath = Application.StartupPath + "\\Config.ini";

            string To = ini12.INIRead(MailPath, "Mail Info", "To", "") + ",";
            int z = 0;
            string i = ini12.INIRead(ConfigPath, "LogSearch", "Nowvalue", "");
            string[] to = To.Split(new char[] { ',' });
            List<string> MailList = new List<string> { };

            while (to[z] != "")
            {
                MailList.Add(to[z]);
                z++;
            }
            
            string Subject = "Event Notification: " + ini12.INIRead(ConfigPath, "LogSearch", "Text" + i, "") + " ------ " + ini12.INIRead(ConfigPath, "LogSearch", "Times" + i, "") + " times";
            string Body =
                                    "Event Notification" + "<br>" + "<br>" + 

                                    "Search Keyword: " + ini12.INIRead(ConfigPath, "LogSearch", "Text" + i, "") + "<br>" +
                                    "Start Test time: " + ini12.INIRead(ConfigPath, "LogSearch", "StartTime", "") + "<br>" +
                                    "Search keyword time: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "<br>" + "<br>" + 
                                    "Please note this E-mail is sent by Alpha mail system automatically, do not reply. If you have any questions please contact system administrator." + "<br>";

            SendMail(MailList, Subject, Body);
        }

        public void SendMail(List<string> MailList, string Subject, string Body)
        {
            MailMessage msg = new MailMessage();

            msg.To.Add(string.Join(",", MailList.ToArray()));       //收件者，以逗號分隔不同收件者
            msg.From = new MailAddress(ini12.INIRead(MailPath, "Mail Info", "From", "") , ini12.INIRead(MailPath, "Mail Info", "From", ""), System.Text.Encoding.UTF8);
            msg.Subject = Subject;      //郵件標題 
            msg.SubjectEncoding = System.Text.Encoding.UTF8;        //郵件標題編碼  
            msg.Body = Body;        //郵件內容

            msg.IsBodyHtml = true;
            msg.BodyEncoding = System.Text.Encoding.UTF8;       //郵件內容編碼 
            msg.Priority = MailPriority.Normal;     //郵件優先級 

            //建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port 
            #region 其它 Host
            /*
            ~~~~~~~~~~~~~~~~~       outlook.com smtp.live.com port:25
            ~~~~~~~~~~~~~~~~~       yahoo smtp.mail.yahoo.com.tw port:465
            ~~~~~~~~~~~~~~~~~       smtp.gmail.com port:587
            ~~~~~~~~~~~~~~~~~       tpmx.tpvaoc.com port: 25        //公司內部的SMTP
            ~~~~~~~~~~~~~~~~~       msa.hinet.net port: 25
            */
            #endregion

            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);
            MySmtp.Credentials = new System.Net.NetworkCredential("alphatestteam@gmail.com", "16433587");     //設定你的帳號密碼
            MySmtp.EnableSsl = true;      //Gmial 的 smtp 需打開 SSL
            MySmtp.Send(msg);
        }

        public void SendMailBtn_Click(object sender, EventArgs e)
        {
            send();
        }

        public void SaveSchBtn_Click(object sender, EventArgs e)
        {
            if (SendMailcheckBox.Checked == true)
            {
                if (string.IsNullOrEmpty(FromTextBox.Text))
                {
                    SavedLabel.Text = "Sender must exist !";
                    FromTextBox.BackColor = System.Drawing.Color.Yellow;
                    Global.FormMail = false;
                }
                else if (string.IsNullOrEmpty(ToTextBox.Text))
                {
                    SavedLabel.Text = "Recipient must exist !";
                    ToTextBox.BackColor = System.Drawing.Color.Yellow;
                    Global.FormMail = false;
                }
                else
                {
                    ini12.INIWrite(MailPath, "Mail Info", "From", FromTextBox.Text.Trim());
                    ini12.INIWrite(MailPath, "Mail Info", "To", ToTextBox.Text.Trim());

                    ini12.INIWrite(MailPath, "Test Case", "TestCase1", Testcase1TextBox.Text.Trim());
                    ini12.INIWrite(MailPath, "Test Case", "TestCase2", Testcase2TextBox.Text.Trim());
                    ini12.INIWrite(MailPath, "Test Case", "TestCase3", Testcase3TextBox.Text.Trim());
                    ini12.INIWrite(MailPath, "Test Case", "TestCase4", Testcase4TextBox.Text.Trim());
                    ini12.INIWrite(MailPath, "Test Case", "TestCase5", Testcase5TextBox.Text.Trim());

                    ini12.INIWrite(MailPath, "Data Info", "ProjectNumber", ProjectNumberTextBox.Text.Trim());
                    ini12.INIWrite(MailPath, "Mail Info", "ProjectName", ProjectTextBox.Text.Trim());
                    ini12.INIWrite(MailPath, "Mail Info", "ModelName", ModelTextBox.Text.Trim());
                    ini12.INIWrite(MailPath, "Mail Info", "Version", VersionTextBox.Text.Trim());

                    ini12.INIWrite(MailPath, "Mail Info", "Tester", TesterTextBox.Text.Trim());
                    ini12.INIWrite(MailPath, "Total Test Time", "value", TotalTextBox.Text.Trim());

                    SavedLabel.Text = "Save Successfully !";
                    ToTextBox.BackColor = default(Color);
                    Global.FormMail = true;
                }
            }
        }

        public void FormMail_Load(object sender, EventArgs e)
        {
            if (ini12.INIRead(MailPath, "Send Mail", "value", "") != "")
            {
                if (int.Parse(ini12.INIRead(MailPath, "Send Mail", "value", "")) == 1)
                {
                    SendMailcheckBox.Checked = true;
                    FromTextBox.Enabled = true;
                    ToTextBox.Enabled = true;
                    Testcase1TextBox.Enabled = true;
                    Testcase2TextBox.Enabled = true;
                    Testcase3TextBox.Enabled = true;
                    Testcase4TextBox.Enabled = true;
                    Testcase5TextBox.Enabled = true;
                    ProjectNumberTextBox.Enabled = true;
                    ProjectTextBox.Enabled = true;
                    ModelTextBox.Enabled = true;
                    VersionTextBox.Enabled = true;
                    TesterTextBox.Enabled = true;
                    TotalTextBox.Enabled = true;
                }
                else
                {
                    SendMailcheckBox.Checked = false;
                    FromTextBox.Enabled = false;
                    ToTextBox.Enabled = false;
                    Testcase1TextBox.Enabled = false;
                    Testcase2TextBox.Enabled = false;
                    Testcase3TextBox.Enabled = false;
                    Testcase4TextBox.Enabled = false;
                    Testcase5TextBox.Enabled = false;
                    ProjectNumberTextBox.Enabled = false;
                    ProjectTextBox.Enabled = false;
                    ModelTextBox.Enabled = false;
                    VersionTextBox.Enabled = false;
                    TesterTextBox.Enabled = false;
                    TotalTextBox.Enabled = false;
                }
            }
            else
            {
                ini12.INIWrite(MailPath, "Send Mail", "value", "0");
                SendMailcheckBox.Checked = false;
                FromTextBox.Enabled = false;
                ToTextBox.Enabled = false;
                Testcase1TextBox.Enabled = false;
                Testcase2TextBox.Enabled = false;
                Testcase3TextBox.Enabled = false;
                Testcase4TextBox.Enabled = false;
                Testcase5TextBox.Enabled = false;
                ProjectNumberTextBox.Enabled = false;
                ProjectTextBox.Enabled = false;
                ModelTextBox.Enabled = false;
                VersionTextBox.Enabled = false;
                TesterTextBox.Enabled = false;
                TotalTextBox.Enabled = false;
            }

            FromTextBox.Text = ini12.INIRead(MailPath, "Mail Info", "From", "");
            ToTextBox.Text = ini12.INIRead(MailPath, "Mail Info", "To", "");

            Testcase1TextBox.Text = ini12.INIRead(MailPath, "Test Case", "TestCase1", "");
            Testcase2TextBox.Text = ini12.INIRead(MailPath, "Test Case", "TestCase2", "");
            Testcase3TextBox.Text = ini12.INIRead(MailPath, "Test Case", "TestCase3", "");
            Testcase4TextBox.Text = ini12.INIRead(MailPath, "Test Case", "TestCase4", "");
            Testcase5TextBox.Text = ini12.INIRead(MailPath, "Test Case", "TestCase5", "");

            ProjectNumberTextBox.Text = ini12.INIRead(MailPath, "Data Info", "ProjectNumber", "");
            ProjectTextBox.Text = ini12.INIRead(MailPath, "Mail Info", "ProjectName", "");
            ModelTextBox.Text = ini12.INIRead(MailPath, "Mail Info", "ModelName", "");
            VersionTextBox.Text = ini12.INIRead(MailPath, "Mail Info", "Version", "");

            TesterTextBox.Text = ini12.INIRead(MailPath, "Mail Info", "Tester", "");
            TotalTextBox.Text = ini12.INIRead(MailPath, "Mail Info", "Total Test Time", "");
        }

        private void SendMailcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SendMailcheckBox.Checked == true)
            {
                ini12.INIWrite(MailPath, "Send Mail", "value", "1");
                FromTextBox.Enabled = true;
                ToTextBox.Enabled = true;
                Testcase1TextBox.Enabled = true;
                Testcase2TextBox.Enabled = true;
                Testcase3TextBox.Enabled = true;
                Testcase4TextBox.Enabled = true;
                Testcase5TextBox.Enabled = true;
                ProjectNumberTextBox.Enabled = true;
                ProjectTextBox.Enabled = true;
                ModelTextBox.Enabled = true;
                VersionTextBox.Enabled = true;
                TesterTextBox.Enabled = true;
                TotalTextBox.Enabled = true;
            }
            else
            {
                ini12.INIWrite(MailPath, "Send Mail", "value", "0");
                FromTextBox.Enabled = false;
                ToTextBox.Enabled = false;
                Testcase1TextBox.Enabled = false;
                Testcase2TextBox.Enabled = false;
                Testcase3TextBox.Enabled = false;
                Testcase4TextBox.Enabled = false;
                Testcase5TextBox.Enabled = false;
                ProjectNumberTextBox.Enabled = false;
                ProjectTextBox.Enabled = false;
                ModelTextBox.Enabled = false;
                VersionTextBox.Enabled = false;
                TesterTextBox.Enabled = false;
                TotalTextBox.Enabled = false;
            }
        }
    }
}
