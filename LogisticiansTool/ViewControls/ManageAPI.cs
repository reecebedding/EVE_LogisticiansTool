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
    public partial class ManageAPI : Form
    {
        private DataRepository _repository;
        private NLog.Logger _logger;

        public ManageAPI()
        {
            InitializeComponent();

            this.CenterToParent();

            _repository = new DataRepository();
            _logger = NLog.LogManager.GetCurrentClassLogger();

            InitializeEventHandlers();
            PopulateGrid();
        }

        private void InitializeEventHandlers()
        {
            btnDelete.Click += (object o, EventArgs e) =>
            {
                //Gets the selected api key, based on the row that was selected
                APIKey key = ((List<APIKey>)dtgAPIView.DataSource)[dtgAPIView.CurrentCell.RowIndex];
                if ((MessageBox.Show("Are you sure you want to delete this API?", "Delete API", MessageBoxButtons.YesNo) == DialogResult.Yes))
                {
                    //Delete the api, and re-draw the grid to show the changes
                    try
                    {
                        _repository.DeleteAPIKey(key);
                    }
                    catch (Exception exn)
                    {
                        _logger.Error(string.Format("Unable to delete api key KeyID: {0}, vCode: {1}. Exception: {2}", key.KeyID, key.VCode, exn.Message));
                    }
                    
                    PopulateGrid();
                }

            };

            btnAdd.Click += (object o, EventArgs e) => 
            { 
                //Open the newAPI form in dialog mode. When closed either an api was added or not. Re-Draw the grid incase there was a change.
                AddAPI newAddAPI = new AddAPI();
                newAddAPI.ShowDialog();
                
                PopulateGrid();
            };
        }

        private void PopulateGrid()
        {
            //Bind the list of API's to the grid
            try
            {
                dtgAPIView.DataSource = _repository.GetAllAPIKeys();
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("Unable to retrieve all API keys for displaying. Exception: {0}", exn.Message));
            }
            
        }
    }
}
