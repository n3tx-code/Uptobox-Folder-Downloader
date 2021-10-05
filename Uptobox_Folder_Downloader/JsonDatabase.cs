using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Uptobox_Folder_Downloader
{
    public class JsonDatabase
    {
        private string json_file_path = "database.json";
        private string errorFilePath = "error.log";
        private List<Download> downloads { get; set; }
        public JsonDatabase(string path)
        {
            json_file_path = path + json_file_path;
            errorFilePath = path + errorFilePath;
            if (File.Exists(json_file_path))
            {
                try
                {
                    string json = File.ReadAllText(json_file_path);
                    downloads = JsonSerializer.Deserialize<List<Download>>(json);
                }
                catch (Exception e)
                {
                    string errorMsg = DateTime.Now.ToString() + " | " + e.Message + "\n";
                    using StreamWriter errorFile = new(errorFilePath, append: File.Exists(errorFilePath));
                    errorFile.WriteAsync(errorMsg);
                    System.Environment.Exit(3);
                }
            }
            else
            {
                downloads = new List<Download>();
            }
        }

        public List<Download> PendingDownloads()
        {
            List<Download> pendingDownloads = new List<Download>();
            foreach (var download in downloads)
            {
                if (download.Status == Statuses.NotDownloaded)
                {
                    pendingDownloads.Add(download);   
                }
            }

            return pendingDownloads;
        }  
        
        public async void AddDownload(Download download)
        {
            downloads.Add(download);
            string downloadsJson =  JsonConvert.SerializeObject(downloads);
            await File.WriteAllTextAsync(json_file_path, downloadsJson);
        }

        public async void Update()
        {
            string downloadsJson =  JsonConvert.SerializeObject(downloads);
            await File.WriteAllTextAsync(json_file_path, downloadsJson);
        }

        public bool doesDownloadAllReadyExist(string code)
        {
            foreach (Download download in downloads)
            {
                if (download.Code.Equals(code))
                {
                    return true;
                }
            }

            return false;
        }
    }
}