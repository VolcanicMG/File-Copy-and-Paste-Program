
using System;

namespace FolderAutoUploader
{
    partial class FileSelectionAndDeletionForm
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

            Invoke(new Action(() =>
            {
                base.Dispose(disposing);
            }));
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileSelectionAndDeletionForm));
            this.folderCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.lastModifedDates = new System.Windows.Forms.RichTextBox();
            this.todaysDate = new System.Windows.Forms.Label();
            this.deletionProgressBar = new System.Windows.Forms.ProgressBar();
            this.failedCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // folderCheckedListBox
            // 
            this.folderCheckedListBox.CheckOnClick = true;
            this.folderCheckedListBox.FormattingEnabled = true;
            this.folderCheckedListBox.Location = new System.Drawing.Point(68, 57);
            this.folderCheckedListBox.Name = "folderCheckedListBox";
            this.folderCheckedListBox.Size = new System.Drawing.Size(292, 214);
            this.folderCheckedListBox.Sorted = true;
            this.folderCheckedListBox.TabIndex = 3;
            this.folderCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.folderCheckedListBox_SelectedIndexChanged);
            // 
            // deleteButton
            // 
            this.deleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteButton.Location = new System.Drawing.Point(131, 367);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(157, 30);
            this.deleteButton.TabIndex = 4;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // backButton
            // 
            this.backButton.Location = new System.Drawing.Point(523, 12);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(60, 39);
            this.backButton.TabIndex = 5;
            this.backButton.Text = "Back";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.Location = new System.Drawing.Point(29, 400);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(533, 18);
            this.statusLabel.TabIndex = 6;
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(113, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(359, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Please select the folders you want to delete";
            // 
            // bindingSource1
            // 
            this.bindingSource1.CurrentChanged += new System.EventHandler(this.bindingSource1_CurrentChanged);
            // 
            // lastModifedDates
            // 
            this.lastModifedDates.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lastModifedDates.Location = new System.Drawing.Point(367, 57);
            this.lastModifedDates.Name = "lastModifedDates";
            this.lastModifedDates.ReadOnly = true;
            this.lastModifedDates.Size = new System.Drawing.Size(140, 217);
            this.lastModifedDates.TabIndex = 8;
            this.lastModifedDates.Text = "";
            // 
            // todaysDate
            // 
            this.todaysDate.AutoSize = true;
            this.todaysDate.Location = new System.Drawing.Point(396, 277);
            this.todaysDate.Name = "todaysDate";
            this.todaysDate.Size = new System.Drawing.Size(76, 13);
            this.todaysDate.TabIndex = 9;
            this.todaysDate.Text = "Today\'s Date: ";
            this.todaysDate.Click += new System.EventHandler(this.label2_Click);
            // 
            // deletionProgressBar
            // 
            this.deletionProgressBar.Location = new System.Drawing.Point(68, 421);
            this.deletionProgressBar.Name = "deletionProgressBar";
            this.deletionProgressBar.Size = new System.Drawing.Size(436, 17);
            this.deletionProgressBar.TabIndex = 10;
            this.deletionProgressBar.Visible = false;
            // 
            // failedCheckedListBox
            // 
            this.failedCheckedListBox.CheckOnClick = true;
            this.failedCheckedListBox.FormattingEnabled = true;
            this.failedCheckedListBox.Location = new System.Drawing.Point(68, 297);
            this.failedCheckedListBox.Name = "failedCheckedListBox";
            this.failedCheckedListBox.Size = new System.Drawing.Size(292, 64);
            this.failedCheckedListBox.Sorted = true;
            this.failedCheckedListBox.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(65, 277);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Failed Folders";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(366, 297);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(196, 52);
            this.label3.TabIndex = 13;
            this.label3.Text = "Must select one of the non-failed\r\n folders in order to delete the\r\n failed ones." +
    " (Won\'t ask to delete\r\n when selected)";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // FileSelectionAndDeletionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 450);
            this.ControlBox = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.failedCheckedListBox);
            this.Controls.Add(this.deletionProgressBar);
            this.Controls.Add(this.todaysDate);
            this.Controls.Add(this.lastModifedDates);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.folderCheckedListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FileSelectionAndDeletionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Selection and Deletion";
            this.Load += new System.EventHandler(this.FileSelectionAndDeletionForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckedListBox folderCheckedListBox;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.RichTextBox lastModifedDates;
        private System.Windows.Forms.Label todaysDate;
        private System.Windows.Forms.ProgressBar deletionProgressBar;
        private System.Windows.Forms.CheckedListBox failedCheckedListBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}