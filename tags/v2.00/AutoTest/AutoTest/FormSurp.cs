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
    public partial class FormSurp : Form
    {
        public FormSurp()
        {
            InitializeComponent();

        }
/*
        private void FormSurp_MouseEnter(object sender, EventArgs e)
        {
            this.Close();
        }
*/
        private void Setting_Load(object sender, EventArgs e)
        {
            //表單戴入時
            // 設定INI檔路徑
            String sPath;
            sPath = Application.StartupPath + "\\Config.ini";

            if (int.Parse(ini12.INIRead(sPath, "ImportDB", "value", "")) == 1)
            {
                ImportDB.Checked = true;
            }
            else
            {
                ImportDB.Checked = false;
            }
        }

        private void ImportDB_CheckedChanged(object sender, EventArgs e)
        {
            if (ImportDB.Checked == true)
            {
                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                // 寫入Import DadaBase啟動
                ini12.INIWrite(sPath, "ImportDB", "value", "1");
            }
            else
            {
                // 設定INI檔路徑
                String sPath;
                sPath = Application.StartupPath + "\\Config.ini";

                // 寫入Import DadaBase關閉
                ini12.INIWrite(sPath, "ImportDB", "value", "0");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
