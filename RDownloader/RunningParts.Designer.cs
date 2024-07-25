/*
 * Created by SharpDevelop.
 * User: Krith
 * Date: 24-07-2024
 * Time: 15:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace RDownloader
{
	partial class RunningParts
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label partsCount;
		private System.Windows.Forms.ListBox listBox1;
		
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
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.partsCount = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(116, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Total Running Parts :";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// partsCount
			// 
			this.partsCount.Location = new System.Drawing.Point(143, 22);
			this.partsCount.Name = "partsCount";
			this.partsCount.Size = new System.Drawing.Size(100, 23);
			this.partsCount.TabIndex = 1;
			this.partsCount.Text = "partsCount";
			this.partsCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listBox1
			// 
			this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 20;
			this.listBox1.Location = new System.Drawing.Point(12, 50);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(456, 184);
			this.listBox1.TabIndex = 2;
			// 
			// RunningParts
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(480, 261);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.partsCount);
			this.Controls.Add(this.label1);
			this.MaximumSize = new System.Drawing.Size(496, 300);
			this.MinimumSize = new System.Drawing.Size(496, 300);
			this.Name = "RunningParts";
			this.Text = "RunningParts";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RunningPartsFormClosing);
			this.Load += new System.EventHandler(this.RunningPartsLoad);
			this.ResumeLayout(false);

		}
	}
}
