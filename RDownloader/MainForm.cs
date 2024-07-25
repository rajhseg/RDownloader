/*
 * Created by SharpDevelop.
 * User: Krith
 * Date: 18-07-2024
 * Time: 10:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace RDownloader
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			AddUrl.Enabled = false;
		}
		
		void AddUrlClick(object sender, EventArgs e)
		{			
			var url = textBox1.Text;
			DownloadFile file = new DownloadFile(url);
			file.Show();
		}
		void TextBox1TextChanged(object sender, EventArgs e)
		{
			if(textBox1.Text.Length<1){
				AddUrl.Enabled = false;
			}
			else{
				AddUrl.Enabled = true;
			}
		}
	}
}
