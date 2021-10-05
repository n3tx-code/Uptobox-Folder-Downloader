using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Uptobox_Folder_Downloader
{
    public enum Statuses
    {
        NotDownloaded,
        Downloaded
    }
    public class Download
    {
        static string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); 
        private string downloadPath = homePath + "/Uptobox_Folder_Downloader/downloads/";
        public string Code { get; set; }
        public Statuses Status { get; set; }
        
        public string fileName { get; set; }

        public Download(string code, string fileName)
        {
            Code = code;
            Status = Statuses.NotDownloaded;
            this.fileName = fileName;
        }

        public async Task StartDownload(UptoboxApi uptoboxApi)
        {
            string downloadLink = await uptoboxApi.GetDownloadLink(Code);
            using HttpClient httpClient = new HttpClient();

            using WebClient client = new WebClient();
            string downloadFullPath = downloadPath + fileName;
            client.DownloadFile(downloadLink, downloadFullPath);
            Status = Statuses.Downloaded;
            // todo :  check if there is enough space
        }
    }
}