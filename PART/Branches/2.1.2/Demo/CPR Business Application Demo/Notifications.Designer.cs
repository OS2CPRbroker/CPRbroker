﻿namespace CPR_Business_Application_Demo
{
    partial class NotificationsForm
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
            this.NotificationsText = new System.Windows.Forms.TextBox();
            this.ClearNotificationsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NotificationsText
            // 
            this.NotificationsText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.NotificationsText.Enabled = false;
            this.NotificationsText.Location = new System.Drawing.Point(0, 0);
            this.NotificationsText.Multiline = true;
            this.NotificationsText.Name = "NotificationsText";
            this.NotificationsText.Size = new System.Drawing.Size(897, 352);
            this.NotificationsText.TabIndex = 0;
            // 
            // ClearNotificationsButton
            // 
            this.ClearNotificationsButton.Location = new System.Drawing.Point(9, 362);
            this.ClearNotificationsButton.Name = "ClearNotificationsButton";
            this.ClearNotificationsButton.Size = new System.Drawing.Size(75, 23);
            this.ClearNotificationsButton.TabIndex = 1;
            this.ClearNotificationsButton.Text = "&Clear";
            this.ClearNotificationsButton.UseVisualStyleBackColor = true;
            this.ClearNotificationsButton.Click += new System.EventHandler(this.ClearNotificationsButton_Click);
            // 
            // NotificationsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 392);
            this.Controls.Add(this.ClearNotificationsButton);
            this.Controls.Add(this.NotificationsText);
            this.Name = "NotificationsForm";
            this.Text = "Notifications";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox NotificationsText;
        private System.Windows.Forms.Button ClearNotificationsButton;
    }
}