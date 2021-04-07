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
    public partial class PromptForm : Form
    {
        public string text;
        public PromptForm()
        {
            InitializeComponent();
        }

        private void PromptForm_Load(object sender, EventArgs e)
        {
            label1.Text = text;
        }

        private void okButton_Click(object sender, EventArgs e)
        {

        }
    }
}
