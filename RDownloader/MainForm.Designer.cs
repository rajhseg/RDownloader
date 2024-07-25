/*
 * Created by SharpDevelop.
 * User: Krith
 * Date: 18-07-2024
 * Time: 10:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace RDownloader
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button AddUrl;
		private System.Windows.Forms.TextBox textBox1;
		
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
			this.AddUrl = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// AddUrl
			// 
			this.AddUrl.Location = new System.Drawing.Point(439, 27);
			this.AddUrl.Name = "AddUrl";
			this.AddUrl.Size = new System.Drawing.Size(75, 23);
			this.AddUrl.TabIndex = 0;
			this.AddUrl.Text = "Add Url";
			this.AddUrl.UseVisualStyleBackColor = true;
			this.AddUrl.Click += new System.EventHandler(this.AddUrlClick);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(34, 29);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(388, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.TextChanged += new System.EventHandler(this.TextBox1TextChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(547, 88);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.AddUrl);
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "RDownloader";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
