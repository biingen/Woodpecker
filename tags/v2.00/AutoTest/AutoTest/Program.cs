using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using SplashDemo;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;

namespace AutoTest
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Thread to show splash window
            Thread thUI = new Thread(new ThreadStart(ShowSplashWindow));
            thUI.Name = "Splash UI";
            thUI.Priority = ThreadPriority.Normal;
            thUI.IsBackground = true;
            thUI.Start();

            //Thread to load time-consuming resources.
            Thread th = new Thread(new ThreadStart(LoadResources));
            th.Name = "Resource Loader";
            th.Priority = ThreadPriority.Highest;
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
            for (int i = 1; i <= 25; i++)
            {
                if (SplashForm != null)
                {SplashForm.Invoke(new MethodInvoker(delegate 
                        {SplashForm.labelMark.Text = "AutoTest";
                         SplashForm.labelLoading.Text = "Loading.......";}));}
                Thread.Sleep(100);
            }
        }

        private static void ShowSplashWindow()
        {
            SplashForm = new frm_Splash();
            Application.Run(SplashForm);
        }
    }
}
