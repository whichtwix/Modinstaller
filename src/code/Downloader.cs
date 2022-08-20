using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Modinstaller
{
    public class Modinstaller
    {
        public const string url = "https://api.github.com/repos/eDonnes124/Town-Of-Us-R/releases/latest";
        public static string path;
        public static string Touversion;
        public static HttpClient client = new HttpClient();
        public static async Task Main(string[] args)
        {
            Console.WriteLine("What is the path of your Amogus?:");
            path = Console.ReadLine(); //todo: think of a better verfication than double entry?
            Console.WriteLine($"Are you sure of your path:{path}\nre-enter to confirm or enter a new path:");
            path = Console.ReadLine();
            Console.WriteLine("Downloading town of us");
            await Handlezip(path); 
            Movefiles(path, Touversion);
            Console.WriteLine("Done!");
        }
        public static async Task Handlezip(string path) 
        {
            try
            {
                client.DefaultRequestHeaders.Add("User-Agent", "TownOfUs Downloader");
                Json assets = await client.GetFromJsonAsync<Json>(url);
                string linktozip = assets.Assets[0].Browser_download_url;
                Touversion = assets.Name;
                var connection = await client.GetAsync(linktozip);
                
                string zippath = $@"{path}" + "\\tou.zip";
                var zip = new FileInfo(zippath);
                if (connection.IsSuccessStatusCode)
                {
                    using (var data = await connection.Content.ReadAsStreamAsync())
                    {
                        using (var datastream = zip.OpenWrite())
                        {
                            await data.CopyToAsync(datastream);
                        }
                    }
                }
                ZipFile.ExtractToDirectory(zippath, path);
                zip.Delete();
                client.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Handlezip()");
                Console.WriteLine(e);
            }
        }
        public static void Movefiles(string path, string version)
        {
           try
           {
                string subfolder = $@"{path}" + $"\\ToU {version}";
                string[] files = Directory.GetFiles(subfolder);
                foreach (string file in files)
                {
                    File.Move($@"{file}", $@"{path}" + $"\\{file.Substring(subfolder.Length + 1)}");
                }

                string[] movablefolders = Directory.GetDirectories(subfolder);
                foreach (string moving in movablefolders)
                {
                    Directory.Move($@"{moving}", $@"{path}" + $"\\{moving.Substring(subfolder.Length + 1)}");
                }

                Directory.Delete(subfolder);
           }
           catch (Exception e)
           {
                Console.WriteLine("Error in movefiles()");
                Console.WriteLine(e);
           }
        }
    }
}
