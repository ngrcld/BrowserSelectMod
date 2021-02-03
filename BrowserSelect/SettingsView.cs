using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Win32;

using Newtonsoft.Json;

using BrowserSelect.Properties;

namespace BrowserSelect
{
    //=============================================================================================================
    public partial class SettingsView : Form
    //=============================================================================================================
    {
        private bool isLoaded = false;
        private bool isDirty = false;
        private bool isDirtyUI = false;
        private bool isDirtyCache = false;

        private BrowserSelectView parentView;
        List<BrowserModel> browsers;
        private ObservableCollection<RuleModel> rules = new ObservableCollection<RuleModel>();

        //-------------------------------------------------------------------------------------------------------------
        public SettingsView(Form parentForm)
        //-------------------------------------------------------------------------------------------------------------
        {
            this.parentView = (BrowserSelectView)parentForm;
            InitializeComponent();
        }

        //-------------------------------------------------------------------------------------------------------------
        private void SettingsView_Load(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            MinimizeBox = false;

            //check if browser select is the default browser or not
            //to disable/enable "set Browser select as default" button
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice"))
            {
                var defaultBrowser = key?.GetValue("ProgId");

                //disable the set default if already default
                if (defaultBrowser != null && (string)defaultBrowser == "BrowserSelectURL")
                    SetDefaultButton.Enabled = false;
            }

            var typeColumn = ((DataGridViewComboBoxColumn)RulesGrid.Columns["type"]);
            typeColumn.Items.Add("URL");
            typeColumn.Items.Add("Process");

            //populate list of browsers for RulesGrid, BrowserList, and DefaultBrowserList
            BrowserFinder finder = new BrowserFinder();
            browsers = finder.FindBrowsers();

            var browserColumn = ((DataGridViewComboBoxColumn)RulesGrid.Columns["browser"]);
            DefaultBrowserList.Items.Add("<choose with BrowserSelect>");

            foreach (BrowserModel browser in browsers)
            {
                BrowserList.Items.Add(browser, !Settings.Default.HiddenBrowsers.Contains(browser.Identifier));
                DefaultBrowserList.Items.Add(browser.ToString());
                browserColumn.Items.Add(browser.ToString());
            }

            if (String.IsNullOrEmpty(Settings.Default.DefaultBrowser))
                DefaultBrowserList.SelectedIndex = 0;
            else
                DefaultBrowserList.SelectedItem = Settings.Default.DefaultBrowser;

            //populate rules list data source and bind it to its grid
            foreach (var rule in Settings.Default.Rules)
                rules.Add(rule);
            var bs = new BindingSource();
            bs.DataSource = rules;
            RulesGrid.DataSource = bs;

            isDirty = false;
            UpdateUIState();

            CenterWindow();

            this.BeginInvoke(new Action(() =>
            {
                isLoaded = true;
            }));
        }

        //-------------------------------------------------------------------------------------------------------------
        private void SettingsView_FormClosing(object sender, FormClosingEventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            // alert user of unsaved changes
            if (isDirty)
            {
                var mbox = MessageBox.Show("You have unsaved changes, are you sure you want to close without saving?",
                                             "Unsaved Changes", 
                                             MessageBoxButtons.YesNo);
                if (mbox == DialogResult.No) 
                    e.Cancel = true;
                else 
                    e.Cancel = false;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        private void SetDefaultButton_Click(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            //set browser select as default in registry

            //http
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice"))
            {
                key.SetValue("ProgId", "BrowserSelectURL");
            }
            //https
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\https\UserChoice"))
            {
                key.SetValue("ProgId", "BrowserSelectURL");
            }

            SetDefaultButton.Enabled = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        private void SaveButton_Click(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            //clear rules (instead of checking for changes we just overwrite the whole ruleset)
            Settings.Default.Rules.Clear();
            foreach (var rule in rules)
            {
                //check if rule has both pattern and browser defined
                if (rule.isValid())
                {
                    //add it to rule list
                    Settings.Default.Rules.Add(rule.ToString());
                }
                else
                {
                    //ignore rule if both pattern and browser is empty otherwise inform user of missing part
                    var err = rule.errorMessage();
                    if (err.Length > 0)
                        MessageBox.Show("Invalid Rule: " + err);
                }

            }

            if (isDirtyCache)
            {
                Settings.Default.CachedBrowserList = JsonConvert.SerializeObject(browsers);
            }

            if (isDirty)
            {
                //save settings
                Settings.Default.Save();
            }

            if (isDirtyUI)
            {
                this.parentView.RefreshBrowserListUI();
            }

            //Update UI
            isDirty = false;
            isDirtyUI = false;
            isDirtyCache = false;
            UpdateUIState();

            Close();
        }

        //-------------------------------------------------------------------------------------------------------------
        private void CancelButton_Click(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            //Check for isDirty will happen in the FormClosing event handler
            Close();
        }

        //-------------------------------------------------------------------------------------------------------------
        private void RefreshButton_Click(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            var mbox = MessageBox.Show("Refreshing the browser list will remove any custom ordering set on the list.  Are you sure?",
                                       "Refresh Browser List",
                                       MessageBoxButtons.YesNo);
            if (mbox == DialogResult.No)
                return;

            //populate list of browsers for RulesGrid, BrowserList, and DefaultBrowserList
            BrowserFinder finder = new BrowserFinder();
            browsers = finder.FindBrowsers(true);

            var browserColumn = ((DataGridViewComboBoxColumn)RulesGrid.Columns["browser"]);
            browserColumn.Items.Clear();
            BrowserList.Items.Clear();
            DefaultBrowserList.Items.Clear();
            DefaultBrowserList.Items.Add("<choose with BrowserSelect>");

            foreach (BrowserModel browser in browsers)
            {
                BrowserList.Items.Add(browser, !Settings.Default.HiddenBrowsers.Contains(browser.Identifier));
                DefaultBrowserList.Items.Add(browser.ToString());
                browserColumn.Items.Add(browser.ToString());
            }

            if (String.IsNullOrEmpty(Settings.Default.DefaultBrowser))
                DefaultBrowserList.SelectedIndex = 0;
            else
                DefaultBrowserList.SelectedItem = Settings.Default.DefaultBrowser;
        }

        //-------------------------------------------------------------------------------------------------------------
        private void BrowserList_ItemCheck(object sender, ItemCheckEventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            this.BeginInvoke(new Action(() =>
            {
                //Save list of checked items in the BrowserList
                Settings.Default.HiddenBrowsers.Clear();
                foreach (object item in BrowserList.Items)
                {
                    if (!BrowserList.CheckedItems.Contains(item))
                        Settings.Default.HiddenBrowsers.Add(((BrowserModel)item).Identifier);
                }

                if (isLoaded)
                {
                    isDirty = true;
                    isDirtyUI = true;
                    UpdateUIState();
                }
            }));
        }

        //-------------------------------------------------------------------------------------------------------------
        private void BrowserUpButton_Click(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            int idxSelected = BrowserList.SelectedIndex;
            if (idxSelected != 0)
            {
                int idxNew = idxSelected - 1;
                
                BrowserModel modelSelected = new BrowserModel();
                modelSelected = browsers[idxSelected];
                browsers[idxSelected] = browsers[idxNew];
                BrowserList.Items[idxSelected] = browsers[idxNew];
                BrowserList.SetItemChecked(idxSelected, !Settings.Default.HiddenBrowsers.Contains(browsers[idxNew].Identifier));
                browsers[idxNew] = modelSelected;
                BrowserList.Items[idxNew] = browsers[idxNew];
                BrowserList.SetItemChecked(idxNew, !Settings.Default.HiddenBrowsers.Contains(browsers[idxNew].Identifier));

                BrowserList.SelectedIndex = idxNew;

                isDirty = true;
                isDirtyUI = true;
                isDirtyCache = true;

                UpdateUIState();
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        private void BrowserList_SelectedIndexChanged(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            UpdateUIState();
        }

        //-------------------------------------------------------------------------------------------------------------
        private void BrowserDownButton_Click(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            int idxSelected = BrowserList.SelectedIndex;
            if (idxSelected < (BrowserList.Items.Count-1))
            {
                int idxNew = idxSelected + 1;

                BrowserModel browserSelected = new BrowserModel();
                browserSelected = browsers[idxSelected];
                browsers[idxSelected] = browsers[idxNew];
                BrowserList.Items[idxSelected] = browsers[idxNew];
                BrowserList.SetItemChecked(idxSelected, !Settings.Default.HiddenBrowsers.Contains(browsers[idxNew].Identifier));
                browsers[idxNew] = browserSelected;
                BrowserList.Items[idxNew] = browsers[idxNew];
                BrowserList.SetItemChecked(idxNew, !Settings.Default.HiddenBrowsers.Contains(browsers[idxNew].Identifier));

                BrowserList.SelectedIndex = idxNew;

                isDirty = true;
                isDirtyUI = true;
                isDirtyCache = true;

                UpdateUIState();
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        private void DefaultBrowserList_SelectedIndexChanged(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            Settings.Default.DefaultBrowser = DefaultBrowserList.SelectedItem.ToString();

            isDirty = true;
            UpdateUIState();
        }

        //-------------------------------------------------------------------------------------------------------------
        private void RuleUpButton_Click(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            int rowCurrent = RulesGrid.CurrentCell.RowIndex;
            int colCurrent = RulesGrid.CurrentCell.ColumnIndex;
            if (rowCurrent != 0)
            {
                int rowNew = rowCurrent - 1;
                rules.Move(rowCurrent, rowNew);
                RulesGrid.Refresh();
                RulesGrid.CurrentCell = RulesGrid.Rows[rowNew].Cells[colCurrent];

                RulesGrid_CellBeginEdit(sender, e);
                UpdateUIState();
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        private void RuleDownButton_Click(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            int rowCurrent = RulesGrid.CurrentCell.RowIndex;
            int colCurrent = RulesGrid.CurrentCell.ColumnIndex;
            if (rowCurrent < (rules.Count-1))
            {
                int rowNew = rowCurrent + 1;
                rules.Move(rowCurrent, rowNew);
                RulesGrid.Refresh();
                RulesGrid.CurrentCell = RulesGrid.Rows[rowNew].Cells[colCurrent];

                RulesGrid_CellBeginEdit(sender, e);
                UpdateUIState();
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        private void RulesGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            UpdateUIState();
        }

        //-------------------------------------------------------------------------------------------------------------
        private void RulesGrid_CellBeginEdit(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            isDirty = true;
            UpdateUIState();
        }

        //-------------------------------------------------------------------------------------------------------------
        private void RulesGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            //Swallow event to prevent "System.ArgumentException: DataGridViewComboBoxCell value is not valid" errors
        }

        //-------------------------------------------------------------------------------------------------------------
        private void CenterWindow()
        //-------------------------------------------------------------------------------------------------------------
        {
            var wa = Screen.FromPoint(new Point(Cursor.Position.X, Cursor.Position.Y)).WorkingArea;
            var left = wa.Width / 2 + wa.Left - Width / 2;
            var top = wa.Height / 2 + wa.Top - Height / 2;

            this.Location = new Point(left, top);
            this.Activate();
        }

        //-------------------------------------------------------------------------------------------------------------
        private void UpdateUIState()
        //-------------------------------------------------------------------------------------------------------------
        {
            int rowCurrent = (RulesGrid.CurrentCell != null) ? RulesGrid.CurrentCell.RowIndex : -1;
            RuleUpButton.Enabled = (rowCurrent != 0);
            RuleDownButton.Enabled = (rowCurrent < (rules.Count-1));

            int idxCurrent = BrowserList.SelectedIndex;
            BrowserUpButton.Enabled = (idxCurrent != 0);
            BrowserDownButton.Enabled = (idxCurrent < (BrowserList.Items.Count-1));

            SaveButton.Enabled = isDirty;
        }
    }
}
