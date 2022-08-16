using System;
using System.Threading;
using System.Windows.Forms;
using FolderAutoUploaderMain;

namespace FolderAutoUpdaterBackgroundProcess
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

            //Check to make sure we can start
            if (args.Length > 0 && args[0].Equals("-Start"))
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

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(new BackgroundProcess());
            }
            else
            {
                return;
            }
        }
    }
}
