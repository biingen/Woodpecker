using System;
using System.Windows.Forms;
using System.Threading;
using SplashDemo;

namespace AutoTest
{
    static class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (System.Environment.OSVersion.Version.Major >= 6) { SetProcessDPIAware(); }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Thread to show splash window
            Thread thUI = new Thread(new ThreadStart(ShowSplashWindow));
            thUI.Name = "Splash UI";
            thUI.Priority = ThreadPriority.Highest;
            thUI.IsBackground = true;
            thUI.Start();

            //Thread to load time-consuming resources.
            Thread th = new Thread(new ThreadStart(LoadResources));
            th.Name = "Resource Loader";
            th.Priority = ThreadPriority.Normal;
            th.Start();
            th.Join();
            
            if (SplashForm != null)
            {
                SplashForm.Invoke(new MethodInvoker(delegate { SplashForm.Close(); }));
            }
            thUI.Join();
            Application.Run(new Form1());
        }

        public static frm_Splash SplashForm
        {
            get;
            set;
        }

        private static void LoadResources()
        {
            for (int i = 1; i <= 10; i++)
            {
                if (SplashForm != null)
                {SplashForm.Invoke(new MethodInvoker(delegate 
                        {SplashForm.labelMark.Text = "AutoTest";}));}
                Thread.Sleep(100);
            }

            Add_ons Add_ons = new Add_ons();//讀取相機的PidVid
            Add_ons.ReadVidPid();
            //Add_ons.CreateExcelFile();
        }

        private static void ShowSplashWindow()
        {
            SplashForm = new frm_Splash();
            Application.Run(SplashForm);
        }
    }
}
