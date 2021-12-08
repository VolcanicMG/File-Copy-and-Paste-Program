using System;
using System.Collections.Generic;
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

        private string newReplacePath;

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
                FLADF._replacePath = newReplacePath; //Needs to use the new replace path so it gets the dir ex. home/so (We select home but we need to get into so)
                FLADF.process = this;
                FLADF._mainForm = mainForm;
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

            int count = 1;
            string newPath = "";

            //The list used to get the amount of files 
            List<string> folderList = new List<string>();

            //instantate the list
            for (int i = 0; i < 50; i++)
            {
                folderList.Add("");
            }

            if (!uploadPath.Equals("") && uploadPath != null)
            {
                DirectoryInfo di = new DirectoryInfo(uploadPath);
                newReplacePath = replacePath + @"\" + di.Name;
            }

            string replaceNewPath = newReplacePath;

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
                actualFiles = countFiles(uploadPath, mainForm);

                mainForm.Invoke(new Action(() =>
                {
                    progressBar2.Maximum = actualFiles;
                }));

                mainForm.Print($"Files: {actualFiles}");

                if (!overwriteFiles)
                {
                    //Goes and checks to see how many folders we have under the same name
                    int actualNumber = 1;
                    for (int i = 1; i <= 50; i++)
                    {
                        if (CheckDirectory(replaceNewPath + i))
                        {
                            folderList.Insert(i, replaceNewPath + i);
                            actualNumber++;
                        }
                    }

                    //Check to see if we went over the folder limit, if so delete the folders till we sit within the min and max limit
                    if (actualNumber > folderLimit)
                    {
                        mainForm.Print("File searching went over the threshold");

                        //file Deletion
                        FileDeletion();

                        //return to make sure we get the correct amount deleted
                        return;
                    }

                    //If we have folders already run the if, else create the first one
                    if (folderList.Count > 0)
                    {
                        //We did not hit the limit and are good to go to make more copies
                        for (int i = 1; i <= 50; i++)
                        {
                            if (folderList[i] != null && !folderList[i].Equals("") && CheckDirectory(folderList[i]))
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
                        newPath = replaceNewPath += count;
                        mainForm.Print($"New Path: {newPath}");

                        //Create the new folder
                        Directory.CreateDirectory(newPath);

                        //converge
                        workerThread = new Thread(() => ConvergeFiles(uploadPath, newPath));
                        workerThread.Start();
                    }
                }
                else
                {

                    //Create prompt asking for deletion
                    PromptForm promptForm = new PromptForm();
                    promptForm.text = "Are you sure you want to overwrite the folder? (Will delete and replace the folder)";
                    DialogResult dialogResult = promptForm.ShowDialog();

                    if (dialogResult == DialogResult.OK)
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
                    else

                    {
                        mainForm.Reset();
                        return;
                    }

                    promptForm.Dispose();


                }

            }
            else if (Form1.workin)
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
            mainForm.Invoke(new Action(() =>
            {
                mainForm.cancelButton.Enabled = true;
            }));

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

                    //Used to make sure the last file is done when canceled it pressed
                    if (!Form1.workin)
                    {
                        mainForm.Reset();

                        return;
                    }
                }

                //sub folders
                foreach (DirectoryInfo subdir in dirs)
                {
                    if (Form1.workin)
                    {
                        string tempPath = Path.Combine(replace, subdir.Name);
                        Directory.CreateDirectory(tempPath);

                        ConvergeFiles(subdir.FullName, tempPath);
                    }
                    else return;
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
            else if (!Form1.workin)
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
        public static int countFiles(string uploadLoc, Form1 mainForm)
        {
            int totalFiles = 0;

            if (CheckDirectory(uploadLoc))
            {

                DirectoryInfo dir = new DirectoryInfo(uploadLoc);
                FileInfo[] files = dir.GetFiles();
                DirectoryInfo[] dirs = dir.GetDirectories();

                //Copy over all the files
                foreach (FileInfo file in files)
                {
                    totalFiles++;
                }

                //sub folders
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(uploadLoc, subdir.Name);

                    totalFiles += countFiles(subdir.FullName, mainForm);
                }

                return totalFiles;
            }
            else
            {
                mainForm.Print("Error - not found");
                mainForm.Print(CheckDirectory(uploadLoc).ToString() + "-" + " uploadLoc");

                return 0;
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
