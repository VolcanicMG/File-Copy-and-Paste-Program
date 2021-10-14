using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderAutoUploader
{
    public partial class AddLocationsForm : Form
    {
        public string locationCopy;
        public string locationReplace;

        public string locationName;

        public AddLocationsForm()
        {
            InitializeComponent();
        }

        private void AddLocationsForm_Load(object sender, EventArgs e)
        {
            locationCopy = "";
            locationReplace = "";
        }

        private void AddLocation_Click(object sender, EventArgs e)
        {
            locationCopy = uploadLocationString.Text;
            locationReplace = replaceLocationString.Text;
            locationName = LocationNameTextBox.Text;

            //Check to make sure that both directories are correct and valid
            if (ProcessHelper.CheckDirectory(locationCopy) && ProcessHelper.CheckDirectory(locationReplace) && (locationName != null || locationName != ""))
            {
                //Set the locations in a class
                LocationsAdd newLocationAdd = new LocationsAdd();
                newLocationAdd.name = locationName;
                newLocationAdd.copy = locationCopy;
                newLocationAdd.replace = locationReplace;
                newLocationAdd.selected = true;

                if(Form1._scheduleLocations.Exists(x => x.name == newLocationAdd.name))
                {
                    return;
                }
                else
                {
                    //add it
                    Form1._scheduleLocations.Add(newLocationAdd);

                    this.Dispose();
                }

            }
            else
            {

            }
        }

        private void searchButton1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string folder = folderBrowserDialog1.SelectedPath;

                //check to make sure the directory exists
                if (ProcessHelper.CheckDirectory(folder))
                {
                    uploadLocationString.Text = folder;
                }
            }
        }

        private void searchButton2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string folder = folderBrowserDialog1.SelectedPath;

                //check to make sure the directory exists
                if (ProcessHelper.CheckDirectory(folder))
                {
                    replaceLocationString.Text = folder;
                }
            }
        }
    }
}
   
