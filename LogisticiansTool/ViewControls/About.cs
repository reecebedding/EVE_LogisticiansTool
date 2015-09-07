using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;

namespace LogisticiansTool
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            InitializeEventHandlers();   
            InitializeValues();

            this.CenterToParent();
        }

        private void InitializeEventHandlers()
        {
            lnkLblDownload.Click += (object o, EventArgs e) => { System.Diagnostics.Process.Start(lnkLblDownload.Text); };
            lnkLblAuthor.Click += (object o, EventArgs e) => { System.Diagnostics.Process.Start(HttpUtility.HtmlEncode(string.Format("{0}/{1}", "https://gate.eveonline.com/Profile/",lnkLblAuthor.Text))); };            
        }

        private void InitializeValues()
        {
            lblName.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString();
            lblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();     
        }        
    }
}
