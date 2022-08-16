using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace FolderAutoUploader
{
    public partial class FileSelectionAndDeletionForm : Form
    {
        public string _replacePath;
        public List<string> backupFolders = new List<string>();
        public List<string> failedFolder = new List<string>();
        public List<DateTime> deleationDates = new List<DateTime>();

        public Form1 _mainForm;
        public ProcessHelper process;

        private int numOfFolders;

        private Thread workerThread;

        private int oldestPreselect;

        public FileSelectionAndDeletionForm()
        {
            InitializeComponent();
        }

        private void FileSelectionAndDeletionForm_Load(object sender, EventArgs e)
        {
            CheckFolders();

            //The lastest date
            var latestDate = deleationDates.OrderBy(x => x.Date).FirstOrDefault();

            //Adding the fodlers to the checked box list
            foreach (string folderPath in backupFolders)
            {
                folderCheckedListBox.Items.Add(folderPath);
            }

            //Adding the failed folders to the checked box list
            foreach (string folderPath in failedFolder)
            {
                failedCheckedListBox.Items.Add(folderPath);
            }

            for (int i = 0; i < deleationDates.Count; i++)
            {

                string date = String.Format("{0:M/d/yyyy}", deleationDates[i]);

                if (deleationDates[i] == latestDate)
                {
                    date += " *Oldest";

                    oldestPreselect = i;
                }

                lastModifedDates.Text += date + "\n";
            }

            //Select the oldest in the list
            folderCheckedListBox.SelectedIndex = oldestPreselect;
            folderCheckedListBox.SetItemChecked(oldestPreselect, true);

            todaysDate.Text += String.Format("{0:M/d/yyyy}", DateTime.Now.Date);

            numOfFolders = 0;

            deletionProgressBar.Value = 0;
            deletionProgressBar.Minimum = numOfFolders; //starts at 0
            deletionProgressBar.Visible = false;

            //Start the deletion if its set to auto delete
            if (_mainForm._autoDelete) button1_Click();
        }

        void CheckFolders()
        {
            string replaceNewPath = _replacePath;

            for (int i = 0; i <= 50; i++) //Set to 50 because thats the hard limit set
            {
                //Add the main folders
                if (Directory.Exists(replaceNewPath + i))
                {
                    backupFolders.Add(replaceNewPath + i);

                    DateTime dt = File.GetLastWriteTime(replaceNewPath + i);
                    deleationDates.Add(dt); //get the last time the file was modified

                    if (Directory.Exists(replaceNewPath + i + "(Failed)"))
                    {
                        failedFolder.Add(replaceNewPath + i + "(Failed)");
                    }
                }

                //Add the failed folders
                for (int j = 0; j <= 50; j++)
                {
                    //Add the failed folders
                    if (Directory.Exists(replaceNewPath + i + "(Failed)" + j))
                    {
                        failedFolder.Add(replaceNewPath + i + "(Failed)" + j);
                    }
                }

            }
        }

        private void button1_Click(object sender = null, EventArgs e = null)
        {
            if (folderCheckedListBox.CheckedItems.Count > 0)
            {
                EnableButtons(false);

                workerThread = new Thread(() => StartDeletion());
                workerThread.Start();
            }
            else
            {
                PromptForm promptForm = new PromptForm();
                promptForm.text = "You have nothing selected";
                promptForm.buttons = false;
                DialogResult dialogResult = promptForm.ShowDialog();

                return;
            }
        }

        private void StartDeletion()
        {
            if (folderCheckedListBox.CheckedItems.Count > 0)
            {
                Invoke(new Action(() =>
                {
                    statusLabel.Text = "Deleting in progress... (Might take a few minutes depending on folder size and how many you delete)";
                }));

                Invoke(new Action(() =>
                {
                    deletionProgressBar.Visible = true;
                }));

                //Check to see if the auto-deletion is true
                if (!_mainForm._autoDelete) ConfirmPopup();
                else
                {
                    //count all the folders
                    foreach (object path in folderCheckedListBox.CheckedItems)
                    {
                        if (Directory.Exists(path.ToString()))
                        {
                            DirectoryInfo dir = new DirectoryInfo(path.ToString());

                            foreach (DirectoryInfo dirs in dir.GetDirectories())
                            {
                                Invoke(new Action(() =>
                                {
                                    numOfFolders++;
                                }));

                            }
                        }
                    }

                    //Set the num of files
                    Invoke(new Action(() =>
                    {
                        _mainForm.Print("Number of Folders: " + numOfFolders);
                        deletionProgressBar.Maximum = numOfFolders;
                    }));

                    //delete all the files
                    foreach (object path in folderCheckedListBox.CheckedItems)
                    {
                        if (Directory.Exists(path.ToString()))
                        {
                            DeleteFolder(path.ToString());
                        }
                    }

                    //_mainForm.Print("Deletion bar value" + deletionProgressBar.Value);

                    Thread.Sleep(10);

                    process.Start();

                    //When the process ends dispose of the popup
                    this.Dispose();
                }
            }
        }

        private void ConfirmPopup()
        {
            PromptForm promptForm = new PromptForm();
            promptForm.text = "Are you sure you want to delete the following?";

            foreach (object path in folderCheckedListBox.CheckedItems)
            {
                Invoke(new Action(() =>
                {
                    promptForm.text += "\n" + path.ToString();
                }));
            }

            DialogResult dialogResult = promptForm.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {

                //count all the folders
                foreach (object path in folderCheckedListBox.CheckedItems)
                {
                    if (Directory.Exists(path.ToString()))
                    {
                        DirectoryInfo dir = new DirectoryInfo(path.ToString());

                        foreach (DirectoryInfo dirs in dir.GetDirectories())
                        {
                            Invoke(new Action(() =>
                            {
                                numOfFolders++;
                            }));

                        }
                    }
                }

                //Count all the failed folders
                if (failedCheckedListBox.CheckedItems.Count > 0)
                {
                    foreach (object path in failedCheckedListBox.CheckedItems)
                    {
                        if (Directory.Exists(path.ToString()))
                        {
                            DirectoryInfo dir = new DirectoryInfo(path.ToString());

                            foreach (DirectoryInfo dirs in dir.GetDirectories())
                            {
                                Invoke(new Action(() =>
                                {
                                    numOfFolders++;
                                }));

                            }
                        }
                    }
                }

                //Set the num of files
                Invoke(new Action(() =>
                {
                    _mainForm.Print("Number of Folders: " + numOfFolders);
                    deletionProgressBar.Maximum = numOfFolders;
                }));

                //delete all the files
                foreach (object path in folderCheckedListBox.CheckedItems)
                {
                    if (Directory.Exists(path.ToString()))
                    {
                        DeleteFolder(path.ToString());
                    }
                }

                //Delete all the failed ones
                if (failedCheckedListBox.CheckedItems.Count > 0)
                {
                    foreach (object path in failedCheckedListBox.CheckedItems)
                    {
                        if (Directory.Exists(path.ToString()))
                        {
                            DeleteFolder(path.ToString());
                        }
                    }
                }

                //_mainForm.Print("Deletion bar value" + deletionProgressBar.Value);

                Thread.Sleep(10);

                process.Start();

                //When the process ends dispose of the popup
                this.Dispose();
            }
            else
            {
                Invoke(new Action(() =>
                {
                    statusLabel.Text = "";

                    EnableButtons(true);
                }));
            }

            promptForm.Dispose();
        }

        private void DeleteFolder(string path)
        {
            //Delete
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (DirectoryInfo dirs in dir.GetDirectories())
            {
                Invoke(new Action(() =>
                {
                    deletionProgressBar.Value++;
                }));

                dirs.Delete(true);
            }

            Directory.Delete(path, true);
        }

        private void EnableButtons(bool enabled)
        {

            deleteButton.Enabled = enabled;
            folderCheckedListBox.Enabled = enabled;
            backButton.Enabled = enabled;
            failedCheckedListBox.Enabled = enabled;

        }

        private void backButton_Click(object sender, EventArgs e)
        {
            _mainForm.Invoke(new Action(() =>
            {
                _mainForm.Reset();
            }));

            this.Dispose();
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void folderCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
