using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.EnterpriseLibrary.Caching;

namespace LogisticiansTool
{
    public partial class RoutePlanner : UserControl
    {
        #region GlobalVariables

        private DataRepository _dataRepository;
        private OverrideCacheManager _cacheManager;
        private NLog.Logger _logger;
        
        //The contract that the view is displaying (If not a blank view, without contract)
        private Contract _activeContract;
        //Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager _cacheManager = CacheFactory.GetCacheManager();

        //All of these are relevant to the search field.
        Ship _chosenShip;
        int _shipQuant = 1;
        int _JDC = 0;
        int _JFC = 0;
        int _JF = 0;
        int _F = 4;

        SolarSystem _startLoc;
        SolarSystem _endDesto;

        #endregion

        //If no contract was parsed, chain it with a new contract
        public RoutePlanner() : this(new Contract()) { }

        public RoutePlanner(Contract contract)
        {
            InitializeComponent();

            _activeContract = contract;
            _dataRepository = new DataRepository();
            _cacheManager = new OverrideCacheManager();
            _logger = NLog.LogManager.GetCurrentClassLogger();

            InitalizeHandlers();
            PopulateSearchForm();
        }


        public void InitalizeHandlers()
        {

            btnAddMidpoint.Click += (object o, EventArgs e) => { AddMidpoint(); };

            btnReset.Click += (object o, EventArgs e) => 
            {
                pnlMidpoints.Controls.Clear(); 
            };

            btnDeletePreset.Click += (object o, EventArgs e) =>
            {
                string routeName = cmbPresets.Text;
                int ind = cmbPresets.FindStringExact(routeName);
                if (ind > 0)
                {
                    if (MessageBox.Show("Are you sure you wish the delete the preset : " + routeName + "?", "Delete Saved Route", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            _dataRepository.DeleteSavedRoute(routeName);
                            _logger.Info("Saved route: \"" + routeName + "\" was successfully deleted");
                        }
                        catch (Exception exn)
                        {
                            _logger.Error("Failed to delete saved route. Error: " + exn.Message);
                        }
                        
                        SetPresets();
                    }
                }
                else
                    MessageBox.Show("No saved route found with the name of : " + routeName + ".", "Delete Saved Route");
            };

            btnAddPreset.Click += (object o, EventArgs e) => 
            {
                if (cmbPresets.Text != "" && cmbPresets.Text != "New Route")
                {
                    if (RunValidation())
                    {
                        List<SolarSystem> chosenRoute = new List<SolarSystem>();
                        chosenRoute.Add(_startLoc);
                        List<SolarSystem> chosenSubRoute = new List<SolarSystem>();
                        foreach (Control cont in pnlMidpoints.Controls)
                        {
                            if (cont.GetType() == typeof(ComboBox))
                            {
                                ComboBox curCombo = (ComboBox)cont;
                                if (curCombo.SelectedItem != null)
                                {
                                    chosenSubRoute.Add((SolarSystem)curCombo.SelectedItem);
                                }
                            }
                        }
                        //chosenSubRoute.Reverse();
                        chosenRoute.AddRange(chosenSubRoute);
                        chosenRoute.Add(_endDesto);
                        if (cmbPresets.FindStringExact(cmbPresets.Text.Trim()) > 0)
                        {
                            if (MessageBox.Show("There is allready a preset with this name. Do you wish to overwrite it?", "Save Saved Route", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                try
                                {
                                    _dataRepository.DeleteSavedRoute(cmbPresets.Text.Trim());
                                    _logger.Info("Saved route: \"" + cmbPresets.Text.Trim() + "\" was successfully deleted");
                                    _dataRepository.AddSavedRoute(cmbPresets.Text.Trim(), chosenRoute);
                                    _logger.Info("Saved route: \"" + cmbPresets.Text.Trim() + "\" was successfully added");                                    
                                }
                                catch (Exception exn)
                                {
                                    _logger.Error("Failed to add new route : " + cmbPresets.Text.Trim() + ". Error: " + exn.Message);
                                }
                                
                                MessageBox.Show("Route Saved");
                            }
                        }
                        else
                            _dataRepository.AddSavedRoute(cmbPresets.Text.Trim(), chosenRoute);

                        SetPresets();
                    }
                }
                else
                    MessageBox.Show("Must enter a preset name");
            };
            btnDeletePreset.Click += (object o, EventArgs e) => { };

            //When the skill drop down changes, store the new value in the global variable, remove it from the cache and re-add the new value. (THIS APPLIES TO ALL SKILLS, SHIP TYPE AND SHIP QUANTITY)
            cmbShipType.SelectedIndexChanged += (object o, EventArgs e) =>
            { 
                _chosenShip = ((Ship)cmbShipType.SelectedItem);


                switch (_chosenShip.ItemID)
                {
                    case 28846:
                        picShip.Image = Properties.Resources._28846;
                        break;
                    case 28850:
                        picShip.Image = Properties.Resources._28850;
                        break;
                    case 28851:
                        picShip.Image = Properties.Resources._28851;
                        break;
                    case 28852:
                        picShip.Image = Properties.Resources._28852;
                        break;
                }


                _cacheManager.Cache.Remove("Search_Ship_Type");
                _logger.Info("Cache item: \"Search Ship Type\" was successfully removed");
                _cacheManager.Cache.Add("Search_Ship_Type", _chosenShip);
                _logger.Info("Value was added to Cache, Key: Search Ship Type, Value: " + _chosenShip);
            };
            numJDC.ValueChanged += (object o, EventArgs e) => 
            { 
                _JDC = Convert.ToInt32(numJDC.Value);
                _cacheManager.Cache.Remove("JDC_Skill_Level");
                _cacheManager.Cache.Add("JDC_Skill_Level", _JDC);
            };
            numJFC.ValueChanged += (object o, EventArgs e) => 
            { 
                _JFC = Convert.ToInt32(numJFC.Value);
                _cacheManager.Cache.Remove("JFC_Skill_Level");
                _cacheManager.Cache.Add("JFC_Skill_Level", _JFC);
            };
            numJF.ValueChanged += (object o, EventArgs e) =>
            {
                _JF = Convert.ToInt32(numJF.Value);
                _cacheManager.Cache.Remove("JF_Skill_Level");
                _cacheManager.Cache.Add("JF_Skill_Level", _JF);
            };
            numF.ValueChanged += (object o, EventArgs e) => 
            {
                _F = Convert.ToInt32(numF.Value);
                _cacheManager.Cache.Remove("F_Skill_Level");
                _cacheManager.Cache.Add("F_Skill_Level", _F);
            };
            numShipQuant.ValueChanged += (object o, EventArgs e) => 
            { 
                _shipQuant = Convert.ToInt32(numShipQuant.Value);
                _cacheManager.Cache.Remove("Search_Ship_Quant");
                _cacheManager.Cache.Add("Search_Ship_Quant", _shipQuant);
            };

            
            cmbStartLoc.TextUpdate += (object o, EventArgs e) =>
            {
                cmbStartLoc.Items.Clear();
                //Get the containers current sizes
                Size contSize = cmbStartLoc.Size;
                string userText = cmbStartLoc.Text;
                //If the user has entered 3 or more chars, lets do a lookup. If its less... Ignore it, we dont want to search this vague
                if (userText.Length >= 3)
                {
                    //Get all systems that are similar to the entered values
                    IEnumerable<SolarSystem> systems = _dataRepository.GetSolarSystemsLikeName(userText);
                    if (systems.Count() > 0)
                    {
                        cmbStartLoc.Items.AddRange(systems.ToArray());

                        //New items were added, so we want to display the drop down so they can select one. They MUST select one.
                        cmbStartLoc.DropDownStyle = ComboBoxStyle.Simple;
                        //Height of the drop down is either 121 if the list height exceedes 121, with a height of 21 for each item. If less, the drop down is equal to minimum height for the items.
                        int height = ((systems.Count() * 21) > 100) ? 121 : ((systems.Count() * 21) + 21);
                        //Re-size the combo back to standard if less than 3 values entered. This is because they might exceed 3, then backspace.
                        cmbStartLoc.Size = new Size(contSize.Width, height);
                    }
                    else
                        //If there are no systems, hide the drop down (Even if its not shown)
                        cmbStartLoc.Size = new Size(contSize.Width, 21);
                }
                else
                {
                    //If there are no systems, hide the drop down (Even if its not shown)
                    cmbStartLoc.Size = new Size(contSize.Width, 21);
                }
                //Highlight the whole combobox, and put the cursor at the end
                cmbStartLoc.Select(cmbStartLoc.Text.Length, 0);
                //Bring the dropdown to the front so nothing obstructs it.
                cmbStartLoc.BringToFront();
            };

            cmbDesto.TextUpdate += (object o, EventArgs e) =>
            {
                cmbDesto.Items.Clear();
                //Get the containers current sizes
                Size contSize = cmbDesto.Size;
                string userText = cmbDesto.Text;
                //If the user has entered 3 or more chars, lets do a lookup. If its less... Ignore it, we dont want to search this vague
                if (userText.Length >= 3)
                {
                    //Get all systems that are similar to the entered values
                    IEnumerable<SolarSystem> systems = _dataRepository.GetSolarSystemsLikeName(userText);
                    if (systems.Count() > 0)
                    {
                        cmbDesto.Items.AddRange(systems.ToArray());

                        //New items were added, so we want to display the drop down so they can select one. They MUST select one.
                        cmbDesto.DropDownStyle = ComboBoxStyle.Simple;
                        //Height of the drop down is either 121 if the list height exceedes 121, with a height of 21 for each item. If less, the drop down is equal to minimum height for the items.
                        int height = ((systems.Count() * 21) > 100) ? 121 : ((systems.Count() * 21) + 21);
                        //Re-size the combo back to standard if less than 3 values entered. This is because they might exceed 3, then backspace.
                        cmbDesto.Size = new Size(contSize.Width, height);
                    }
                    else
                        //If there are no systems, hide the drop down (Even if its not shown)
                        cmbDesto.Size = new Size(contSize.Width, 21);
                }
                else
                {
                    //If there are no systems, hide the drop down (Even if its not shown)
                    cmbDesto.Size = new Size(contSize.Width, 21);
                }
                //Highlight the whole combobox, and put the cursor at the end
                cmbDesto.Select(cmbDesto.Text.Length, 0);
                //Bring the dropdown to the front so nothing obstructs it.
                cmbDesto.BringToFront();
            };

            //Set the start solar system object. This is valid as the dropdown values are SolarSystem objects
            cmbStartLoc.SelectedIndexChanged += (object o, EventArgs e) =>
            {
                _startLoc = ((SolarSystem)cmbStartLoc.SelectedItem);
                cmbStartLoc.Size = new Size(cmbStartLoc.Size.Width, 23);
            };

            //Set the end desto solar system object. This is valid as the dropdown values are SolarSystem objects
            cmbDesto.SelectedIndexChanged += (object o, EventArgs e) =>
            {
                _endDesto = ((SolarSystem)cmbDesto.SelectedItem);
                cmbDesto.Size = new Size(cmbDesto.Size.Width, 23);
            };

            btnCalc.Click += (object o, EventArgs e) => { Calculate(); };

            cmbPresets.SelectedIndexChanged += (object o, EventArgs e) => 
            {
                if (cmbPresets.SelectedItem != null)
                {
                    Route chosenRoute = (Route)cmbPresets.SelectedItem;
                    pnlMidpoints.Controls.Clear();
                    if (chosenRoute.SystemRoute != null && chosenRoute.SystemRoute.Count() > 0)
                    {
                        _startLoc = chosenRoute.SystemRoute[0];
                        cmbStartLoc.Text = chosenRoute.SystemRoute[0].SolarSystemName;
                        for (int i = 1; i < chosenRoute.SystemRoute.Count() - 1; i++)
                        {
                            AddMidpoint(chosenRoute.SystemRoute[i]);
                        }

                        if (chosenRoute.SystemRoute.Count() > 1)
                        {
                            _endDesto = chosenRoute.SystemRoute[chosenRoute.SystemRoute.Count() - 1];
                            cmbDesto.Text = chosenRoute.SystemRoute[chosenRoute.SystemRoute.Count() - 1].SolarSystemName;
                        }
                    }
                }
            };
        }

        private void AddMidpoint()
        {
            AddMidpoint(new SolarSystem());
        }

        private void AddMidpoint(SolarSystem systemToAdd)
        {
            //Get a count of all the ComboBox's in the panel
            int controlCount = 0;
            foreach (Control cont in pnlMidpoints.Controls)
                if (cont.GetType() == typeof(ComboBox))
                    controlCount++;

            //Configure the textbox to be similar to the start / end desto properties
            ComboBox newTextBox = new ComboBox();
            pnlMidpoints.Controls.Add(newTextBox);
            
            newTextBox.ValueMember = "SolarSystemID";
            newTextBox.DisplayMember = "SolarSystemName";
            newTextBox.DropDownStyle = ComboBoxStyle.Simple;
            newTextBox.Tag = controlCount + 1;

            if (systemToAdd.SolarSystemName != null)
            {
                //If the system parsed is a legit system, set the combo box's text and selected item to the parsed system
                newTextBox.Text = systemToAdd.SolarSystemName;
                newTextBox.Items.Add(systemToAdd);
                newTextBox.SelectedItem = systemToAdd;
            }
            
            //We dont store each individual solar system, so when they select an item from the dropdown, just close it backup.
            newTextBox.SelectedIndexChanged += (object o, EventArgs e) =>
            {
                newTextBox.Size = new Size(newTextBox.Size.Width, 23);
            };

            //When the textbox text is updated, behave EXACTLY like the static start / desto boxes
            newTextBox.TextUpdate += (object o, EventArgs e) =>
            {
                newTextBox.Items.Clear();
                //Get the containers current sizes
                Size contSize = newTextBox.Size;
                string userText = newTextBox.Text;
                //If the user has entered 3 or more chars, lets do a lookup. If its less... Ignore it, we dont want to search this vague
                if (userText.Length >= 3)
                {
                    //Get all systems that are similar to the entered values
                    IEnumerable<SolarSystem> systems = _dataRepository.GetSolarSystemsLikeName(userText);
                    if (systems.Count() > 0)
                    {
                        newTextBox.Items.AddRange(systems.ToArray());
                        //New items were added, so we want to display the drop down so they can select one. They MUST select one.
                        newTextBox.DropDownStyle = ComboBoxStyle.Simple;
                        //Height of the drop down is either 121 if the list height exceedes 121, with a height of 21 for each item. If less, the drop down is equal to minimum height for the items.
                        int height = ((systems.Count() * 21) > 100) ? 121 : ((systems.Count() * 21) + 21);
                        //Re-size the combo back to standard if less than 3 values entered. This is because they might exceed 3, then backspace.
                        newTextBox.Size = new Size(contSize.Width, height);
                        newTextBox.BringToFront();
                    }
                    else
                        //If there are no systems, hide the drop down (Even if its not shown)
                        newTextBox.Size = new Size(contSize.Width, 21);
                }
                else
                {
                    //If there are no systems, hide the drop down (Even if its not shown)
                    newTextBox.Size = new Size(contSize.Width, 21);
                }
                //Highlight the whole combobox, and put the cursor at the end
                newTextBox.Select(newTextBox.Text.Length, 0);
                //Bring the dropdown to the front so nothing obstructs it.
                newTextBox.BringToFront();
            };
            newTextBox.Size = new Size(140, 21);
            //Calcualate the next logical position based on the existing controls. 10 padding
            if (controlCount > 0)
                newTextBox.Location = new Point(10, ((controlCount * 21) + controlCount * 9) + 9);
            else
                newTextBox.Location = new Point(10, 9);           

        }

        private void PopulateSearchForm()
        {
            //Get the default value from teh Numberic control, then attempt to update the value from the Cache. (APPLIES TO ALL SKILLS, SHIP TYPE AND SHIP QUANTITY)
            decimal JDC = numJDC.Value;
            if (_cacheManager.Cache.GetData("JDC_Skill_Level") != null)
                Decimal.TryParse(_cacheManager.Cache.GetData("JDC_Skill_Level").ToString(), out JDC);                          
            
            decimal JFC = numJFC.Value;
            if (_cacheManager.Cache.GetData("JFC_Skill_Level") != null)
                Decimal.TryParse(_cacheManager.Cache.GetData("JFC_Skill_Level").ToString(), out JFC);

            decimal JF = numJF.Value;
            if (_cacheManager.Cache.GetData("JF_Skill_Level") != null)
                Decimal.TryParse(_cacheManager.Cache.GetData("JF_Skill_Level").ToString(), out JF);

            decimal F = numF.Value;
            if (_cacheManager.Cache.GetData("F_Skill_Level") != null)
                Decimal.TryParse(_cacheManager.Cache.GetData("F_Skill_Level").ToString(), out F);

            decimal shipQuant = numShipQuant.Value;
            if (_cacheManager.Cache.GetData("Search_Ship_Quant") != null)
                Decimal.TryParse(_cacheManager.Cache.GetData("Search_Ship_Quant").ToString(), out shipQuant);

            //update the controls values with the possibly new values
            numJDC.Value = JDC;
            numJFC.Value = JFC;
            numJF.Value = JF;
            numF.Value = F;
            numShipQuant.Value = shipQuant;        

            IEnumerable<Ship> ships = _dataRepository.GetAllShips();
            cmbShipType.Items.AddRange(ships.ToArray());

            //If the cache has the users prefered ship type, get it.
            if (_cacheManager.Cache.GetData("Search_Ship_Type") != null)
            {
                //Retrieve prefered ship object
                Ship chosenShip = (Ship)_cacheManager.Cache.GetData("Search_Ship_Type");
                for (int i = 0; i < cmbShipType.Items.Count; i++)
                {
                    //For each of the items in the ship type dropdown, if its text is the same as the users prefered ship, set it to default based on index (i)
                    if (((Ship)cmbShipType.Items[i]).ShipName.ToUpper() == chosenShip.ShipName.ToUpper())
                    {
                        cmbShipType.SelectedIndex = i;
                    }
                }
            }
            else
                //If the cache does not have the users value, set it to 0. Do this last, as it triggers the change index event, thus refreshing the cache
                cmbShipType.SelectedIndex = 0;

            //If the contract has a start system, populate the locaton combobox
            if (_activeContract.StartSystem != null)
            {
                _startLoc = _activeContract.StartSystem;
                cmbStartLoc.Text = _startLoc.SolarSystemName;
            }
            //If the contract has a end system, populate the locaton combobox
            if (_activeContract.EndSystem != null)
            {
                _endDesto = _activeContract.EndSystem;
                cmbDesto.Text = _endDesto.SolarSystemName;
            }

            List<Ship> shipItems = new List<Ship>();
            shipItems.Add(new Ship() { ItemID = 34126, ShipName = "Prototype Jump Drive Economizer " });
            shipItems.Add(new Ship() { ItemID = 34124, ShipName = "Experimental Jump Drive Economizer " });
            shipItems.Add(new Ship() { ItemID = 34122, ShipName = "Limited Jump Drive Economizer " });
            shipItems.Add(new Ship() { ItemID = 1319, ShipName = "Expanded Cargohold II" });

            cmbSlotOne.Items.AddRange(shipItems.ToArray());
            cmbSlotTwo.Items.AddRange(shipItems.ToArray());
            cmbSlotThree.Items.AddRange(shipItems.ToArray());

            int temp = 0;
            int maxWidth = 0;
            foreach (string itemName in shipItems.Select(x => x.ShipName))
            {
                temp = TextRenderer.MeasureText(itemName, cmbSlotOne.Font).Width;
                if (temp > maxWidth)
                    maxWidth = temp;
            }
            cmbSlotOne.DropDownWidth = maxWidth;
            cmbSlotTwo.DropDownWidth = maxWidth;
            cmbSlotThree.DropDownWidth = maxWidth;
            
            SetPresets();

            //If the contract has both a start and end system, run a calculation straight away.
            if (_activeContract.EndSystem != null && _activeContract.StartSystem != null)
                Calculate();
        }

        private void SetPresets()
        {
            string routeName = "New Route";
            if (cmbPresets.Text != "")
            {
                routeName = cmbPresets.Text.Trim();
            }            
            List<Route> savedRoutes = _dataRepository.GetAllRoutes().ToList();
            savedRoutes.Insert(0, new Route() { RouteName = "New Route" });
            cmbPresets.DisplayMember = "RouteName";
            cmbPresets.DataSource = savedRoutes;
            
            cmbPresets.SelectedIndex = cmbPresets.FindStringExact(routeName);
            if (cmbPresets.SelectedItem != null)
                cmbPresets.Text = ((Route)cmbPresets.SelectedItem).RouteName;
            else
                cmbPresets.Text = "New Route";
        }

        private void Calculate()
        {
            //Do a basic check on the search parameters
            if (RunValidation())
            {
                List<SolarSystem> chosenRoute = new List<SolarSystem>();
                chosenRoute.Add(_startLoc);
                List<SolarSystem> chosenSubRoute = new List<SolarSystem>();

                for (int i = 1; i < pnlMidpoints.Controls.Count+1; i++)
                {                    
                    foreach (Control cont in pnlMidpoints.Controls)
                    {
                        if ((int)cont.Tag == i)
                        {
                            if ((SolarSystem)(((ComboBox)cont).SelectedItem) != null)
                            {
                                chosenSubRoute.Add((SolarSystem)(((ComboBox)cont).SelectedItem));
                            }   
                        }
                    }
                }

                chosenRoute.AddRange(chosenSubRoute);
                chosenRoute.Add(_endDesto);

                float shipJumpDistance = CalculateJumpDistance();

                List<SolarSystem> jumpRoute = new List<SolarSystem>();
                for (int i = 0; i < chosenRoute.Count() - 1; i++)
                {
                    List<SolarSystem> curJumpRoute = Models.Algorithms.GetShortestJumpPath(chosenRoute[i], chosenRoute[i + 1], shipJumpDistance).ToList();
                    if (jumpRoute.Count() > 0 && curJumpRoute.Count() > 0)
                    {
                        if (curJumpRoute[0].SolarSystemName == jumpRoute[jumpRoute.Count() - 1].SolarSystemName)
                        {
                            jumpRoute.RemoveAt(jumpRoute.Count() - 1);
                        }    
                    }                    
                    jumpRoute.AddRange(curJumpRoute);
                }

                pnlMidpoints.Controls.Clear();
                for (int i = 1; i < jumpRoute.Count - 1; i++)
                {                    
                    AddMidpoint(jumpRoute[i]);
                }

                float jumpDistance = 0;

                for (int i = 1; i < jumpRoute.Count(); i++)
                {
                    if (jumpRoute[i].Security < 0.5m)
                    {
                        jumpDistance += Models.Algorithms.GetEuclideanDistance(jumpRoute[i - 1], jumpRoute[i]);
                    }                    
                }

                lstRoute.DisplayMember = "SolarSystemDisplayName";
                lstRoute.Items.Clear();
                foreach (SolarSystem system in jumpRoute)
                {
                    lstRoute.Items.Add(system);
                }

                //Calculate the LY range, not meters
                jumpDistance = jumpDistance / 9460730472580800;

                
                lblTotalDistance.Text = string.Format("{0} LY", jumpDistance.ToString("#.##"));

                lblTotalCargo.Text = string.Format("{0} m3", (CalculateCargoSpace() * _shipQuant).ToString("N"));
                lblCargoPerShip.Text = string.Format("{0} m3", (CalculateCargoSpace()).ToString("N"));

                float fuelConsumptionPerShip = CalculateFuel(jumpDistance);

                lblTotalFuelConsumption.Text = string.Format("{0} Units ({1} m3)", (Math.Ceiling((decimal)fuelConsumptionPerShip) * _shipQuant).ToString("N"), ((Math.Ceiling((double)fuelConsumptionPerShip) * _shipQuant) * 0.10).ToString("N"));
                lblFuelPerShip.Text = string.Format("{0} Units ({1} m3)", Math.Ceiling((decimal)fuelConsumptionPerShip).ToString("N"), (Math.Ceiling((double)fuelConsumptionPerShip) * 0.10).ToString("N"));

                decimal fuelCostPU = Convert.ToDecimal(_cacheManager.Cache.GetData(_chosenShip.FuelID.ToString()));
                lblTotalFuelCost.Text = string.Format("{0} ISK ({1} ISK P/U)", (fuelCostPU * (Math.Ceiling((decimal)fuelConsumptionPerShip) * _shipQuant)).ToString("N"), fuelCostPU.ToString("N"));

                switch (_chosenShip.FuelID)
                {
                    case 16274 :
                        picFuelType.Image = Properties.Resources.HeliumIsotopes;
                        break;
                    case 17889:
                        picFuelType.Image = Properties.Resources.HydrogenIsotopes;
                        break;
                    case 17888:
                        picFuelType.Image = Properties.Resources.NitrogenIsotopes;
                        break;
                    case 17887:
                        picFuelType.Image = Properties.Resources.OxygenIsotopes;
                        break;
                }
            }
        }

        private float CalculateJumpDistance()
        {
            double range = _chosenShip.JumpRange;
            for (int i = 1; i <= _JDC; i++)
            {
                range = range + (range * 0.25);
            }
            return (float)(9460730472580800 * range);
        }

        private float CalculateFuel(float distance)
        {
            List<int> shipItemsValues = new List<int>();

            if (cmbSlotOne.SelectedItem != null)
                shipItemsValues.Add(((Ship)cmbSlotOne.SelectedItem).ItemID);
            if (cmbSlotTwo.SelectedItem != null)
                shipItemsValues.Add(((Ship)cmbSlotTwo.SelectedItem).ItemID);
            if (cmbSlotThree.SelectedItem != null)
                shipItemsValues.Add(((Ship)cmbSlotThree.SelectedItem).ItemID);

            float fuelConsumption = _chosenShip.FuelConsumption * distance;
          
            
            //float fuelConsumption = _chosenShip.FuelConsumption;

            //4126, ShipName = "Prototype Jump Drive Economizer " });
            //shipItems.Add(new Ship() { ItemID = 34124, ShipName = "Experimental Jump Drive Economizer " });
            //shipItems.Add(new Ship() { ItemID = 34122,

            float reductionVal = 0;

            switch (shipItemsValues.Where(x => x == 34126).Count())
            {
                case 0:
                    break;
                case 1:
                    reductionVal = 10;
                    break;
                case 2:
                    reductionVal = 15;
                    break;
                case 3:
                    reductionVal = 17.5F;
                    break;
            }

            //for (int i = 1; i <= shipItemsValues.Where(x => x == 34126).Count(); i++)
            //{
            // //   fuelConsumption = fuelConsumption - (fuelConsumption / (10 / i));
            //    reductionVal = reductionVal + (10 / i);
            //}

            fuelConsumption = fuelConsumption - ((fuelConsumption / 10) * _JFC);
            fuelConsumption = fuelConsumption - ((fuelConsumption / 10) * _JF);

            fuelConsumption = fuelConsumption - (int)((float)fuelConsumption * (reductionVal / 100));

           

            return fuelConsumption;
        }

        private double CalculateCargoSpace()
        {
            int cargoExpanders = 0;
            if (cmbSlotOne.SelectedItem != null && ((Ship)cmbSlotOne.SelectedItem).ItemID == 1319)
                cargoExpanders++;
            if (cmbSlotTwo.SelectedItem != null && ((Ship)cmbSlotTwo.SelectedItem).ItemID == 1319)
                cargoExpanders++;
            if (cmbSlotThree.SelectedItem != null && ((Ship)cmbSlotThree.SelectedItem).ItemID == 1319)
                cargoExpanders++;

            double cargoSpace = Convert.ToDouble(_chosenShip.CargoCapacity);
            if (cargoExpanders > 0)
            {
                for (int i = 1; i <= cargoExpanders; i++)
                {
                    cargoSpace = cargoSpace + (cargoSpace * 0.275);
                }    
            }
            
            cargoSpace = cargoSpace + ((cargoSpace * 0.05) * _F);
            return Math.Ceiling(cargoSpace);
        }

        private bool RunValidation()
        {
            string errors = "";
            if (_startLoc == null)
                errors += "Must Select A Start Location \r";
            if (_endDesto == null)
                errors += "Must Select A Destination \r";
            if (errors.Length > 0)
            {
                MessageBox.Show("Errors :\r" + errors);
                return false;
            }
            return true;
        }
    }
}
