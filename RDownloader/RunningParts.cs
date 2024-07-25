/*
 * Created by SharpDevelop.
 * User: Krith
 * Date: 24-07-2024
 * Time: 15:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RDownloader
{
	/// <summary>
	/// Description of RunningParts.
	/// </summary>
	public partial class RunningParts : Form
	{
		List<DownloadParts> runningParts = new List<DownloadParts>();
		Timer loop = new Timer();
		Object obj = new object();
		
		public RunningParts(List<DownloadParts> parts)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			runningParts = parts;
			partsCount.Text = parts.Count.ToString();
			loop.Interval = 1000;
			loop.Enabled = true;
			loop.Tick += Render;
			loop.Start();					
		}

		public void Render(object sender, EventArgs e){
			
			lock(obj) {
				loop.Stop();
				
				Invoke(new Action(()=> {  
				         
				    listBox1.Items.Clear();
				    runningParts = runningParts.Where(x=> !x.IsDownloadHundredPercentComplete()).ToList();
				    
					foreach (var element in runningParts) {
					    var item = element.Name+"   "+ element.GetProgressText();
						listBox1.Items.Add(item);
					}
					
					partsCount.Text = runningParts.Count.ToString();
					
				}));
				
				loop.Start();
			}
		}
		
		public void UpdatePartsAndRender(List<DownloadParts> parts){
			
			lock(obj)
			{
				runningParts = parts;
			}
		}
		
		void RunningPartsFormClosing(object sender, FormClosingEventArgs e)
		{
			loop.Stop();
			listBox1.Items.Clear();					
		}
		
		void RunningPartsLoad(object sender, EventArgs e)
		{
	    	Render(this, e);
		}
		
	}
}
