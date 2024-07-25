
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.Configuration;

namespace RDownloader
{
	/// <summary>
	/// Description of DownloadFile.
	/// </summary>
	public partial class DownloadFile : Form
	{
		BackgroundWorker worker;
			
		private int SplitSize = 100;
		private int TotalSplitLength = 0;
		private double TotalResponseLength = 0;
		private bool IsReponseChunked = false;
		private bool mergingStarted =false;
		
		private List<Task> runningTasks = new List<Task>();
		List<TaskParameters> allTasks = new List<TaskParameters>();
		
		private delegate void UpdateDownloadSize(string id, double size);
		private delegate void UpdateTotalSize(string id, double size);
		
		private event UpdateDownloadSize DownloadSizechanged;
		private event UpdateTotalSize TotalSizechanged;
		
		private int parallelTaskCount = ConfigurationSettings.AppSettings["parallel"] == null ? 5
			: string.IsNullOrEmpty(ConfigurationSettings.AppSettings["parallel"]) ? 5 
				: int.Parse(ConfigurationSettings.AppSettings["parallel"]);
		
		private List<string> Files = new List<string>();
		private List<string> DeletingFiles = new List<string>();
		
		private string downloadFileName;
		private string parentDirectoryName = "";
		
		private List<DownloadParts> parts = new List<DownloadParts>();
		private List<CancellationTokenSource> tokenSources = new List<CancellationTokenSource>();
		
		public bool IsDownloadable {get;set;}
		
		private string _downloadFileUrl;
		
		private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
		
		private System.Windows.Forms.Timer overallTimer = new System.Windows.Forms.Timer();

		private bool isAllFinished = false;
		
		private bool completed = false;
		
		private object lockObject = new object();
				
		private RunningParts runningParts = null;
		
		public string DownloadFileUrl 
		{
			set 
			{
				this._downloadFileUrl = value;
			}
			
			get
			{
				return this._downloadFileUrl;
			}
		
		}
		
		private void UpdateProgress(object sender, ProgressChangedEventArgs e){
			var obj = e.UserState as ProgressState;
			if(obj!=null){
				if(obj.TypeSize==UpdateTypeSize.UpdatePartSplit){					
					var objParts = obj.Object as DownloadParts;
					if(objParts!=null)
					{			
						this.downloadFileGroup.Controls.Add(objParts);							
					}						
				} else if(obj.TypeSize==UpdateTypeSize.UpdateOverallStatus){
					this.overallStatus.Text = obj.Object as string;
				} else if(obj.TypeSize == UpdateTypeSize.UpdatePartStatus){
					var pStatus = (obj.Object as UpdatePartStatus);
					if(pStatus!=null){
						pStatus.Part.UpdatePartStatus(pStatus.Message);
					}
				}
				else {
					UpdateProgressChanged(obj.ID, obj.Size, obj.TypeSize);
				}
			}
		}
		
		private void UpdateProgressChanged(string id, double size, UpdateTypeSize typeSize){
			if(typeSize == UpdateTypeSize.DownloadSize){
				this.UpdateDSize(id, size);
			} else if(typeSize==UpdateTypeSize.TotalSize){
				this.UpdateTSize(id, size);
			}			
		}
		
		private void UpdateDSize(string id, double size){
			if(!string.IsNullOrEmpty(id))
			{	var _id = parts.FirstOrDefault(x=>x.PartId == id);
				if(_id!=null){
					_id.UpdateDownloadSize(size);
				}
			}
		}
		
		
		private void UpdateTSize(string id, double size){
			if(!string.IsNullOrEmpty(id)) {
				var _id = parts.FirstOrDefault(x=>x.PartId == id);
				if(_id!=null){
					_id.UpdateTotalSize(size);
				}
			}
		}
		
		public DownloadFile(string url)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			worker = new BackgroundWorker();
			
			runningTasks = new List<Task>();
			allTasks = new List<TaskParameters>();
			parts = new List<DownloadParts>();
			Files = new List<string>();
			DeletingFiles = new List<string>();
			
			this.DownloadFileUrl = url;
			this.mergingStarted = false;
		    this.IsDownloadable = true;
		    this.DownloadSizechanged += UpdateDSize;
		    this.TotalSizechanged += UpdateTSize;
		    
		    this.worker.WorkerReportsProgress = true;
		    
		    this.worker.DoWork += Download;
		    this.worker.ProgressChanged += UpdateProgress;
		    this.worker.RunWorkerCompleted+= WorkerCompleted;
		    this.worker.WorkerSupportsCancellation = true;
		    this.completed = false;
		    
		    this.InitInfoAndUI();
		    
		    //this.worker.RunWorkerAsync(new object[]{_downloadFileUrl, this.TotalResponseLength, this.IsReponseChunked});
		
		    this.overallTimer.Interval = 500;
		    this.overallTimer.Tick+=(s,e)=>{ overallStatus.Text = this.GetOverallStatus(); };
		    this.overallTimer.Enabled = true;
		    
		    if(this.parallelTaskCount < 1){
		    	this.parallelTaskCount = 1;
		    }
		    
		    if(this.parallelTaskCount > 10){
		    	this.parallelTaskCount = 10;
		    }
		    
		    //this.overallTimer.Start();
		}
				
		private string GetOverallStatus(){
			var tSize = this.parts.Select(x=> x.GetTotalSize()).Sum(x=>x);
			var dSize = this.parts.Select(x=>x.GetDownloadSize()).Sum(x=>x);			                  
			string status = DownloadParts.GetProgressText(tSize, dSize);		
			return status;
		}
		
		public void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e){
		
			if(this.IsReponseChunked){
				this.parts[0].CompletedDownload();
			} 
				
			this.overallTimer.Stop();
			this.overallStatus.Text = "100% download completed";					
		}
		
		public void InitInfoAndUI(){	
			
			Files = new List<string>();
			
	        System.Net.WebRequest req = HttpWebRequest.Create(this.DownloadFileUrl);
	        req.Method = "HEAD";
	        System.Net.WebResponse resp = req.GetResponse();
	        
	        var contentLengthObj = resp.Headers.Get("Content-Length");
	        
	        if(string.IsNullOrEmpty(contentLengthObj)){
	        	
	        	this.IsReponseChunked = true;
	        	this.SetDownloadFileName(resp);
	        	
	        	var docPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), this.parentDirectoryName);	        		        		        	
				var finalPath = Path.Combine(docPath, this.downloadFileName);
				
				Files.Add(finalPath);
				
				var partname = "Part_1";
				
				DownloadParts part = new DownloadParts(partname, partname);
				part.DownloadPartAgain += this.ReRunPart;
				parts.Add(part);

				this.downloadFileGroup.Controls.Add(part);							
				
	        }
	        else {
	        	
	        var responseLength = double.Parse(resp.Headers.Get("Content-Length"));
	        
	        this.SetDownloadFileName(resp);
	        
	        var partSize = (long)(responseLength / SplitSize);
	        this.TotalResponseLength = responseLength;	        
	        var remainder = Convert.ToInt32(responseLength % SplitSize);
	        
	        
	        this.TotalSplitLength = SplitSize;
	        
	        if(remainder > 0)
	        	this.TotalSplitLength += 1;
	         	
	        long previous = 0;
	        int index = 1;
	        string partname = "";
	        
	        for (long i = 1; i <= TotalSplitLength; i = i + 1)
	        {	        	
	        	string name = "Part_"+index.ToString()+"_" + Guid.NewGuid().ToString();
				
	        	var docPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), this.parentDirectoryName);
	        	
				var finalPath = Path.Combine(docPath, name);
				
				Files.Add(finalPath);
				
				partname = "Part_"+index.ToString();
				
				DownloadParts part = new DownloadParts(name, partname);
				part.DownloadPartAgain += this.ReRunPart;
				parts.Add(part);


				this.downloadFileGroup.Controls.Add(part);							
				previous = i;
	            
	            index++;
	        }
	      }
	        
	        var cancelcount = downloadFileGroup.Controls.Count;
	        
	        this.tokenSources = Enumerable.Range(0, cancelcount).Select(_=> new CancellationTokenSource()).ToList();
		}
		
		private void SetDownloadFileName(WebResponse resp){			
			
	        var keyString = resp.Headers.Get("Content-Disposition");
	        
	        if(string.IsNullOrEmpty(keyString)){	        
				Uri uri = new Uri(this.DownloadFileUrl);		
			    this.downloadFileName = System.IO.Path.GetFileName(uri.LocalPath);			    	
			    this.filename.Text = this.downloadFileName;
	        }
	        else {
		        var allKeys = keyString.Split(';');
		        	
		        foreach (string element in allKeys) {
		        	var sKeys = element.Split('=');
		        	if(sKeys[0].Trim().ToLower()=="filename"){
		        		this.downloadFileName = sKeys[1];
		        		this.filename.Text = this.downloadFileName;
		        		break;
		        	}
		        }
	       }
			
	        var docPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), this.downloadFileName);
	        docPath = this.DirectoryCreation(docPath);
		}
	
		public void Download(object sender, DoWorkEventArgs e)
    	{
			var URL = (string)(e.Argument as Object[])[0];
			var responseLength = (double)(e.Argument as Object[])[1];
			var isChunked = (bool)(e.Argument as Object[])[2];
			
			if(isChunked){
				int _fileIndex = 0;
				var finalPath = Files[_fileIndex];										
				var part = parts[_fileIndex];
				var token = tokenSources[_fileIndex];
				
				part.IsResponseChunked(true);
				
				ProgressState sta2 = new ProgressState();
				sta2.ID = part.PartId;
				sta2.Size = 0;
				sta2.TypeSize = UpdateTypeSize.TotalSize;
				
				this.worker.ReportProgress(0, sta2);							
													            				
	            var task_part = new Task(()=> StartDownloadFromChunked(URL, finalPath, part, token.Token, e),token.Token);	
	            task_part.Start();
	            
	            TaskParameters param = new TaskParameters();
	            param.TaskId = task_part.Id;
	            param.TaskObject = task_part;
	            param.Url = URL;
	            param.FinalPath = finalPath;		            
	            param.Parts = part;		
	            param.EventArgs = e;
	            allTasks.Add(param);	            
	            
	            this.runningTasks.Add(task_part);
	            
	            if(runningParts!=null){
					var paramPa = this.GetRunningParts();
					runningParts.UpdatePartsAndRender(paramPa);
				}
					
	            
	            Task.WaitAll(task_part);	
	            this.isAllFinished = true;
	            this.completed = true;
	            
	            FileInfo info = new FileInfo(finalPath);
	            var _dir = info.Directory.FullName;
	            Process.Start(_dir);
			}
			else {
				var partSize = (long)(responseLength / SplitSize);
		        
				var remainder = responseLength - ( partSize * SplitSize); // Convert.ToInt32(responseLength % SplitSize);
		         	
		        long previous = 0;
		        int index = 1;
		        string partname = "";		        		       
		        
		        for (long i = (long)partSize; i <= responseLength; i = i + (long)partSize)
		        {	        		        	
		        	var _fileIndex = index-1;
		        	long startPosition = previous;
		        	long endPosition = i-1;
		        	
					var finalPath = Files[_fileIndex];										
					var part = parts[_fileIndex];
					var token = tokenSources[_fileIndex];
					
					ProgressState sta1 =new ProgressState();
					sta1.ID = part.PartId;
					sta1.Size = partSize;
					sta1.TypeSize = UpdateTypeSize.TotalSize;
					
					this.worker.ReportProgress(0, sta1);							
													            				
		            var task_part = new Task(()=> StartDownload(URL, finalPath, startPosition, endPosition, part, token.Token, e), token.Token);	
		            //task_part.RunSynchronously();
		            
		            TaskParameters param = new TaskParameters();
		            param.TaskId = task_part.Id;
		            param.TaskObject = task_part;
		            param.Url = URL;
		            param.FinalPath = finalPath;
		            param.StartIndex = startPosition;
		            param.EndIndex = endPosition;
		            param.Parts = part;
		            param.EventArgs = e;
		            allTasks.Add(param);	            
		            
		            previous = i;
		            
		            index++;
		        }
			        	
		        if(remainder > 0){		        	
	        		double i = partSize * SplitSize + remainder;	        			       		        	      
		        	var _fileIndex = index-1;
		        	long startPosition = previous;
		        	long endPosition = (long)Math.Round(i);
		        	
					var finalPath = Files[_fileIndex];										
					var part = parts[_fileIndex];
					var token = tokenSources[_fileIndex];
					
					ProgressState sta2 = new ProgressState();
					sta2.ID = part.PartId;
					sta2.Size = remainder;
					sta2.TypeSize = UpdateTypeSize.TotalSize;
					
					this.worker.ReportProgress(0, sta2);							
													            				
		            var task_part = new Task(()=> StartDownload(URL, finalPath, startPosition, endPosition, part, token.Token, e), token.Token);	
		            //task_part.RunSynchronously();
		            
		            
		            TaskParameters param = new TaskParameters();
		            param.TaskId = task_part.Id;
		            param.TaskObject = task_part;
		            param.Url = URL;
		            param.FinalPath = finalPath;
		            param.StartIndex = startPosition;
		            param.EndIndex = endPosition;
		            param.Parts = part;
		            param.EventArgs = e;
		            allTasks.Add(param);	            
		            
		            previous = endPosition;		            		       			        	   
		        }
		        
		        //Parallel.ForEach(allTasks, new ParallelOptions(){ MaxDegreeOfParallelism = 5 } , x=> x.Start());
		         
		        // allTasks.ForEach(x=>x.Start());
		        
		        //Task.WaitAll(allTasks.ToArray());
		        
		        this.myTimer_Tick(e);
		        
		       // mergeClean();
			}
			
			if(parts!=null && parts.All(x=>x.IsDownloadHundredPercentComplete()))
			{
				MessageBox.Show("File downloaded");			
			}
    	}
		
		public async Task ReRunPart(DownloadParts part){
			
			if(!this.isAllFinished && !this.completed){
					
				
				var task = allTasks.Where(x=>x.Parts.Name == part.Name).FirstOrDefault();
				
				try {
					if(!task.TaskObject.IsFaulted && !task.TaskObject.IsCanceled && !task.TaskObject.IsCompleted){
						var index = allTasks.IndexOf(task);
						if(index > -1){
							this.tokenSources[index].Cancel();	

							await Task.WhenAll(task.TaskObject);													
						}
					}
					
					lock(lockObject){					
						this.ReRunPartSubMethod(task, part);
					}
					
				}
				catch(Exception ex){
					//MessageBox.Show(ex.Message);
				}
				
			}
		}
		
		public void ReRunPartSubMethod(TaskParameters task, DownloadParts part){
			        this.runningTasks.RemoveAll(x=> task.TaskObject.Id == x.Id);
										
					ProgressState sta =new ProgressState();
					sta.ID = part.PartId;
					sta.Size = 0;
					sta.TypeSize = UpdateTypeSize.DownloadSize;					
					this.worker.ReportProgress(0, sta);			
													
					this.RunTaskParameters(task);	
				
					ProgressState sta2 =new ProgressState();
					sta2.ID = part.PartId;
					sta2.Size = 0;
					sta2.TypeSize = UpdateTypeSize.UpdatePartStatus;
					sta2.Object = new UpdatePartStatus(){ Part = part, Message = "Queued, ReStarting, Please Wait..."};
					this.worker.ReportProgress(0, sta2);					
		}
		
		public void RunTaskParameters(TaskParameters param){	
			if(param!=null) {
				
				var _finalPathIndex = this.Files.IndexOf(param.FinalPath);
				
				string name = "Part_"+(_finalPathIndex+1).ToString()+"_" + Guid.NewGuid().ToString();
				
	        	var docPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), this.parentDirectoryName);
	        	
				var _finalPath = Path.Combine(docPath, name);
				
				DeletingFiles.Add(param.FinalPath);
				
				this.Files[_finalPathIndex] = _finalPath;
				param.FinalPath = _finalPath;
				
				var _token = new CancellationTokenSource();
				tokenSources[_finalPathIndex] = _token;
				var token = tokenSources[_finalPathIndex];
				
				var _ind = this.allTasks.IndexOf(param);
				Task task_part = null;
				
				if(IsReponseChunked){
					task_part = new Task(()=> StartDownloadFromChunked(param.Url, param.FinalPath, param.Parts, token.Token, param.EventArgs ), token.Token);
				} else {
				
					task_part = new Task(()=> StartDownload(param.Url, param.FinalPath,
				                                            param.StartIndex, param.EndIndex, param.Parts, token.Token, param.EventArgs), token.Token);	           	
				}
				 		            
	            param.TaskId = task_part.Id;
	            param.TaskObject = task_part;
	            			            
	            this.allTasks[_ind] = param;	            
			}
		}
		
		private void ExecDeleteFiles(){
			
			foreach (var element in DeletingFiles) {
				if(File.Exists(element)){
					try{
						File.Delete(element);
					} finally{
						
					}
				}
			}
		}
		
		private void myTimer_Tick(DoWorkEventArgs e)
		{		
			while(true) {						
				lock (lockObject)
	            {   
					try {
					if(this.isAllFinished && this.completed) {
						ExecDeleteFiles();
						break;
					}						
					
					this.RunParallel(this.allTasks,e);	
												
					}catch(Exception ex){
						//MessageBox.Show(ex.Message);
					}
	            }
			}
		
		}

		private void RunParallel(List<TaskParameters> tasksParams, DoWorkEventArgs e){
							
			if(this.runningTasks==null)
				this.runningTasks = new List<Task>();
			
			var exceptionTasks = this.runningTasks.Where(x=>x.IsFaulted).ToList().Select(x=>x.Id).ToList();
			
			if(exceptionTasks!=null && exceptionTasks.Count > 0) {
				
				this.runningTasks.RemoveAll(x=> exceptionTasks.Contains(x.Id));
				
				for (int i = 0; i < exceptionTasks.Count; i++) {
					
					var param = this.allTasks.FirstOrDefault(x=>x.TaskId == exceptionTasks[i]);
					this.RunTaskParameters(param);
				}							
			}			
			
			var finishedTask = this.runningTasks.Where(x=>x.IsCompleted).ToList().Count;					
			
			if(finishedTask==0 && this.runningTasks.Count==0)
				finishedTask = this.parallelTaskCount;
			else {
				finishedTask = this.parallelTaskCount - this.runningTasks.ToList().Count + finishedTask;
			}
			
			var tasks = tasksParams.Select(x=>x.TaskObject);	
			
			var unRunTasks = tasks.Except(this.runningTasks);
			
			var newTasks = unRunTasks.Take(finishedTask).ToList();
			
			if(newTasks!=null && newTasks.Count() >0){
				
				Parallel.ForEach(newTasks, new ParallelOptions(){ MaxDegreeOfParallelism = this.parallelTaskCount },
				                 (x) => {
				                 	if(!x.IsCompleted && !x.IsCanceled && !x.IsFaulted) 
				                 	{
				                 		x.Start();
				                 		this.runningTasks.Add(x);
				                 	}
				                 });				
				
				 if(runningParts!=null){
					var param = this.GetRunningParts();
					runningParts.UpdatePartsAndRender(param);
				  }				
			}	
												
			this.isAllFinished = tasks.All(x=>x.IsCompleted);	
			var allAreHundredPercent = parts.All(x=>x.IsDownloadHundredPercentComplete());
			
			if(isAllFinished && !this.mergingStarted && allAreHundredPercent) {
				this.mergingStarted = true;
				ProgressState sta2 = new ProgressState();
				sta2.ID = "0";
				sta2.Size = 0;
				sta2.TypeSize = UpdateTypeSize.UpdateOverallStatus;
				sta2.Object = "Merging....";
				
				this.worker.ReportProgress(0, sta2);							
													            				
		        
				this.mergeClean();
				this.completed = true;
				return;
			}			
		}
		 
		public void StartDownloadFromChunked(string url, string finalPath, DownloadParts part, CancellationToken token, DoWorkEventArgs e, int retry = 0){
			
			try {	
				
				HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);				
				HttpWebResponse response = (HttpWebResponse)(request.GetResponse());
				Stream responseStream = response.GetResponseStream();									
				this.saveStream(finalPath, responseStream, part, token, e);			
			}
			catch(Exception ex){
				retry++;
				
				if(retry<11)
					this.StartDownloadFromChunked(url, finalPath, part, token, e, retry);
			}	
		}
		
		public void StartDownload(string url, string finalPath, long startIndex, long endIndex, DownloadParts part, CancellationToken token, DoWorkEventArgs e){
			try {				
					
				if(File.Exists(finalPath))
					File.Delete(finalPath);
				
				HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
				request.AddRange(startIndex, endIndex);
				HttpWebResponse response = (HttpWebResponse)(request.GetResponse());
				Stream responseStream = response.GetResponseStream();									
				this.saveStream(finalPath, responseStream, part, token, e);			
			}
			catch(Exception ex){				
				throw ex;
			}
		}
		
		private void saveStream(string path, Stream stream, DownloadParts part, CancellationToken token, DoWorkEventArgs e){
			
			
			var filestream = new FileStream(path,FileMode.Append, FileAccess.Write);
			
			try{
				
				using (Stream respStream = stream)
				{
				    byte[] buffer = new byte[4096];
				    double size = 0;
				    int bytesRead;
				    
				    while ((bytesRead = respStream.Read(buffer, 0, 4096)) > 0)
				    {		
				    	if(this.worker.CancellationPending || token.IsCancellationRequested){
				    		break;
				    	}
				    	
				    	size += bytesRead;
				        filestream.Write(buffer, 0, bytesRead);			        
					        
						ProgressState sta =new ProgressState();
						sta.ID = part.PartId;
						sta.Size = size;
						sta.TypeSize = UpdateTypeSize.DownloadSize;
						
						this.worker.ReportProgress(0, sta);						
				    }
				    
				    if(token.IsCancellationRequested){
				    	token.ThrowIfCancellationRequested();
				    }
				    
				    if(this.worker.CancellationPending){
			    		e.Cancel =true;
			    		return;
			    	}
				}
						
				filestream.Dispose();
			}
			catch(Exception ex){
				throw ex;
			}
			finally{
				filestream.Dispose();	
				stream.Dispose();
			}
		}
		
		private string DirectoryCreation(string folder){
			bool retry = true;
			int index = 1;
			string path = folder;
			while(retry) {
				try{
					if(!Directory.Exists(path)){
						Directory.CreateDirectory(path);
						retry = false;
						DirectoryInfo info = new DirectoryInfo(path);
						this.parentDirectoryName = info.Name;
						return path;
					} else{
						path = folder+index;
						index++;
					}
				} catch(UnauthorizedAccessException ex){
					retry = false;
					throw ex;
				}
				catch(AccessViolationException ex){
					retry = false;
					throw ex;
				}
				catch(NotSupportedException ex){
					retry = false;
					throw ex;
				}
				catch(Exception ex){
					path = folder+index;
					index++;
				}
			}
		
			return path;
		}
		
     	private void mergeClean()
    	{        	
	        const int chunkSize = 1 * 1024; // 2KB
	        	        
        	var docPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), this.parentDirectoryName);        	        
        	   
	        using (var output = File.Create(Path.Combine(docPath, downloadFileName)))
	        {
	        	int fileindex = 0;
	        	
	            foreach (var file in Files)
	            {
	            	fileindex++;
	            	var filescount = Files.Count;
	            	var percentage = (fileindex*100)/filescount;
	            	
					ProgressState sta2 = new ProgressState();
					sta2.ID = "0";
					sta2.Size = 0;
					sta2.TypeSize = UpdateTypeSize.UpdateOverallStatus;
					sta2.Object = "Merging....  "+percentage+"%";
					
					this.worker.ReportProgress(0, sta2);							
														            							        
	                using (var input = File.OpenRead(file))
	                {
	                    var buffer = new byte[chunkSize];
	                    int bytesRead;
	                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
	                    {
	                        output.Write(buffer, 0, bytesRead);
	                    }
	                }
	            }
	        }

	        foreach (var file in Files)
	        {
	            File.Delete(file);
	        }
	        
	        Process.Start(docPath);
		}
		
     	void DownloadFileFormClosing(object sender, FormClosingEventArgs e)
		{
     		runningTasks.ForEach((x)=>{
     		                     
     		                     	var tsk = allTasks.FirstOrDefault(y=>y.TaskId==x.Id);
	     		                    var index = allTasks.IndexOf(tsk);
		     		                   
	     		                    if(index > -1){
		     		                    	var canceltoken = this.tokenSources[index];
		     		                    	if(canceltoken!=null)
		     		                    		canceltoken.Cancel();
		     		                }
	     		                          		                    
     		                     });
     		
     		
     		
     		this.isAllFinished = true;
     		this.completed = true;
     		
     		this.overallTimer.Stop();
     		this.worker.CancelAsync();     		
     		
     		allTasks.ForEach(x=> {
     		                 	var index = allTasks.IndexOf(x);
     		                 	if(index > -1){
     		                 		var cancelToken = this.tokenSources[index];
     		                 		if(cancelToken!=null)
     		                 			cancelToken.Cancel();
     		                 	}
     		               });
     		
     		parts.ForEach(x=>{
     		              	x.DownloadPartAgain-=this.ReRunPart;
     		              });     	
		}
     	
		void DownloadFileLoad(object sender, EventArgs e)
		{
			this.isAllFinished = false;
			this.completed = false;
	 		this.worker.RunWorkerAsync(new object[]{_downloadFileUrl, this.TotalResponseLength, this.IsReponseChunked});				   
		    this.overallTimer.Enabled = true;
		    this.overallTimer.Start();
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			var param = this.GetRunningParts();
			runningParts = new RunningParts(param);			
			runningParts.ShowDialog();			
		}
		
		private List<DownloadParts> GetRunningParts(){
			var taskIDs = runningTasks.Select(x=>x.Id).ToList();
			
			var param = allTasks.Where(x=>taskIDs.Contains(x.TaskId)).ToList().Select(x=>x.Parts).ToList();
			var result = param.Where(x=> !x.IsDownloadHundredPercentComplete()).ToList();
			return result;
		}
	}
	
	public enum UpdateTypeSize {
		DownloadSize =1,
		TotalSize,
		UpdatePartSplit,
		UpdateOverallStatus,
		UpdatePartStatus
	}
	
	public class ProgressState {
		public string ID {get;set;}
		public double Size{get;set;}
		public UpdateTypeSize TypeSize{get;set;}
		public object Object {get;set;}
	}
	
	public class TaskParameters {
		public int TaskId {get;set;}
		public Task TaskObject {get;set;}
		public string Url {get;set;}
		public string FinalPath {get;set;}
		public long StartIndex {get;set;}
		public long EndIndex {get;set;}
		public DownloadParts Parts {get;set;}		
		public DoWorkEventArgs EventArgs {get;set;}
	}
	
	public class UpdatePartStatus{ 
		public string Message {get;set;}
		public DownloadParts Part {get;set;}
	}
}
