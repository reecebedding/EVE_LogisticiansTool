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
    public partial class ContractView : UserControl
    {
        public Contract _activeContract;

        private DataRepository _repository;
        public event PropertyChangedEventHandler _propertyChanged;

        public ContractView(Contract contract)
        {        
            InitializeComponent();

            _activeContract = contract;
            _repository = new DataRepository();

            PopulateFields(contract);
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            //This is the click event that will raise an event, for the parent to deal with in order to view the contracts details / route etc..
            picContract.Click += (object o, EventArgs e) => 
            {
                if (_propertyChanged != null)
                    _propertyChanged(this, new PropertyChangedEventArgs("pic"));
            };
        }

        private void PopulateFields(Contract contract)
        {
            //Populate all the onscreen controls with the relevant information of the contract
            lblLocation.Text = contract.StartSystem.SolarSystemDisplayName;
            lblDestination.Text = contract.EndSystem.SolarSystemDisplayName;
            lblStatus.Text = contract.Status;
            lblVolume.Text = contract.Volume.ToString("N") + " m3";
            lblExpires.Text = contract.Expiration.ToString();
            lblReward.Text = contract.Reward.ToString("N") + " ISK";
            lblCollateral.Text = contract.Collateral.ToString("N") + " ISK";
        }
    }
}
