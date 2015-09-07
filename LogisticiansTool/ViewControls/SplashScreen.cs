using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogisticiansTool
{
    public partial class SplashScreen : Form
    {
        Timer loadingTimer = new Timer();
        int elipseCount = 0;

        public SplashScreen()
        {
            InitializeComponent();

            this.CenterToScreen();

            loadingTimer.Tick += (sender, eventArgs) => ChangeLoadingText();
            loadingTimer.Interval = 250;
            loadingTimer.Start();
        }

        private void ChangeLoadingText()
        {
            elipseCount++;
            if (elipseCount > 3)
            {
                lblLoading.Text = "Loading";
                elipseCount = 0;
            }
            else if(elipseCount == 1)
            {
                lblLoading.Text = "Loading .";
            }
            else if (elipseCount == 2)
            {
                lblLoading.Text = "Loading ..";
            }
            else
            {
                lblLoading.Text = "Loading ...";
            }


        }
    }
}
