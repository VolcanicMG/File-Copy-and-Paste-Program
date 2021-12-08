
namespace FolderAutoUploader
{
    partial class AddLocationsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddLocationsForm));
            this.AddLocation = new System.Windows.Forms.Button();
            this.uploadLocationString = new System.Windows.Forms.TextBox();
            this.replaceLocationString = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.searchButton1 = new System.Windows.Forms.Button();
            this.searchButton2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.LocationNameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AddLocation
            // 
            this.AddLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddLocation.Location = new System.Drawing.Point(149, 127);
            this.AddLocation.Name = "AddLocation";
            this.AddLocation.Size = new System.Drawing.Size(169, 40);
            this.AddLocation.TabIndex = 0;
            this.AddLocation.Text = "Add Location";
            this.AddLocation.UseVisualStyleBackColor = true;
            this.AddLocation.Click += new System.EventHandler(this.AddLocation_Click);
            // 
            // uploadLocationString
            // 
            this.uploadLocationString.Location = new System.Drawing.Point(119, 75);
            this.uploadLocationString.Name = "uploadLocationString";
            this.uploadLocationString.Size = new System.Drawing.Size(224, 20);
            this.uploadLocationString.TabIndex = 17;
            // 
            // replaceLocationString
            // 
            this.replaceLocationString.Location = new System.Drawing.Point(119, 101);
            this.replaceLocationString.Name = "replaceLocationString";
            this.replaceLocationString.Size = new System.Drawing.Size(224, 20);
            this.replaceLocationString.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Copy Location";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Copy To/Replace";
            // 
            // searchButton1
            // 
            this.searchButton1.Location = new System.Drawing.Point(349, 75);
            this.searchButton1.Name = "searchButton1";
            this.searchButton1.Size = new System.Drawing.Size(101, 20);
            this.searchButton1.TabIndex = 21;
            this.searchButton1.Text = "Find";
            this.searchButton1.UseVisualStyleBackColor = true;
            this.searchButton1.Click += new System.EventHandler(this.searchButton1_Click);
            // 
            // searchButton2
            // 
            this.searchButton2.Location = new System.Drawing.Point(349, 100);
            this.searchButton2.Name = "searchButton2";
            this.searchButton2.Size = new System.Drawing.Size(101, 21);
            this.searchButton2.TabIndex = 22;
            this.searchButton2.Text = "Find";
            this.searchButton2.UseVisualStyleBackColor = true;
            this.searchButton2.Click += new System.EventHandler(this.searchButton2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(159, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 25);
            this.label1.TabIndex = 23;
            this.label1.Text = "Set Location";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LocationNameTextBox
            // 
            this.LocationNameTextBox.Location = new System.Drawing.Point(119, 49);
            this.LocationNameTextBox.Name = "LocationNameTextBox";
            this.LocationNameTextBox.Size = new System.Drawing.Size(224, 20);
            this.LocationNameTextBox.TabIndex = 24;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Location Name";
            // 
            // AddLocationsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 183);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.LocationNameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.uploadLocationString);
            this.Controls.Add(this.replaceLocationString);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.searchButton1);
            this.Controls.Add(this.searchButton2);
            this.Controls.Add(this.AddLocation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddLocationsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AddLocationsForm";
            this.Load += new System.EventHandler(this.AddLocationsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AddLocation;
        private System.Windows.Forms.TextBox uploadLocationString;
        private System.Windows.Forms.TextBox replaceLocationString;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button searchButton1;
        private System.Windows.Forms.Button searchButton2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox LocationNameTextBox;
        private System.Windows.Forms.Label label4;
    }
}