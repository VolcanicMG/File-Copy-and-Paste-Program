using System;
using System.Windows.Forms;

namespace FolderAutoUploader
{
    public partial class PromptForm : Form
    {
        public string text;
        public bool buttons = true;
        public PromptForm()
        {
            InitializeComponent();
        }

        private void PromptForm_Load(object sender, EventArgs e)
        {
            label1.Text = text;

            if(!buttons)
            {
                cancelButton.Enabled = false;
                okButton.Enabled = false;

                ControlBox = true;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {

        }
    }
}
