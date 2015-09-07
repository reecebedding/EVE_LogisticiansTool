using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace LogisticiansTool
{
    public partial class MainForm : Form
    {
        private OverrideCacheManager CacheManager;
        private DataRepository _repository;
        private NLog.Logger _logger;

        public MainForm()
        {
            _repository = new DataRepository();
            _logger = NLog.LogManager.GetCurrentClassLogger();

            CacheManager = new OverrideCacheManager();

            InitializeComponent();
            InitializeEventHandlers();


            InitializeCacheManagement();

            this.CenterToScreen();

            tabControl.Controls.Add(new TabPage() { BackColor = Color.Gainsboro, Text = "Contracts", Name = "tabContractOverview" });
            ContractOverview contractViewScreen = new ContractOverview();
            contractViewScreen._viewContractEvent += (sender, e) => RespondToContractViewRequest(sender, e);
            tabControl.Controls["tabContractOverview"].Controls.Add(contractViewScreen);
            tabControl.Controls["tabContractOverview"].Controls["ContractOverview"].Dock = DockStyle.Fill;
            tabControl.Controls.Add(new TabPage() { BackColor = Color.Gainsboro, Text = "Route Planner", Name = "tabRoutePlanner" });
            tabControl.Controls["tabRoutePlanner"].Controls.Add(new RoutePlanner());
            tabControl.Controls["tabRoutePlanner"].Controls["RoutePlanner"].Dock = DockStyle.Fill;
        }

        private void RespondToContractViewRequest(object o, EventArgs e)
        {
            tabControl.Controls["tabRoutePlanner"].Controls.Clear();

            tabControl.Controls["tabRoutePlanner"].Controls.Add(new RoutePlanner(((ContractView)o)._activeContract));
            //tabControl.Controls["tabRoutePlanner"].Controls["RoutePlanner"].Dock = DockStyle.Fill;

            tabControl.SelectedTab = (TabPage)tabControl.Controls["tabRoutePlanner"];
        }

        private void InitializeCacheManagement()
        {
            CacheManager.RefreshItemPriceCache();
            Timer refreshTimer = new Timer();
            refreshTimer.Interval = 60000;
            refreshTimer.Tick += (object o, EventArgs e) => { CacheManager.RefreshItemPriceCache(); };
            refreshTimer.Start();
        }



        private void InitializeEventHandlers()
        {
            manageAPIToolStripMenuItem.Click += (object o, EventArgs e) => { 
                ManageAPI newManageAPIForm = new ManageAPI();                 
                newManageAPIForm.ShowDialog();
                ContractOverview contractScreen = (ContractOverview)tabControl.Controls["tabContractOverview"].Controls["ContractOverview"];
                contractScreen.PopulateAPIList();                
            };

            resetSavedRoutesMenuItem.Click += (object o, EventArgs e) =>
            {
                if (MessageBox.Show("Are you sure you want to reset all saved routes?", "Reset Saved Routes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _repository.DeleteSavedRoutesFile();
                }
            };

            resetSavedAPIKeysMenuItem.Click += (object o, EventArgs e) =>
            {
                if (MessageBox.Show("Are you sure you want to reset all saved API key's?", "Reset Saved API's", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        _repository.DeleteSavedAPIsFile();
                        _logger.Info("Saved APIs file was deleted.");
                    }
                    catch (Exception exn)
                    {
                        _logger.Error("Failed to delete saved API's. Error: " + exn.Message);
                    }    
                }
            };

            errorLogMenuItem.Click += (object o, EventArgs e) => 
            {
                if (File.Exists("logs/Log.log"))
                {
                    System.Diagnostics.Process.Start("notepad.exe", "logs/Log.log");
                }
                else
                    MessageBox.Show("No Log File Found");
                
            };

            viewChangeLogMenuItem.Click += (object o, EventArgs e) =>
            {
                if (File.Exists("Resources/ChangeLog.txt"))
                {
                    System.Diagnostics.Process.Start("notepad.exe", "Resources/ChangeLog.txt");
                }
                else
                    MessageBox.Show("No Change File Found");
            };
            

            clearCacheMenuItem.Click += (object o, EventArgs e) => 
            {
                if (MessageBox.Show("Are you sure you want to clear the cache?. Data will be removed.", "Clear Cache", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        CacheManager.Cache.Flush();
                        _logger.Info("Cache was successfully flushed");
                    }
                    catch (Exception exn)
                    {
                        _logger.Error("Failed to clear cache. Error: " + exn.Message);
                    }
                    
                    CacheManager.RefreshItemPriceCache();
                }
            };

            exitToolStripMenuItem.Click += (object o, EventArgs e) => {
                this.Close();
            };

            aboutToolStripMenuItem.Click += (object o, EventArgs e) => {
                About aboutScreen = new About();
                aboutScreen.Show();
            };
        }        
    }
}
