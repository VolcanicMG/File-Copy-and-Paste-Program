using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace FolderAutoUploader
{
    static class Program
    {
        public static Mutex mutex = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string appName = Application.ProductName;
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);

            //Need some way to stop the pre existing program without killing it from here when wanting to open the program from the desktop.*****

            if (!createdNew)
            {
                //app is already running! Exiting the application
                return;
            }

            //Delete all processes that are running the background process
            Process[] workers = Process.GetProcessesByName("FolderAutoUpdaterBackgroundProcess");
            foreach (Process worker in workers)
            {
                worker.Kill();
                worker.WaitForExit();
                worker.Dispose();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if(args.Length > 0 && args[0].Equals("-StartMinimizedFromBackground"))
            {
                Application.Run(new Form1(true));
            }
            else
            {
                Application.Run(new Form1(false));
            }

        }
    }
}

