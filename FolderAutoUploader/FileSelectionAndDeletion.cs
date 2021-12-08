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
        public List<DateTime> deleationDates = new List<DateTime>();

        public Form1 _mainForm;
        public ProcessHelper process;

        private int numOfFolders;

        private Thread workerThread;

        public FileSelectionAndDeletionForm()
        {
            InitializeComponent();
        }

        private void FileSelectionAndDeletionForm_Load(object sender, EventArgs e)
        {
            CheckFolders();

            foreach (string folderPath in backupFolders)
            {
                folderCheckedListBox.Items.Add(folderPath);
            }

            for (int i = 0; i < deleationDates.Count; i++)
            {
                string date = String.Format("{0:M/d/yyyy}", deleationDates[i]);

                LastModifedDates.Text += date + "\n";
            }

            var latestDate = deleationDates.OrderBy(x => x.Date).FirstOrDefault();

            LastestDate.Text += String.Format("{0:M/d/yyyy}", latestDate);

            numOfFolders = 0;

            deletionProgressBar.Value = 0;
            deletionProgressBar.Minimum = numOfFolders; //starts at 0
            deletionProgressBar.Visible = false;
        }

        void CheckFolders()
        {
            string replaceNewPath = _replacePath;

            for (int i = 0; i <= 50; i++) //Set to 50 because thats the hard limit set
            {
                if (Directory.Exists(replaceNewPath + i))
                {
                    backupFolders.Add(replaceNewPath + i);

                    DateTime dt = File.GetLastWriteTimeUtc(replaceNewPath + i);
                    deleationDates.Add(dt); //get the last time the file was modified
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EnableButtons(false);

            workerThread = new Thread(() => StartDeletion());
            workerThread.Start();
        }

        private void StartDeletion()
        {
            if (folderCheckedListBox.CheckedItems.Count > 0)
            {
                Invoke(new Action(() =>
                {
                    statusLabel.Text = "Deleting in progress... (Might take a few minutes depending on folder size)";
                }));

                PromptForm promptForm = new PromptForm();
                promptForm.text = "Are you sure you want to delete the following?";

                Invoke(new Action(() =>
                {
                    deletionProgressBar.Visible = true;
                }));

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

        }

        private void backButton_Click(object sender, EventArgs e)
        {
            _mainForm.Reset();

            this.Dispose();
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
