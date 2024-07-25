
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RDownloader
{
	/// <summary>
	/// Description of DownloadParts.
	/// </summary>
	public partial class DownloadParts : UserControl
	{
		public delegate Task DownloadPartAgainHandler(DownloadParts part);
		public event DownloadPartAgainHandler DownloadPartAgain;
		
		public string PartId {set;get;}
		
		//public string Name {set;get;}
		
		private double downloadedSizeOfFile = 0;
		
		private double totalSizeOfFile = 0;
		
		private bool IsRunning {get;set;}
		
		private bool IsFinished {get;set;}
		
		private bool IsChunked {get;set;}
		
		public DownloadParts(string id, string partname)
		{			
			InitializeComponent();				
			this.PartId = id;	
			this.Name = partname;
			this.partname.Text = partname;
			this.IsRunning = false;
			this.IsFinished = false;
		}
		
		public void IsResponseChunked(bool value){
			this.IsChunked = value;
			this.button1.Enabled = !value;
		}
		
		public void UpdatePartStatus(string message){
			this.downloadSize.Text = message;
		}
		
		public void UpdateTotalSize(double totalSize)
		{
			this.totalSizeOfFile = totalSize;			
		}	
		
		public double GetTotalSize(){
			return this.totalSizeOfFile;
		}
		
		public double GetDownloadSize(){
			return this.downloadedSizeOfFile;
		}
		
		public void CompletedDownload(){
			this.downloadProgress.Style = ProgressBarStyle.Continuous;
			this.downloadProgress.Value = 100;
			this.IsFinished = true;
			this.IsRunning = false;
		}
		
		public void UpdateDownloadSize(double downloadSize)
		{
			this.IsRunning = true;
			
			this.downloadedSizeOfFile = downloadSize;
			// this.downloadSize.Text = downloadSize.ToString();
			
			if(totalSizeOfFile>0)
			{
				double val = ((double)(downloadedSizeOfFile/totalSizeOfFile))* 100;
				this.downloadProgress.Value = (int)val;
			}
			else{
				this.downloadProgress.Style = ProgressBarStyle.Marquee;
			}
			
			this.downloadSize.Text = this.GetProgressText();
			
			if(IsChunked){
				button1.Enabled = false;
			} else {
				button1.Enabled = !this.IsDownloadHundredPercentComplete();
			}
		}
		
		public string GetProgressText(){
			
			if(totalSizeOfFile==0)
				return SizeExtension.ToFileSize(downloadedSizeOfFile, 3);
			
			var valDec = (decimal)((downloadedSizeOfFile/totalSizeOfFile)*100);
			var val = Math.Round(valDec, 3);
			var d = SizeExtension.ToFileSize(downloadedSizeOfFile, 3);
			var t = SizeExtension.ToFileSize(totalSizeOfFile, 3);
			
			d = downloadedSizeOfFile.ToString();
			t = totalSizeOfFile.ToString();
			
			return ""+val.ToString()+"%"+"    "+d+" / "+t;
		}
		
		public decimal GetDownloadPercent(){
			
			if(totalSizeOfFile==0)
				return 0;
			
			var valDec = (decimal)((downloadedSizeOfFile/totalSizeOfFile)*100);
			var val = Math.Round(valDec,3);
			return val;
		}
		
		public bool IsDownloadHundredPercentComplete(){
			return this.GetDownloadPercent() >= 100;
		}
		
		public static string GetProgressText(double totalsize, double downloadsize){
			
			if(totalsize==0)
				return SizeExtension.ToFileSize(downloadsize, 3);
			
			//int val = (int)(((double)(downloadsize/totalsize))* 100);
			
			var valDec = (decimal)((downloadsize/totalsize)*100);
			var val = Math.Round(valDec, 3);
			
			var d = SizeExtension.ToFileSize(downloadsize, 3);
			var t = SizeExtension.ToFileSize(totalsize, 3);
			return ""+val.ToString()+"%"+"    "+d+" / "+t;
		}
		
		
		
		public static string AdjustFileSize(double fileSizeInBytes)
		{
			var names = new string[]{"bytes", "KB", "MB", "GB"};
		
		    double sizeResult = fileSizeInBytes * 1.0;
		    int nameIndex = 0;
		    while (sizeResult > 1024 && nameIndex < names.Length)
		    {
		        sizeResult /= 1024; 
		        nameIndex++;
		    }
		
		    return (int)sizeResult+" "+ names[nameIndex];
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			if(DownloadPartAgain!=null){
				this.IsFinished = false;
				this.downloadedSizeOfFile = 0;
				DownloadPartAgain(this);				
			}
		}
	}
}
