using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderAutoUploaderMain
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

        }
    }

    //Try and find another way to get the exe path dynamicly
    public static class EXEPaths
    {
        public static string mainEXEPath = Application.StartupPath + @"\" + "FolderAutoUploader.exe";
        public static string backgroundEXEPath = Application.StartupPath + @"\" + "FolderAutoUpdaterBackgroundProcess.exe";
    }

    public static class Info
    {
        public static string folderNameDates = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string datesFileLocation = folderNameDates + @"\CopyPasteData\dates.bin";

        //Get the path for the locations 
        public static string folderName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string saveFileLocation = folderName + @"\CopyPasteData\locationSaves.txt";

        public static string failedFilesLocation = folderName + @"\CopyPasteData\FailedFiles.txt";
    }

    //The class we use to save information
    [Serializable]
    public class Data
    {
        public List<DateTime> dates;
        public List<LocationsAdd> scheduleLocations;
        public DateTime dateLastRan;

        public bool alreadyRanToday;
        public bool alreadyRanThisWeek;
        public bool alreadyRanThisMonth;

        public DateTime weeklyTime;
        public DateTime monthlyTime;
        public DateTime dailyTime;

        public int folderLimit;

        //Settings
        public bool loadOnStartup;
        public bool runFromBackground;
        public bool autoDelete;

        public checkCycle checktype;
        
        public enum checkCycle
        { 
            notChecked,
            dailyChecked,
            weeklyChecked,
            monthlyChecked
        }

    }

    [Serializable]
    public class LocationsAdd
    {
        public string name;

        public string copy;
        public string replace;

        public bool selected;
    }
}
