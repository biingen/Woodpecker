using System;
using System.Drawing;
using System.Windows.Forms;

namespace Woodpecker
{
    public partial class frm_Splash : Form
    {
        public frm_Splash()
        {
            InitializeComponent();
        }

        private void frm_Splash_Load(object sender, EventArgs e)
        {
            Graphics graphics = CreateGraphics();
            float dpiX = graphics.DpiX;
            float dpiY = graphics.DpiY;
            /*if (dpiX == 96 && dpiY == 96)
            {
                this.Height = 600;
                this.Width = 1120;
            }*/
            int intPercent = (dpiX == 96) ? 100 : (dpiX == 120) ? 125 : 150;

            // 針對字體變更Form的大小
            this.Height = this.Height * intPercent / 100;

            pictureBox1.Image = Properties.Resources.Loading_WPK;
        }
    }
}
