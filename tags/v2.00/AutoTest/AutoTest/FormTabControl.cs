using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AutoTest
{
    public partial class FormTabControl : Form
    {
        public FormTabControl()
        {
            InitializeComponent();
        }

        private const int CS_DROPSHADOW = 0x20000;      //宣告陰影參數
        
        [DllImport("user32.dll")]       //拖動無窗體的控件>>>>>>>>>>>>>>
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;        //<<<<<<<<<<<<<<<<<<<<<<<

        private void Add_TabPage(string str, Form myForm)        //載入Form到tab
        {
            if (tabControlCheckHave(this.tabControl, str))
            {
                return;
            }
            else
            {
                tabControl.TabPages.Add(str);
                tabControl.SelectTab(tabControl.TabPages.Count - 1);

                myForm.FormBorderStyle = FormBorderStyle.None;
                myForm.Dock = DockStyle.Fill;
                myForm.TopLevel = false;
                myForm.Show();
                myForm.Parent = tabControl.SelectedTab;
            }
        }

        private bool tabControlCheckHave(System.Windows.Forms.TabControl tab, String tabName)        //設定tab索引編號
        {
            for (int i = 0; i < tab.TabCount; i++)
            {
                if (tab.TabPages[i].Text == tabName)
                {
                    tab.SelectedIndex = i;
                    return true;
                }
            }
            return false;
        }

        private void MainSettingBtn_Click(object sender, EventArgs e)
        {
            Setting FormSetting = new Setting();
            //this.ClosePicBox.Click += new System.EventHandler(FormSetting.OkBtn_Click);

            MainSettingBtn.Enabled = false;
            FormSetting.Dock = DockStyle.Fill;
            Add_TabPage("Main Setting", FormSetting);
        }

        private void ScheduleBtn_Click(object sender, EventArgs e)
        {
            FormSchedule FormSchedule = new FormSchedule();
            //this.ClosePicBox.Click += new System.EventHandler(FormSchedule.SaveSchBtn_Click);

            ScheduleSettingBtn.Enabled = false;
            FormSchedule.Dock = DockStyle.Fill;
            Add_TabPage("Multi Schedule Setting", FormSchedule);
        }

        private void MailSettingBtn_Click(object sender, EventArgs e)
        {
            FormMail FormMail = new FormMail();
            //this.ClosePicBox.Click += new System.EventHandler(FormMail.SaveSchBtn_Click);
            

            MailSettingBtn.Enabled = false;
            FormMail.Dock = DockStyle.Fill;
            Add_TabPage("Mail Setting", FormMail);
        }

        #region 陰影
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!DesignMode)
                {
                    cp.ClassStyle |= CS_DROPSHADOW;
                }
                return cp;
            }
        }
        #endregion
        
        #region 滑鼠拖曳視窗
        private void gPanelTitleBack_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);//*********************調用移動無窗體控件函數
        }
        #endregion
        
        #region 關閉按鈕
        private void ClosePicBox_Click(object sender, EventArgs e)
        {
            if (Global.FormSetting == true && Global.FormSchedule == true && Global.FormMail == true)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                if(Global.FormSetting == false)
                    MessageBox.Show("Main Setting Error !");

                if (Global.FormSchedule == false)
                    MessageBox.Show("Schedule Setting Error !");

                if (Global.FormMail == false)
                    MessageBox.Show("Mail Setting Error !");
            }
        }
        #endregion
    }
}
