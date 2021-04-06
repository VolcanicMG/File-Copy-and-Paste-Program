using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderAutoUploader
{
    static class Program
    {
        static RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            try
            {
                key.SetValue("Auto_Copy/Paste_Program", "\"" + Application.ExecutablePath + "\"");
            }
            finally
            {
                key.Dispose();
            }
        }
    }
}
