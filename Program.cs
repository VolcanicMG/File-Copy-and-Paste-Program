using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace FolderAutoUploader
{
    static class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            //Use a double try/catch to make sure the program does not crash and so we can use the finally statement
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                try
                {

                    key.SetValue("Auto_Copy/Paste_Program", "\"" + Application.ExecutablePath + "\"");
                }
                finally
                {
                    key.Dispose();
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
