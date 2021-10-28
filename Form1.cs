using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
using System.Windows.Forms;

namespace FolderAutoUploader
{
    public partial class Form1 : Form
    {
        public List<DateTime> dates = new List<DateTime>();
        private DateTime DailyTime;
        public DateTime _dateLastRan;

        public bool _dailyChecked;
        public bool _alreadyRanToday;

        public string uploadPath;
        public string replacePath;

        public bool overwriteFiles;

        private string saveFileLocation;
        private string datesFileLocation;

        public static bool workin;

        public static System.Timers.Timer timer;

        public static int _folderLimit;

        //Used for the schedule form
        public static List<LocationsAdd> _scheduleLocations;
        public static List<ProcessHelper> _processes;


        //Settings
        public static bool _loadOnStartup;

        public Form1()
        {
            InitializeComponent();

            //Set tooltips
            toolTip1.SetToolTip(this.label2, "Pick a folder to override another");
            toolTip1.SetToolTip(this.label3, "Pick a folder that gets overwritten");
            toolTip1.SetToolTip(this.overwriteCheckbox, "Deletes and Creates a new folder with the same name");
            toolTip1.SetToolTip(this.dailyCheckBox, "Used to set the upload to 'daily' (Will save on exit and not allow other dates to be stored)");
            toolTip1.SetToolTip(this.datePickerButton, "*WIP*");

            //Rich text box
            loggerInfo.HideSelection = false;

            //Get the path for the locations 
            string folderName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            saveFileLocation = folderName + @"\CopyPasteData\locationSaves.txt";

            //Get the path for the dates
            string folderNameDates = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            datesFileLocation = folderNameDates + @"\CopyPasteData\dates.bin"; //Need to change to data at some point ********---------------------------------------------------------

            //set and reset
            _scheduleLocations = new List<LocationsAdd>();

            //Check to make sure the directory is created
            if (!ProcessHelper.CheckDirectory(folderName + @"\CopyPasteData"))
            {
                Directory.CreateDirectory(folderName + @"\CopyPasteData");
            }

            //Set to blank so we don't have to see it
            fileInfoLabel.Text = "";
            folderInfoLabel.Text = "";

            //Set the calendar to not precede today
            dateTimePicker.MinDate = DateTime.Today;

            _processes = new List<ProcessHelper>();

            //Load in the dates file and check it
            if (File.Exists(datesFileLocation))
            {
                IFormatter formatter = new BinaryFormatter();

                Stream stream = new FileStream(datesFileLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
                Data infoData = (Data)formatter.Deserialize(stream);
                stream.Close();

                dates = infoData.dates;
                _dailyChecked = infoData.dailyChecked;
                _alreadyRanToday = infoData.alreadyRanToday;
                _dateLastRan = infoData.dateLastRan;
                _folderLimit = infoData.folderLimit;
                _scheduleLocations = infoData.scheduleLocations;
                _loadOnStartup = infoData.loadOnStartup;

                //Used for the first time or when there is nothing
                if (_scheduleLocations == null)
                {
                    _scheduleLocations = new List<LocationsAdd>();
                }
            }
            else
            {
                //Print("Dates file not created yet (Will happen on first ever launch or if the file gets deleted)"); //WILL CAUSE ERROR ATM. MOVE TO LOAD?
            }

            //Set the value of the incriminate
            if (_folderLimit != null && _folderLimit > 0)
            {
                numericUpDown1.Value = _folderLimit;
            }
            else numericUpDown1.Value = 5;

            numericUpDown1.Maximum = 15;
            numericUpDown1.Minimum = 1;

            //Check and see if the program already ran today and if not set it to false
            if (_alreadyRanToday)
            {
                DateTime today = DateTime.Today;

                if (_dateLastRan != today && _dateLastRan != DateTime.MinValue)
                {
                    _alreadyRanToday = false;
                }
            }

            //Check to make sure to set the check box to daily if it was already set to begin with
            if (_dailyChecked)
            {
                dailyCheckBox.Checked = true;

            }

            //Load in the dates that are already selected
            foreach (DateTime date in dates)
            {
                //Add the text into the date
                dateTextBox.AppendText(date + "\n");
            }

            //Set up the timer
            SetupTimer();

            //on exit save all the dates
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            //ProcessHelper.Print($"Already ran today: {_alreadyRanToday}. The Date last ran: {_dateLastRan}");
        }


        #region Delcared_Functions
        /// <summary>
        /// The timer used to check the dates
        /// </summary>
        private void SetupTimer()
        {

            //Every minute if the daily is checked then set the timer to check if the day is the right day.
            if (_dailyChecked && !_alreadyRanToday && !workin)
            {
                timer = new System.Timers.Timer(30000);
                timer.Elapsed += timer_Elapsed;
                timer.Start();
            }


        }

        /// <summary>
        /// Used to save the dates and other daily things that need saved
        /// </summary>
        private void Save()
        {
            //Save the selected
            for (int i = 0; i < checkedListBoxLocations.Items.Count; i++)
            {
                _scheduleLocations[i].selected = checkedListBoxLocations.GetItemChecked(i);
            }

            BinaryFormatter formatter = new BinaryFormatter();
            Data infoData = new Data();

            infoData.dates = dates;
            infoData.dailyChecked = _dailyChecked;
            infoData.alreadyRanToday = _alreadyRanToday;
            infoData.dateLastRan = _dateLastRan;
            infoData.folderLimit = _folderLimit;
            infoData.scheduleLocations = _scheduleLocations;
            infoData.loadOnStartup = _loadOnStartup;

            using (FileStream stream = File.OpenWrite(datesFileLocation))
            {
                formatter.Serialize(stream, infoData);
                stream.Close();
            }
        }

        /// <summary>
        /// Used to run functions at the end of the program (When it closes)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExit(object sender, EventArgs e)
        {

            if (ProcessHelper.CheckDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\CopyPasteData"))
            {
                Save();
            }
            else
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\CopyPasteData");

                Save();
            }

        }

        /// <summary>
        /// Enable or disable of the buttons
        /// </summary>
        /// <param name="on"></param>
        public void EnableButtons(bool on)
        {
            Invoke(new Action(() =>
            {
                //Disable the buttons once the program starts
                uploadButton.Enabled = on;
                SaveButton.Enabled = on;
                LoadButton.Enabled = on;

                overwriteCheckbox.Enabled = on;

                searchButton1.Enabled = on;
                searchButton2.Enabled = on;

                copySelectedButton.Enabled = on;
            }));

        }


        /// <summary>
        /// Reset the buttons to make sure everything can be used again
        /// </summary>
        public void Reset()
        {

            Invoke(new Action(() =>
            {
                EnableButtons(true);

                progressBar1.Value = 0;
                progressBar2.Value = 0;

                fileInfoLabel.Text = "";
                folderInfoLabel.Text = "";
                runningLabel.Text = "";
            }));

        }

        /// <summary>
        /// Used to save the upload and replace path in a text folder for later reading
        /// </summary>
        private void saveToFile()
        {
            //reset
            File.WriteAllText(saveFileLocation, "");

            using (StreamWriter sw = File.AppendText(saveFileLocation))
            {
                sw.WriteLine(uploadLocationString.Text);
                sw.WriteLine(replaceLocationString.Text);

                sw.Close();
            }

            Print("Upload and replace path have been saved");
        }

        /// <summary>
        /// Used to specify the event that happens once the timer runs out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            DailyTime = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0, 0);

            if (DailyTime <= now && _dailyChecked)
            {
                Invoke(new Action(() =>
                {
                    //Change the label to running
                    runningLabel.Text = "Running...";
    
                    button1_Click_1();

                    Print("Starting...");

                    _alreadyRanToday = true;
                    _dateLastRan = now;

                }));
            }

            timer.Stop();
            timer.Dispose();
        }

        #endregion

        //Used to start an copy action
        public void button1_Click(object sender = null, EventArgs e = null)
        {
            ProcessHelper copy = new ProcessHelper(this, true, 0, uploadLocationString.Text, replaceLocationString.Text, overwriteFiles, _folderLimit, progressBar1, progressBar2, folderInfoLabel, fileInfoLabel);
            copy.Start();
        }

        private void Upload_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void DatePicker_Click(object sender, EventArgs e)
        {
            //ProcessHelper.Print the date in the logger
            //ProcessHelper.Print(dateTimePicker.Text);

            if (!dates.Contains(dateTimePicker.Value))
            {
                //Add to the list
                dates.Add(dateTimePicker.Value);

                //Add the text into the date
                dateTextBox.Text += dateTimePicker.Text + "\n";
            }
            else
            {
                Print("Already in the list");
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void loggerInfo_TextChanged(object sender, EventArgs e)
        {

        }

        private void searchButton1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string folder = folderBrowserDialog1.SelectedPath;

                //check to make sure the directory exists
                if (ProcessHelper.CheckDirectory(folder))
                {
                    uploadPath = folder;

                    uploadLocationString.Text = folder;
                }
            }
        }

        private void searchButton2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string folder = folderBrowserDialog1.SelectedPath;

                //check to make sure the directory exists
                if (ProcessHelper.CheckDirectory(folder))
                {
                    replacePath = folder;

                    replaceLocationString.Text = folder;
                }
            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void overwriteCheckbox_CheckedChanged(object sender, EventArgs e)
        {

            overwriteFiles = overwriteCheckbox.Checked;

            if(overwriteCheckbox.Checked)
            {
                uploadButton.Text = "Overwrite";
            }
            else uploadButton.Text = "Copy";
        }

        private void loggerClearButton_Click(object sender, EventArgs e)
        {
            //reset
            loggerInfo.Text = "";

            progressBar1.Value = 0;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!uploadPath.Equals("") || !replacePath.Equals(""))
            {

                //if the file don't exist create it
                if (!File.Exists(saveFileLocation))
                {
                    using (StreamWriter sw = File.CreateText(saveFileLocation))
                    {

                    }

                    Print("Save file created");
                }

                //save to the file
                saveToFile();
            }
            else Print("Error - One or more files do not exist or have not been selected");
        }

        private void LoadButton_Click(object sender = null, EventArgs e = null)
        {
            if (File.Exists(saveFileLocation))
            {
                string[] lines = File.ReadAllLines(saveFileLocation);

                if (lines.Length > 0)
                {
                    uploadPath = lines[0];
                    replacePath = lines[1];

                    Invoke(new Action(() =>
                    {
                        uploadLocationString.Text = uploadPath;
                        replaceLocationString.Text = replacePath;

                    }));

                    Print("Loaded in paths");
                }
            }
            else Print("Nothing to load");

        }

        /// <summary>
        /// Logger
        /// </summary>
        /// <param name="print"></param>
        public void Print(object print)
        {

            Invoke(new Action(() =>
            {
                try
                {
                    loggerInfo.AppendText("[" + DateTime.Now + "] " + print.ToString() + "\n");
                }
                catch (Exception e)
                {

                }
            }));

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void canclelButton_Click(object sender, EventArgs e)
        {
            if (workin)
            {
                workin = false;

                Print("Canceling");
            }
            else Print("No process is running");
        }

        private void dailyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!dailyCheckBox.Checked)
            {
                _dailyChecked = false;
                datePickerButton.Enabled = true;
                dateTimePicker.Enabled = true;
            }
            else
            {
                _dailyChecked = true;

                dateTextBox.Text = "";

                datePickerButton.Enabled = false;
                dateTimePicker.Enabled = false;
            }
        }

        private void clearDatesButton_Click(object sender, EventArgs e)
        {
            dates.Clear();

            dateTextBox.Text = "";
        }

        private void dateTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //notify
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.ContextMenu = this.ContextMenu;
            notifyIcon1.Text = "Folder Copy and Replace";
            notifyIcon1.Visible = true;

            cancelButton.Enabled = false;

            windowsStartupCheckBox.Checked = _loadOnStartup;

            //Fill in the cells for the locations
            if (_scheduleLocations != null && _scheduleLocations.Count > 0)
            {
                foreach (LocationsAdd location in _scheduleLocations)
                {
                    checkedListBoxLocations.Items.Add(location.name, location.selected);
                }
            }

            


            //Display the version
            Print("Version: V0.0.3.4");
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _folderLimit = (int)numericUpDown1.Value;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        //Add Button
        private void button2_Click(object sender, EventArgs e)
        {
            AddLocationsForm addLocationsForm = new AddLocationsForm();
            DialogResult dialogResult = addLocationsForm.ShowDialog();

            //Refresh the list to make all the new changes appear
            checkedListBoxLocations.Items.Clear();

            //Fill in the cells for the locations
            if (_scheduleLocations != null && _scheduleLocations.Count > 0)
            {
                foreach (LocationsAdd location in _scheduleLocations)
                {
                    checkedListBoxLocations.Items.Add(location.name, location.selected);
                }
            }
        }

        //Copy All Selected Button
        private void button1_Click_1(object sender = null, EventArgs e = null)
        {
            if (checkedListBoxLocations.CheckedItems.Count > 0 && !workin)
            {
                int id = 0;

                foreach (string LocationName in checkedListBoxLocations.CheckedItems)
                {
                    LocationsAdd findCheckedLocation = _scheduleLocations.Find(x => x.name.Contains(LocationName));
                    ProcessHelper copy = new ProcessHelper(this, false, id, findCheckedLocation.copy, findCheckedLocation.replace, overwriteFiles, _folderLimit, progressBar1, progressBar2, folderInfoLabel, fileInfoLabel);

                    id++;
                }

                //start
                _processes[0].Start();

                //Debug
                //Print("Processes: " + _processes.Count.ToString());
                //foreach (ProcessHelper prcess in _processes)
                //{
                //    Print("\n");

                //    prcess.countFiles(prcess.uploadPath);

                //    Print("Process ID: " + prcess.workerID.ToString());
                //    Print("UploadPath: " + prcess.uploadPath.ToString());
                //    Print("ReplacePath: " + prcess.replacePath.ToString());
                //    Print("Amount of files " + prcess.actualFiles);

                //}
            }
            else
            {
                Print("You have no locations selected or set up");
            }
        }

        public void NextProcess()
        {
            //0 is the curret process being processed
            if (_processes.Count > 0)
            {
                _processes[0].Start();
            }
            else
            {
                Print("All done with processes"); 
            }
        }

        //Remove Button
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            //Delete it from the static list
            foreach (string Location in checkedListBoxLocations.CheckedItems)
            {
                //Remove from the actual list too
                _scheduleLocations.Remove(_scheduleLocations.Find(x => x.name.Contains(Location)));
            }

            //Get the count
            int CheckedCount = checkedListBoxLocations.CheckedItems.Count;

            //Delete it from the list UI
            for (int i = 0; i < CheckedCount; i++)
            {
                ((IList)checkedListBoxLocations.Items).Remove(checkedListBoxLocations.CheckedItems[0]);
            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        //Get the current selected one?
        private void checkedListBoxLocations_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void windowsStartupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _loadOnStartup = windowsStartupCheckBox.Checked;

            //Check and add the the startup key
            if (_loadOnStartup)
            {
                //Use a double try/catch to make sure the program does not crash and so we can use the finally statement
                try
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                    try
                    {

                        key.SetValue("Auto_Copy/Paste_Program", "\"" + Application.ExecutablePath + "\"");
                        key.Close();
                    }
                    finally
                    {
                        key.Dispose();
                    }
                }
                catch (Exception ee)
                {
                    Print(ee);
                }
            }
            else
            {
                //Use a double try/catch to make sure the program does not crash and so we can use the finally statement
                try
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                    try
                    {

                        key.SetValue("Auto_Copy/Paste_Program", "\"" + Application.ExecutablePath + "\"");
                        key.DeleteValue("Auto_Copy/Paste_Program", false);
                        key.Close();
                    }
                    finally
                    {
                        key.Dispose();
                    }
                }
                catch (Exception ee)
                {
                    Print(ee);
                }
            }
        }
    }

    [Serializable]
    public class Data
    {
        public List<DateTime> dates;
        public List<LocationsAdd> scheduleLocations;
        public DateTime dateLastRan;

        public bool dailyChecked;
        public bool alreadyRanToday;
        public int folderLimit;

        public bool loadOnStartup;
    }

    [Serializable]
    public class LocationsAdd
    {
        public string name;

        public string copy;
        public string replace;

        public bool selected;
    }
}
