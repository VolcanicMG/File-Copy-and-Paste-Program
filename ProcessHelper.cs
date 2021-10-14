using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace FolderAutoUploader
{
    public class ProcessHelper
    {
        //Main form
        Form1 mainForm;

        //File paths
        public string uploadPath;
        public string replacePath;

        //Options
        public bool overwriteFiles;

        //Threading
        private Thread workerThread;

        //Counting
        public int actualFiles;
        public int folderLimit;

        public bool single;

        //Forms
        ProgressBar progressBar1;
        ProgressBar progressBar2;

        string uploadLocationString;
        string replaceLocationString;

        Label folderInfoLabel;
        Label fileInfoLabel;

        //ID
        public int workerID;

        public ProcessHelper(Form1 _mainForm, bool _single, int _workerID, string _uploadLocationText, string _replaceLocationText, bool _overwriteFils, int _folderLimit,
            ProgressBar _progressBar1 = null, ProgressBar _progressBar2 = null, Label _folderInfoLabel = null, Label _fileInfoLabel = null)
        {
            mainForm = _mainForm;

            overwriteFiles = _overwriteFils;

            progressBar1 = _progressBar1;
            progressBar2 = _progressBar2;

            folderLimit = _folderLimit;

            uploadLocationString = _uploadLocationText;
            replaceLocationString = _replaceLocationText;

            folderInfoLabel = _folderInfoLabel;
            fileInfoLabel = _fileInfoLabel;

            workerID = _workerID;
            single = _single;

            uploadPath = uploadLocationString;
            replacePath = replaceLocationString;

            Form1._processes.Add(this);
        }

        public void FileDeletion()
        {
            //Create prompt asking for deletion
            PromptForm promptForm = new PromptForm();
            promptForm.text = "You have more backups than your limit. Would you like to remove some?";
            DialogResult dialogResult = promptForm.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                FileSelectionAndDeletionForm FLADF = new FileSelectionAndDeletionForm();
                FLADF._replacePath = replacePath;
                FLADF.process = this;
                FLADF.Show();
            }
            else

            {
                mainForm.Reset();
                return;
            }

            promptForm.Dispose();
        }

        //Used to start the copy process
        public void Start()
        {
            Form1.workin = true;

            int count = 0;
            int limit = folderLimit + 5;
            string newPath = "";
            int folders;

            string replaceNewPath = replacePath;

            //Reset the progress bar
            mainForm.Invoke(new Action(() =>
            {
                progressBar1.Value = 0;
                progressBar2.Value = 0;
            }));

            actualFiles = 0;

            //Disable the buttons once the program starts
            mainForm.EnableButtons(false);

            mainForm.Print($"Upload Path: {uploadPath}");
            mainForm.Print($"Replace Path: {replacePath}");

            //Check to make sure both paths exist
            if (CheckDirectory(uploadPath) && CheckDirectory(replacePath) && Form1.workin)
            {
                //Count the amount of files
                countFiles(uploadPath);

                mainForm.Invoke(new Action(() =>
                {
                    progressBar2.Maximum = actualFiles;
                }));

                mainForm.Print($"Files: {actualFiles}");

                if (!overwriteFiles)
                {
                    for (int i = 0; i < limit; i++)
                    {
                        folders = count + 1;

                        if (folders > folderLimit)
                        {
                            mainForm.Print("File searching went over the threshold");

                            //file Deletion
                            FileDeletion();

                            break;
                        }

                        if (CheckDirectory(replaceNewPath + i))
                        {
                            //mainForm.Print($"Replace path: {replaceNewPath + i}");
                            count++;
                        }
                        else
                        {
                            newPath = replaceNewPath += count;
                            mainForm.Print($"New Path: {newPath}");

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
                    mainForm.Print("Deleting folder");

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

                    mainForm.Print("Deleting complete");

                    //overwrite and create new folders
                    Directory.Delete(replacePath);

                    Directory.CreateDirectory(replacePath);

                    ConvergeFiles(uploadPath, replacePath);

                    //start the thread for the application
                    workerThread = new Thread(() => ConvergeFiles(uploadPath, replacePath));
                    workerThread.Start();
                }

            }
            else if(Form1.workin)
            {
                mainForm.Print("Error - One or more folders do not exist");

                mainForm.Reset();

                //if (workerThread != null && workerThread.IsAlive)
                //{
                //    workerThread.Abort();
                //}
            }
        }

        /// <summary>
        /// Used to copy and paste folders
        /// </summary>
        /// <param name="upload"></param>
        /// <param name="replace"></param>
        private void ConvergeFiles(string upload, string replace)
        {
            if (CheckDirectory(replace) && CheckDirectory(upload) && Form1.workin)
            {
                mainForm.Invoke(new Action(() =>
                {
                    progressBar1.Value = 0;
                }));

                mainForm.Invoke(new Action(() =>
                {
                    folderInfoLabel.Text = "Folder: " + replace;
                }));

                DirectoryInfo dir = new DirectoryInfo(upload);
                FileInfo[] files = dir.GetFiles();
                DirectoryInfo[] dirs = dir.GetDirectories();

                //Set the size of the progress bar's max
                if (dirs.Length > 0)
                {
                    mainForm.Invoke(new Action(() =>
                    {
                        progressBar1.Maximum = dirs.Length * files.Length;
                    }));
                }
                else
                {
                    mainForm.Invoke(new Action(() =>
                    {
                        progressBar1.Maximum = files.Length;
                    }));
                }

                //mainForm.Print("Number of folders: " + dirs.Length);
                //mainForm.Print("Number of files: " + files.Length);

                //Copy over all the files
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(replace, file.Name);
                    file.CopyTo(tempPath, false);

                    mainForm.Invoke(new Action(() =>
                    {
                        fileInfoLabel.Text = "File: " + Path.GetFileName(tempPath);

                        progressBar1.Value += 1;
                        progressBar2.Value += 1;
                    }));

                    actualFiles--;
                }

                //sub folders
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(replace, subdir.Name);
                    Directory.CreateDirectory(tempPath);

                    ConvergeFiles(subdir.FullName, tempPath);
                }

                mainForm.Invoke(new Action(() =>
                {
                    progressBar1.Value = progressBar1.Maximum;
                }));

                //The program has stopped
                if (actualFiles == 0)
                {
                    mainForm.Print("Done!");

                    mainForm._dateLastRan = DateTime.Today;

                    mainForm.Reset();

                    //Remove the process from the list and select the next one
                    Form1._processes.Remove(this);

                    //Move on to the next process
                    if (!single)
                    {
                        mainForm.NextProcess();
                    }

                    //remove the thread
                    workerThread.Abort(this);
                }
            }
            else if(!Form1.workin)
            {
                mainForm.Print("Canceled");
            }
            else
            {
                mainForm.Print("Error");

                mainForm.Print(CheckDirectory(replace).ToString() + "-" + " Replace");
                mainForm.Print(CheckDirectory(upload).ToString() + "-" + " Upload");
            }
        }

        /// <summary>
        ///Used to count all the files in a directory including those within sub directories
        /// </summary>
        /// <param name="uploadLoc">The path used to find the directory</param>
        public void countFiles(string uploadLoc)
        {
            if (CheckDirectory(uploadLoc))
            {

                DirectoryInfo dir = new DirectoryInfo(uploadLoc);
                FileInfo[] files = dir.GetFiles();
                DirectoryInfo[] dirs = dir.GetDirectories();

                //Copy over all the files
                foreach (FileInfo file in files)
                {
                    actualFiles++;
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

                mainForm.Print("Error - not found");
                mainForm.Print(CheckDirectory(uploadLoc).ToString() + "-" + " uploadLoc");

            }
        }

        /// <summary>
        /// Used to check the existence of a directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool CheckDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                return true;
            }
            else return false;
        }

    }
}
