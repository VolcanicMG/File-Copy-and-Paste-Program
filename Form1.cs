using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace FolderAutoUploader
{
    public partial class Form1 : Form
    {
        public List<DateTime> dates = new List<DateTime>();
        private DateTime DailyTime;
        private DateTime _dateLastRan;

        public bool _dailyChecked;
        public bool _alreadyRanToday;

        public string uploadPath;
        public string replacePath;

        public bool overwriteFiles;

        private string saveFileLocation;
        private string datesFileLocation;

        Thread workerThread;
        public bool workin;

        public int actualFiles;

        public static System.Timers.Timer timer;

        public int _folderLimit;

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

            //Get the path
            string folderName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            saveFileLocation = folderName + @"\CopyPasteData\locationSaves.txt";

            //Get the path
            string folderNameDates = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            datesFileLocation = folderNameDates + @"\CopyPasteData\dates.bin";

            //Check to make sure the directory is created
            if (!CheckDirectory(folderName + @"\CopyPasteData"))
            {
                Directory.CreateDirectory(folderName + @"\CopyPasteData");
            }

            //Set to blank so we don't have to see it
            fileInfoLabel.Text = "";
            folderInfoLabel.Text = "";

            //Set the calendar to not precede today
            dateTimePicker.MinDate = DateTime.Today;

            //Load in the dates file and check it
            if (File.Exists(datesFileLocation))
            {
                IFormatter formatter = new BinaryFormatter();

                Stream stream = new FileStream(datesFileLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
                Data datesData = (Data)formatter.Deserialize(stream);
                stream.Close();

                dates = datesData.dates;
                _dailyChecked = datesData.dailyChecked;
                _alreadyRanToday = datesData.alreadyRanToday;
                _dateLastRan = datesData.dateLastRan;
                _folderLimit = datesData.folderLimit;

            }
            else
            {
                Print("Dates file not created yet (Will happen on first ever launch or if the file gets deleted)");
            }

            //Set the value of the incriminate
            numericUpDown1.Value = _folderLimit;

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

            //Print($"Already ran today: {_alreadyRanToday}. The Date last ran: {_dateLastRan}");
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
                timer = new System.Timers.Timer(60000);
                timer.Elapsed += timer_Elapsed;
                timer.Start();
            }


        }

        /// <summary>
        /// Used to save the dates and other daily things that need saved
        /// </summary>
        private void SaveDates()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Data datesData = new Data();

            datesData.dates = dates;
            datesData.dailyChecked = _dailyChecked;
            datesData.alreadyRanToday = _alreadyRanToday;
            datesData.dateLastRan = _dateLastRan;
            datesData.folderLimit = _folderLimit;

            using (FileStream stream = File.OpenWrite(datesFileLocation))
            {
                formatter.Serialize(stream, datesData);
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

            if (CheckDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\CopyPasteData"))
            {
                SaveDates();
            }
            else
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\CopyPasteData");

                SaveDates();
            }

        }

        /// <summary>
        /// Logger
        /// </summary>
        /// <param name="print"></param>
        public void Print(string print)
        {
            if (print != null && !print.Equals("") && SynchronizationContext.Current != null)
            {
                loggerInfo.AppendText("[" + DateTime.Now + "] " + print + "\n");
            }
            else if (print != null && !print.Equals(""))
            {
                Invoke(new Action(() =>
                {
                    loggerInfo.AppendText("[" + DateTime.Now + "] " + print + "\n");
                }));
            }
        }

        /// <summary>
        /// Enable or disable of the buttons
        /// </summary>
        /// <param name="on"></param>
        private void EnableButtons(bool on)
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
            }));
            
        }

        /// <summary>
        /// Used to check the existence of a directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private bool CheckDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        ///Used to count all the files in a directory including those within sub directories
        /// </summary>
        /// <param name="uploadLoc">The path used to find the directory</param>
        private void countFiles(string uploadLoc)
        {
            if (CheckDirectory(uploadLoc))
            {

                DirectoryInfo dir = new DirectoryInfo(uploadLoc);
                FileInfo[] files = dir.GetFiles();
                DirectoryInfo[] dirs = dir.GetDirectories();

                //Copy over all the files
                foreach (FileInfo file in files)
                {

                    Invoke(new Action(() =>
                    {
                        actualFiles++;
                    }));

                }

                //sub folders
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(uploadLoc, subdir.Name);

                    countFiles(subdir.FullName);
                }

            }
            else
            {
                Invoke(new Action(() =>
                {
                    Print("Error - not found");
                    Print(CheckDirectory(uploadLoc).ToString() + "-" + " uploadLoc");
                }));
            }
        }

        /// <summary>
        /// Reset the buttons to make sure everything can be used again
        /// </summary>
        private void Reset()
        {

            if (SynchronizationContext.Current != null)
            {
                EnableButtons(true);

                if (workerThread != null && workerThread.IsAlive)
                {
                    workerThread.Abort();
                }

                progressBar1.Value = 0;
                progressBar2.Value = 0;

                fileInfoLabel.Text = "";
                folderInfoLabel.Text = "";

            }
            else
            {
                Invoke(new Action(() =>
                {
                    EnableButtons(true);

                    workerThread.Abort();

                    progressBar1.Value = 0;
                    progressBar2.Value = 0;

                    fileInfoLabel.Text = "";
                    folderInfoLabel.Text = "";
                }));
            }

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
                sw.WriteLine(uploadPath);
                sw.WriteLine(replacePath);

                sw.Close();
            }

            Print("Upload and replace path have been saved");
        }

        public void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime setTime = new DateTime();

            DailyTime = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0, 0);

            if (DailyTime <= now)
            {
                //Load in the file setting if there are any
                LoadButton_Click();

                if (File.Exists(saveFileLocation))
                {
                    Invoke(new Action(() =>
                    {
                        button1_Click(); //Predefine some of the paths for the buttons to respond too (Use the load button)
                    }));
                    
                    Print("Starting...");
                }
                else
                {
                    Print("You need to save something in order for the program to work daily or automatically. Find a coping place and replace path and save them. The next day the progrogam will find it.");
                }

                _alreadyRanToday = true;
                _dateLastRan = now;

                timer.Stop();
                timer.Dispose();
            }
        }

        /// <summary>
        /// Used to copy and paste folders
        /// </summary>
        /// <param name="upload"></param>
        /// <param name="replace"></param>
        private void ConvergeFiles(string upload, string replace)
        {
            if (CheckDirectory(replace) && CheckDirectory(upload))
            {
                Invoke(new Action(() =>
                {
                    progressBar1.Value = 0;
                    
                    folderInfoLabel.Text = "Folder: " + replace;
                }));

                DirectoryInfo dir = new DirectoryInfo(upload);
                FileInfo[] files = dir.GetFiles();
                DirectoryInfo[] dirs = dir.GetDirectories();

                //Set the size of the progress bar's max
                Invoke(new Action(() =>
                {
                    if (dirs.Length > 0)
                    {
                        progressBar1.Maximum = dirs.Length * files.Length;
                    }
                    else
                    {
                        progressBar1.Maximum = files.Length;
                    }
                }));

                //Print("Number of folders: " + dirs.Length);
                //Print("Number of files: " + files.Length);

                //Copy over all the files
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(replace, file.Name);
                    file.CopyTo(tempPath, false);

                    Invoke(new Action(() =>
                    {
                        fileInfoLabel.Text = "File: " + Path.GetFileName(tempPath);

                        progressBar1.Value += 1;
                        progressBar2.Value += 1;

                        actualFiles--;
                    }));

                }

                //sub folders
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(replace, subdir.Name);
                    Directory.CreateDirectory(tempPath);

                    ConvergeFiles(subdir.FullName, tempPath);
                }

                Invoke(new Action(() =>
                {
                    progressBar1.Value = progressBar1.Maximum;
                }));

                //The program has stopped
                if (actualFiles == 0)
                {
                    Print("Done!");
                    workin = false;

                    _dateLastRan = DateTime.Today;

                    Reset();
                }
            }
            else
            {
                Invoke(new Action(() =>
                {
                    Print("Error");

                    Print(CheckDirectory(replace).ToString() + "-" + " Replace");
                    Print(CheckDirectory(upload).ToString() + "-" + " Upload");
                }));
            }
        }
        #endregion

        public void button1_Click(object sender = null, EventArgs e = null)
        {
            int count = 0;
            int limit = _folderLimit + 5;
            string newPath = "";
            int folders;

            string replaceNewPath = replacePath;

            //Reset the progress bar
            progressBar1.Value = 0;
            progressBar2.Value = 0;

            actualFiles = 0;

            uploadPath = uploadLocationString.Text;
            replacePath = replaceLocationString.Text;

            //Disable the buttons once the program starts
            EnableButtons(false);

            Print($"Upload Path: {uploadPath}");
            Print($"Replace Path: {replacePath}");

            //Check to make sure both paths exist
            if (CheckDirectory(uploadPath) && CheckDirectory(replacePath))
            {
                workin = true;

                //Count the amount of files
                countFiles(uploadPath);

                progressBar2.Maximum = actualFiles;

                Print($"Files: {actualFiles}");

                if (!overwriteFiles)
                {
                    for (int i = 0; i < limit; i++)
                    {
                        folders = count + 1;

                        if (folders > _folderLimit)
                        {
                            Print("File searching went over the threshold");
                            
                            //Create prompt asking for deletion
                            PromptForm promptForm = new PromptForm();
                            promptForm.text = "You have more backups than your limit. Would you like to remove some?";
                            DialogResult dialogResult = promptForm.ShowDialog();

                            if(dialogResult == DialogResult.OK)
                            {
                                FileSelectionAndDeletionForm FLADF = new FileSelectionAndDeletionForm();
                                FLADF._replacePath = replacePath;
                                FLADF._mainForm = this;
                                FLADF.Show();
                            }

                            promptForm.Dispose();

                            break;
                        }

                        if (CheckDirectory(replaceNewPath + i))
                        {
                            Print($"Replace path: {replaceNewPath + i}");
                            count++;
                        }
                        else
                        {
                            newPath = replaceNewPath += count;
                            Print($"New Path: {newPath}");

                            //Create the new folder
                            Directory.CreateDirectory(newPath);

                            //converge
                            workerThread = new Thread(() => ConvergeFiles(uploadPath, newPath));
                            workerThread.Start();

                            break;
                        }
                    }
                }
                else
                {
                    Print("Deleting folder");

                    //Delete and create a fresh folder
                    DirectoryInfo dir = new DirectoryInfo(replacePath);

                    foreach (FileInfo file in dir.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dirs in dir.GetDirectories())
                    {
                        dirs.Delete(true);
                    }

                    Print("Deleting complete");

                    //overwrite and create new folders
                    Directory.Delete(replacePath);

                    Directory.CreateDirectory(replacePath);

                    //start the thread for the application
                    workerThread = new Thread(() => ConvergeFiles(uploadPath, replacePath));
                    workerThread.Start();
                }

            }
            else
            {
                Print("Error - One or more files do not exist");

                Reset();
            }

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
            //Print the date in the logger
            //Print(dateTimePicker.Text);

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
                if (CheckDirectory(folder))
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
                if (CheckDirectory(folder))
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

            //Print(overwriteFile.ToString());
        }

        private void loggerClearButton_Click(object sender, EventArgs e)
        {
            //reset
            loggerInfo.Text = "";

            progressBar1.Value = 0;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if ((CheckDirectory(uploadPath) && CheckDirectory(replacePath)) && (!uploadPath.Equals("") || !replacePath.Equals("")))
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

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void canclelButton_Click(object sender, EventArgs e)
        {
            if (workin)
            {
                Reset();

                workin = false;

                Print("Aborting upload");
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

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _folderLimit = (int)numericUpDown1.Value;
        }
    }

    [Serializable]
    public class Data
    {
        public List<DateTime> dates;
        public DateTime dateLastRan;

        public bool dailyChecked;
        public bool alreadyRanToday;
        public int folderLimit;
    }
}
