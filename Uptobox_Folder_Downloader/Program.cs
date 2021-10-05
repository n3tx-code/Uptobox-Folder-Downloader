using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace Uptobox_Folder_Downloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string path = homePath + "/Uptobox_Folder_Downloader/";
            string errorFilePath = homePath + "/Uptobox_Folder_Downloader/error.log";
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!Directory.Exists(path + "downloads/"))
                {
                    Directory.CreateDirectory(path + "downloads/");
                }

            }
            catch (Exception e)
            {
                string errorMsg = DateTime.Now.ToString() + " | " + e.Message.ToString() + "\n";
                await using StreamWriter errorFile = new(errorFilePath, append: File.Exists(errorFilePath));
                await errorFile.WriteAsync(errorMsg);
                System.Environment.Exit(3);
            }

            JsonDatabase jsonDatabase = new JsonDatabase(path);
            UptoboxApi uptoboxApi = new UptoboxApi();
            List<Download> pendingDownloads = jsonDatabase.PendingDownloads();

            while (true)
            {
                Console.WriteLine("coucou");
                do
                {
                    pendingDownloads = jsonDatabase.PendingDownloads();
                    if (jsonDatabase.PendingDownloads().Count > 0)
                    {

                        await pendingDownloads[0].StartDownload(uptoboxApi);
                        jsonDatabase.Update();
                    }
                    else
                    {
                        await uptoboxApi.Retrieve10Downloads(jsonDatabase);
                        pendingDownloads = jsonDatabase.PendingDownloads();
                    }
                } while (pendingDownloads.Count > 0);

                Thread.Sleep(TimeSpan.FromHours(1));
            }
        }
    }
}