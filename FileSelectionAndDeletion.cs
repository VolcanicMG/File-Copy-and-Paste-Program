using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderAutoUploader
{
    public partial class FileSelectionAndDeletionForm : Form
    {
        public string _replacePath;
        public List<string> backupFolders = new List<string>();
        public Form1 _mainForm;

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
        }

        void CheckFolders()
        {
            string replaceNewPath = _replacePath;
           
            for (int i = 0;  i < 15; i++)
            {
                if (Directory.Exists(replaceNewPath + i))
                {
                    backupFolders.Add(replaceNewPath + i);
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

                    Thread.Sleep(1000);
                    _mainForm.button1_Click();
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
    }
}
