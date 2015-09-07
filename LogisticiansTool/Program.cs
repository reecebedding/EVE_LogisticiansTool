using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LogisticiansTool
{
    static class Program
    {
        private static SplashScreen splashScreen = null;
        private static bool LoadSplash = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (LoadSplash)
                LoadWithSplash();
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }

        private static void LoadWithSplash()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            System.Threading.Thread splashThread = new System.Threading.Thread(new System.Threading.ThreadStart(
            delegate
            {
                splashScreen = new SplashScreen();
                splashScreen.TopMost = false;
                Application.Run(splashScreen);
            }));

            splashThread.SetApartmentState(System.Threading.ApartmentState.STA);
            splashThread.Start();

            MainForm mainApplication = new MainForm();
            mainApplication.Load += new EventHandler(mainForm_Load);
            Application.Run(mainApplication);
        }

        private static void mainForm_Load(object sender, EventArgs e)
        {
            if (splashScreen == null)
            {
                return;
            }
            splashScreen.Invoke(new Action(splashScreen.Close));
            splashScreen.Dispose();
            splashScreen = null;
        }
    }
}
