using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Uptobox_Folder_Downloader
{
    public class UptoboxApi
    {
        private static string apiToken { get; set; }
        private static string folderPath = "//raspberry";
        // todo : 
        private static string doneFolderPath = folderPath + "/done";
        public UptoboxApi()
        {
            apiToken = Environment.GetEnvironmentVariable("UPTOBOX_API_TOKEN");
        }

        public async Task<string> GetDownloadLink(string fileCode)
        {
            string url = "https://uptobox.com/api/link?token=" + apiToken + "&file_code=" + fileCode;
            using HttpClient httpClient = new HttpClient();
            string content = await httpClient.GetStringAsync(url);
            var json = JsonConvert.DeserializeObject(content);
            JObject obj = JObject.Parse(json.ToString());
            return obj["data"]["dlLink"].ToString();
        }

        public async Task Retrieve10Downloads(JsonDatabase database)
        {
            List<Download> downloads = new List<Download>();
            string url = "https://uptobox.com/api/user/files?token=" + apiToken + "&path=//raspberry&limit=10";
            using HttpClient httpClient = new HttpClient();
            var content = await httpClient.GetStringAsync(url);
            var json = JsonConvert.DeserializeObject(content);
            JObject obj = JObject.Parse(json.ToString());
            var files = obj["data"]["files"];
            foreach (var file in files)
            {
                if (!database.doesDownloadAllReadyExist(file["file_code"].ToString()))
                {
                    Download download = new Download(file["file_code"].ToString(), file["file_name"].ToString());
                    database.AddDownload(download);   
                }
            }
            
        }
    }
}