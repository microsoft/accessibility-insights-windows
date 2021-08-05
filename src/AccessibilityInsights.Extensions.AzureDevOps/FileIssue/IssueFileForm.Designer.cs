// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Extensions.AzureDevOps.FileIssue
{
    partial class IssueFileForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IssueFileForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.zoomLabel = new System.Windows.Forms.Label();
            this.zoomOut = new System.Windows.Forms.Button();
            this.zoomIn = new System.Windows.Forms.Button();
            this.fileIssueBrowser = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileIssueBrowser)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.zoomLabel, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.zoomOut, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.zoomIn, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 804);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1535, 38);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // zoomLabel
            // 
            this.zoomLabel.AutoSize = true;
            this.zoomLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zoomLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zoomLabel.Location = new System.Drawing.Point(1440, 0);
            this.zoomLabel.Name = "zoomLabel";
            this.zoomLabel.Size = new System.Drawing.Size(54, 38);
            this.zoomLabel.TabIndex = 3;
            this.zoomLabel.Text = "100%";
            this.zoomLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // zoomOut
            // 
            this.zoomOut.AccessibleName = "Zoom out";
            this.zoomOut.BackColor = System.Drawing.Color.Transparent;
            this.zoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.zoomOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.zoomOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zoomOut.Location = new System.Drawing.Point(1399, 0);
            this.zoomOut.Margin = new System.Windows.Forms.Padding(0);
            this.zoomOut.Name = "zoomOut";
            this.zoomOut.Size = new System.Drawing.Size(38, 38);
            this.zoomOut.TabIndex = 4;
            this.zoomOut.Text = "-";
            this.zoomOut.UseVisualStyleBackColor = false;
            // 
            // zoomIn
            // 
            this.zoomIn.AccessibleName = "Zoom in";
            this.zoomIn.BackColor = System.Drawing.Color.Transparent;
            this.zoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.zoomIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.zoomIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zoomIn.Location = new System.Drawing.Point(1497, 0);
            this.zoomIn.Margin = new System.Windows.Forms.Padding(0);
            this.zoomIn.Name = "zoomIn";
            this.zoomIn.Size = new System.Drawing.Size(38, 38);
            this.zoomIn.TabIndex = 5;
            this.zoomIn.Text = "+";
            this.zoomIn.UseVisualStyleBackColor = false;
            // 
            // fileIssueBrowser
            // 
            this.fileIssueBrowser.AccessibleDescription = "Form to file issue";
            this.fileIssueBrowser.AccessibleName = "Issue filing browser";
            this.fileIssueBrowser.CreationProperties = null;
            this.fileIssueBrowser.DefaultBackgroundColor = System.Drawing.Color.White;
            this.fileIssueBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileIssueBrowser.Location = new System.Drawing.Point(0, 0);
            this.fileIssueBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.fileIssueBrowser.Name = "fileIssueBrowser";
            this.fileIssueBrowser.Size = new System.Drawing.Size(1535, 804);
            this.fileIssueBrowser.TabIndex = 0;
            this.fileIssueBrowser.ZoomFactor = 1D;
            // 
            // IssueFileForm
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuPopup;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1535, 842);
            this.Controls.Add(this.fileIssueBrowser);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "IssueFileForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Issue Dialog";
            this.Load += new System.EventHandler(this.IssueFileForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileIssueBrowser)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label zoomLabel;
        private System.Windows.Forms.Button zoomOut;
        private System.Windows.Forms.Button zoomIn;
        private Microsoft.Web.WebView2.WinForms.WebView2 fileIssueBrowser;
    }
}
