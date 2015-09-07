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
    public partial class AddAPI : Form
    {
        private DataRepository _repository;
        private NLog.Logger _logger;

        public AddAPI()
        {
            _repository = new DataRepository();
            _logger = NLog.LogManager.GetCurrentClassLogger();

            InitializeComponent();
            InitializeEventHandlers();

            this.CenterToParent();
        }

        public void InitializeEventHandlers()
        {
            btnAdd.Click += (object o, EventArgs e) => 
            {
                if (VerifyValues())
                {
                    //Creates base API from KeyID and vCode
                    APIKey newAPI = new APIKey()
                    {
                        KeyID = Convert.ToInt32(txtKeyID.Text),
                        VCode = txtVCode.Text                        
                    };
                    //Looks up the keys type (Corp or Char) 
                    try
                    {
                        newAPI.Type = _repository.GetAPIType(newAPI);
                        //This will either come back with 1 "Corp" value, or up to 3 for a "Char" key
                        foreach (KeyValuePair<string, string> charOrCorp in _repository.GetCharsOrCorpOnAPI(newAPI))
                        {
                            //This is the same API, but for a different toon. So use the same key, just change name and id.
                            newAPI.CharacterName = charOrCorp.Key;
                            newAPI.CharacterID = Convert.ToInt32(charOrCorp.Value);
                            try
                            {
                                _repository.AddAPIKey(newAPI);
                                _logger.Info("New API Added. KeyID: " + newAPI.KeyID  + "vCode: " + newAPI.VCode);
                            }
                            catch (Exception exn)
                            {
                                _logger.Error(string.Format("Failed to add a new API Key. KeyID: {0}, vCode: {1}. Error: {2}", newAPI.KeyID, newAPI.VCode, exn.Message));
                            }
                            
                            this.Close();
                        }         
                    }
                    catch (Exception exn)
                    {
                        _logger.Error(string.Format("Unable to retrieve API type KeyID: {0}, vCode: {1}. Exception: {2}", newAPI.KeyID, newAPI.VCode, exn.Message));
                        apiError.SetError(btnAdd, "Incorrect API Combination, or API server is offline.");
                    }     
                }
            };
        }

        private bool VerifyValues()
        {
            apiError.Clear();
            bool validValues = true;

            if (txtVCode.Text.Trim().Length == 0)
            {
                validValues = false;
                apiError.SetError(txtVCode, "VCode must not be empty");
            }

            if (txtKeyID.Text.Trim().Length == 0)
            {
                validValues = false;
                apiError.SetError(txtKeyID, "KeyID must not be empty");
            }

            int keyID;
            if (!Int32.TryParse(txtKeyID.Text, out keyID))
            {
                validValues = false;
                apiError.SetError(txtKeyID, "KeyID must be a number");
            }

            //If the data they entered is ok, check if it is a valid API Key
            if (validValues)
            {
                bool validCombo = false;
                try
                {
                    //Attempt to read and determin if its a standard key.
                    validCombo = _repository.IsAPIValid(new APIKey() { KeyID = keyID, VCode = txtVCode.Text.Trim() });                    
                    if (!validCombo)
                    {
                        validValues = false;
                        apiError.SetError(btnAdd, "Invalid API. Please make sure it allows contracts");
                    }
                }
                catch (Exception exn)
                {
                    validValues = false;
                    apiError.SetError(btnAdd, "Incorrect API Combination, or API server is offline.");
                    _logger.Error(string.Format("Unable to retrieve API information for KeyID: {1}, vCode: {2}. Exception: {4}", keyID, txtVCode.Text.Trim(), exn.Message));
                }
            }
            return validValues;
        }
    }
}
