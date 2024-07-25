/*
 * Created by SharpDevelop.
 * User: Krith
 * Date: 18-07-2024
 * Time: 10:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace RDownloader
{
	partial class DownloadFile
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label filename;
		private System.Windows.Forms.FlowLayoutPanel downloadFileGroup;
		private System.Windows.Forms.Label overallStatus;		
		private System.Windows.Forms.Button button1;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		public void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.filename = new System.Windows.Forms.Label();
			this.downloadFileGroup = new System.Windows.Forms.FlowLayoutPanel();
			this.overallStatus = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Filename :";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// filename
			// 
			this.filename.Location = new System.Drawing.Point(100, 19);
			this.filename.Name = "filename";
			this.filename.Size = new System.Drawing.Size(250, 23);
			this.filename.TabIndex = 1;
			this.filename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// downloadFileGroup
			// 
			this.downloadFileGroup.AutoScroll = true;
			this.downloadFileGroup.Location = new System.Drawing.Point(12, 60);
			this.downloadFileGroup.Name = "downloadFileGroup";
			this.downloadFileGroup.Size = new System.Drawing.Size(673, 243);
			this.downloadFileGroup.TabIndex = 2;
			// 
			// overallStatus
			// 
			this.overallStatus.Location = new System.Drawing.Point(354, 19);
			this.overallStatus.Name = "overallStatus";
			this.overallStatus.Size = new System.Drawing.Size(199, 23);
			this.overallStatus.TabIndex = 3;
			this.overallStatus.Text = "overall";
			this.overallStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(561, 19);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(95, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "Running Parts";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// DownloadFile
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(697, 320);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.overallStatus);
			this.Controls.Add(this.downloadFileGroup);
			this.Controls.Add(this.filename);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(713, 359);
			this.MinimumSize = new System.Drawing.Size(713, 359);
			this.Name = "DownloadFile";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "DownloadFile";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DownloadFileFormClosing);
			this.Load += new System.EventHandler(this.DownloadFileLoad);
			this.ResumeLayout(false);

		}
	}
}
