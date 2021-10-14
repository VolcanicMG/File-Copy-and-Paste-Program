using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace FolderAutoUploader
{
    public partial class FileSelectionAndDeletionForm : Form
    {
        public string _replacePath;
        public List<string> backupFolders = new List<string>();
        public List<string> deleationDates = new List<string>();

        //public Form1 _mainForm;
        public ProcessHelper process;

        public FileSelectionAndDeletionForm()
        {
            InitializeComponent();
        }

        private void FileSelectionAndDeletionForm_Load(object sender, EventArgs e)
        {
            int number = 1;

            CheckFolders();

            foreach (string folderPath in backupFolders)
            {
                folderCheckedListBox.Items.Add(folderPath);
            }

            for (int i = 0; i < deleationDates.Count; i++)
            {
                LastModifedDates.Text += deleationDates[i] + "\n";
            }
        }

        void CheckFolders()
        {
            string replaceNewPath = _replacePath;

            for (int i = 0; i < Form1._folderLimit; i++) 
            {
                if (Directory.Exists(replaceNewPath + i))
                {
                    backupFolders.Add(replaceNewPath + i);

                    DateTime dt = File.GetLastWriteTimeUtc(replaceNewPath + i);
                    deleationDates.Add(String.Format("{0:M/d/yyyy}", dt)); //get the last time the file was modified
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (folderCheckedListBox.CheckedItems.Count > 0)
            {
                statusLabel.Text = "Deleting in progress... (Might take a few minutes depending on folder size)";

                PromptForm promptForm = new PromptForm();
                promptForm.text = "Are you sure you want to delete the following?";
                DialogResult dialogResult = promptForm.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {

                    foreach (object path in folderCheckedListBox.CheckedItems)
                    {
                        if (Directory.Exists(path.ToString()))
                        {
                            DeleteFolder(path.ToString());
                        }
                    }

                    Thread.Sleep(10);
                    process.Start();

                    //When the process ends dispose of the popup
                    this.Dispose();
                }
                else statusLabel.Text = "";

                promptForm.Dispose();
            }

        }

        private void DeleteFolder(string path)
        {
            //Delete
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dirs in dir.GetDirectories())
            {
                dirs.Delete(true);
            }

            Directory.Delete(path);
        }

        private void backButton_Click(object sender, EventArgs e)
        {

            this.Dispose();
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}
