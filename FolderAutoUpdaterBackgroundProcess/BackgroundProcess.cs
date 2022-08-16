using FolderAutoUploaderMain;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace FolderAutoUpdaterBackgroundProcess
{
    public partial class BackgroundProcess : Form
    {
        private Data infoData;

        private DateTime dailyTime;

        //Icon menu
        private ContextMenu trayMenu;

        //Used to exit then open
        private bool open;

        private Data.checkCycle _checkType;
        public bool _alreadyRanToday;
        public bool _alreadyRanThisWeek;
        public bool _alreadyRanThisMonth;

        private DateTime _weeklyTime;
        private DateTime _monthlyTime;
        private DateTime _dailyTime;

        //The timer used for this class
        public System.Windows.Forms.Timer timer;

        //The data from the folder we are accessing
        public Data dateData;

        public DateTime _dateLastRan;

        public List<DateTime> dates = new List<DateTime>();

        //Settings
        public bool _loadOnStartup;
        public bool _runFromBackground;

        public BackgroundProcess()
        {
            InitializeComponent();
        }

        private void BackgroundProcess_Load(object sender, EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;

            // Load in the dates file and check it
            if (File.Exists(Info.datesFileLocation))
            {
                IFormatter formatter = new BinaryFormatter();

                Stream stream = new FileStream(Info.datesFileLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
                infoData = (Data)formatter.Deserialize(stream);
                stream.Close();

                dates = infoData.dates;
                _checkType = infoData.checktype;
                _alreadyRanToday = infoData.alreadyRanToday;
                _alreadyRanThisWeek = infoData.alreadyRanThisWeek;
                _alreadyRanThisMonth = infoData.alreadyRanThisMonth;
                _weeklyTime = infoData.weeklyTime;
                _monthlyTime = infoData.monthlyTime;
                _dateLastRan = infoData.dateLastRan;
                _loadOnStartup = infoData.loadOnStartup;
                _runFromBackground = infoData.runFromBackground;
                _dailyTime = infoData.dailyTime;
            }

            //Menu
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Open", OnOpen);
            trayMenu.MenuItems.Add("Exit", OnExit);

            trayIcon.Text = "Folder Copy/Paste";
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            StartTimer();
        }

        private void TrayIcon_Click(object sender, EventArgs e)
        {
            OnOpen();
        }

        private void OnOpen(object sender = null, EventArgs e = null)
        {
            open = true;

            OnExit();
        }

        private void OnExit(object sender = null, EventArgs e = null)
        {
            //Check to make sure the timer is set up if not ignore it
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }

            BinaryFormatter formatter = new BinaryFormatter();

            infoData.dates = dates;
            infoData.weeklyTime = _weeklyTime;
            infoData.monthlyTime = _monthlyTime;
            infoData.dailyTime = _dailyTime;

            using (FileStream stream = File.OpenWrite(Info.datesFileLocation))
            {
                formatter.Serialize(stream, infoData);
                stream.Close();
            }

            //Close the application
            Application.Exit();

            if (open)
            {
                Process.Start(EXEPaths.mainEXEPath);
            }
        }

        private void StartTimer()
        {
            if (_checkType != Data.checkCycle.notChecked || dates.Count > 0)
            {
                timer = new System.Windows.Forms.Timer();
                timer.Tick += Timer_Tick;
                timer.Interval = (60) * 60000; //(x) represents minutes
                //timer.Interval = (10) * 1000; //(x) represents seconds
                timer.Enabled = true;
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now; //The date and time for right now

            switch (_checkType)
            {
                case Data.checkCycle.dailyChecked:
                    if (_dailyTime <= now && !_alreadyRanToday) //Need some kind of check also for each day
                    {
                        RunPopup();
                        _dailyTime = now.AddDays(1);
                    }
                    break;

                case Data.checkCycle.weeklyChecked:
                    if (_weeklyTime <= now && !_alreadyRanThisWeek) 
                    {
                        RunPopup();
                        _weeklyTime = now.AddDays(7);
                    }
                    break;

                case Data.checkCycle.monthlyChecked:
                    if (_monthlyTime <= now && !_alreadyRanThisMonth) 
                    {
                        RunPopup();
                        _monthlyTime = now.AddMonths(1);
                    }
                    break;

                case Data.checkCycle.notChecked:
                    CheckDate(now);
                    break;

                default:
                    timer.Stop();
                    timer.Dispose();

                    OnExit();
                    break;
            }

            // Listen to notification activation
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                OnOpen();
            };

        }

        public void CheckDate(DateTime timeToCheck)
        {
            //Check each of our dates
            if (dates.Count > 0)
            {
                foreach (DateTime date in dates)
                {
                    if (timeToCheck >= date)
                    {
                        RunPopup();
                        dates.Remove(date);
                    }
                }
            }
        }

        public void RunPopup()
        {
            new ToastContentBuilder()
                .AddText("Time to start copy paste")
                .AddAudio(new Uri("ms-appx:///Sound.mp3"))
                .SetToastScenario(ToastScenario.Alarm)
                .Show();

            //If the user has the following setting set up we will run the program minimized when the timer is triggered
            if(_runFromBackground)
            {
                Process.Start(EXEPaths.mainEXEPath, "-StartMinimizedFromBackground");
            }
        }
    }
}
