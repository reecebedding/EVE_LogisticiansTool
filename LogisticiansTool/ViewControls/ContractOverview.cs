using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogisticiansTool
{
    public partial class ContractOverview : UserControl
    {
        private DataRepository _dataRepository;
        private OverrideCacheManager _cacheManager;
        private NLog.Logger _logger;

        public event PropertyChangedEventHandler _viewContractEvent;

        public ContractOverview()
        {
            InitializeComponent();

            _dataRepository = new DataRepository();
            _cacheManager = new OverrideCacheManager();
            _logger = NLog.LogManager.GetCurrentClassLogger();

            cmbStatus.SelectedIndex = 0;
            object cacheItem = _cacheManager.Cache.GetData("ContractStatus");
            if (cacheItem != null)
                cmbStatus.Text = cacheItem.ToString();

            InitializeHandlers();
            PopulateAPIList();            
        }

        public void PopulateAPIList()
        {
            cmbApiList.Items.Clear();
            IEnumerable<APIKey> apis = new List<APIKey>();
            //Gets all the APIS, and if they are valid, display them. We dont want to show the user keys that have gone invalid.
            try
            {
                apis = _dataRepository.GetAllAPIKeys();
            }
            catch (SystemException exn)
            {
                if (exn is System.Web.HttpException)
                    MessageBox.Show("Unable to contact EVE API server. Please try again later.");
                else
                    _logger.Error("Unable to retrieve API key data. Error: " + exn.Message);
            }
            
            if (apis.Count() > 0)
            {
                cmbApiList.Text = "";
                cmbApiList.Items.AddRange(apis.Where(x => x.IsValid == true).ToArray());
            }
            else
                cmbApiList.Text = "No API Keys";
            
        }

        private void InitializeHandlers()
        {
            cmbApiList.SelectedIndexChanged += (object o, EventArgs e) => { PopulateContracts((APIKey)cmbApiList.SelectedItem); };
            cmbStatus.SelectedIndexChanged += (object o, EventArgs e) => 
            {                 
                _cacheManager.Cache.Add("ContractStatus", cmbStatus.Text.Trim());
                _logger.Info("Contract Status was added to the cache with value: " + cmbStatus.Text.Trim());
            };
            cmbStatus.SelectedIndexChanged += (object o, EventArgs e) => { PopulateContracts((APIKey)cmbApiList.SelectedItem); };
        }

        private void RespondToContract(object o, EventArgs e)
        {
            //This is raised when the contract control reports back that it was clicked.
            if (_viewContractEvent != null)
                //Re-Raise the event under this event so that the parent listening to this, can respond and show the correct page.
                _viewContractEvent(o, new PropertyChangedEventArgs(e.ToString()));
        }

        private void PopulateContracts(APIKey key)
        {
            pnlContractCont.Controls.Clear();
            try
            {
                IEnumerable<Contract> contracts = new List<Contract>();

                if (key != null)
                {
                    //Get all available contracts on the key. So if they are outstanding or in-progress
                    if (cmbStatus.Text.Trim() == "All")
                        contracts = _dataRepository.GetAvailableContracts(key);
                    else
                        contracts = _dataRepository.GetAvailableContractsByStatus(key, cmbStatus.Text.Trim());
                }
                

                if (contracts.Count() == 0)
                {
                    Label noContracts = new Label();
                    noContracts.Text = "No Contracts";
                    //Center the label on the panel
                    noContracts.Location = new Point((pnlContractCont.Width / 2 ) - (noContracts.Width / 2), 20);                    
                    pnlContractCont.Controls.Add(noContracts);
                }
                else
                {
                    foreach (Contract contract in contracts.OrderBy(x => x.Expiration).ToList())
                    {
                        //Create a new contract view control for each contract. _PropertyChanged event is raised when the control is clicked / chosen
                        ContractView newContract = new ContractView(contract);
                        
                        newContract.Width = pnlContractCont.Width;
                        newContract._propertyChanged += (sender, e) => RespondToContract(sender, e);
                        pnlContractCont.Controls.Add(newContract);

                        //Put the controls location directly under the last one.
                        pnlContractCont.Controls[pnlContractCont.Controls.Count - 1].Location = new Point(0, ((pnlContractCont.Controls.Count - 1) * newContract.Size.Height) + 10);
                    }
                }                
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to retrieve contracts for this API. Please try again");
            }
            
        }

    }
}
