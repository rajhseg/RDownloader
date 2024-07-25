/*
 * Created by SharpDevelop.
 * User: Krith
 * Date: 18-07-2024
 * Time: 10:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace RDownloader
{
	partial class DownloadParts
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Label partname;
		private System.Windows.Forms.ProgressBar downloadProgress;
		private System.Windows.Forms.Label downloadSize;
		private System.Windows.Forms.Button button1;
		
		/// <summary>
		/// Disposes resources used by the control.
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
		private void InitializeComponent()
		{
			this.partname = new System.Windows.Forms.Label();
			this.downloadProgress = new System.Windows.Forms.ProgressBar();
			this.downloadSize = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// partname
			// 
			this.partname.Location = new System.Drawing.Point(18, 10);
			this.partname.Name = "partname";
			this.partname.Size = new System.Drawing.Size(64, 23);
			this.partname.TabIndex = 0;
			this.partname.Text = "part 1";
			this.partname.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// downloadProgress
			// 
			this.downloadProgress.Location = new System.Drawing.Point(88, 10);
			this.downloadProgress.Name = "downloadProgress";
			this.downloadProgress.Size = new System.Drawing.Size(281, 23);
			this.downloadProgress.Step = 1;
			this.downloadProgress.TabIndex = 1;
			// 
			// downloadSize
			// 
			this.downloadSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.downloadSize.Location = new System.Drawing.Point(377, 10);
			this.downloadSize.Name = "downloadSize";
			this.downloadSize.Size = new System.Drawing.Size(194, 23);
			this.downloadSize.TabIndex = 2;
			this.downloadSize.Text = "downloadSize";
			this.downloadSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(575, 10);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(55, 23);
			this.button1.TabIndex = 3;
			this.button1.Text = "ReStart";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// DownloadParts
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.button1);
			this.Controls.Add(this.downloadSize);
			this.Controls.Add(this.downloadProgress);
			this.Controls.Add(this.partname);
			this.Name = "DownloadParts";
			this.Size = new System.Drawing.Size(636, 44);
			this.ResumeLayout(false);

		}
	}
}
