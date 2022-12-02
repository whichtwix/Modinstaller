using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Modinstaller
{
    public sealed class Modinstaller
    {
        public const string url = "https://api.github.com/repos/eDonnes124/Town-Of-Us-R/releases/latest";
        
        public static string Touversion;

        public static HttpClient client = new();

        public static async Task Main(string[] args)
        {
            bool acceptedpath = false;
            string path = string.Empty;
            while (acceptedpath != true)
            {
                Console.WriteLine("What is the path of your Amogus?:");
                path = Console.ReadLine();
                if (Directory.Exists(path + "\\Among Us_Data")) acceptedpath = true;
                else Console.WriteLine($"the path '{path}' is not valid; vanilla files could not be found");
            }
            Console.WriteLine("Downloading town of us");
            await Handlezip(path);
            Movefiles(path, Touversion); 
        }
        
        public static async Task Handlezip(string path)
        {
            try
            {
                client.DefaultRequestHeaders.Add("User-Agent", "TownOfUs Downloader");
                client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

                Json assets = await client.GetFromJsonAsync<Json>(url);
                Assets linktozip = assets.Assets.Find(link => link.Browser_download_url.EndsWith("zip"));
                Touversion = assets.Name;
                var connection = await client.GetAsync(linktozip.Browser_download_url);

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
                string omitname = e.StackTrace.Split(@"C:\")[0];
                string errorlocation = e.StackTrace.Split(@"C:\")[1].Substring(11);
                Console.WriteLine(omitname + errorlocation);
                Console.ReadLine();
            }
        }
        
        public static void Movefiles(string path, string version)
        {
            try
            {
                if (Directory.Exists(path + "\\BepInEx")) Directory.Delete(path + "\\BepInEx", true);
                //this check will be phased out once the penultimate latest update doesnt use mono
                if (Directory.Exists(path + "\\mono")) Directory.Delete(path + "\\mono", true); 
                if (Directory.Exists(path + "\\dotnet")) Directory.Delete(path + "\\dotnet", true);
                
                string subfolder = $@"{path}" + $"\\ToU {version}";
                string[] files = Directory.GetFiles(subfolder);
                foreach (string file in files)
                {
                    File.Move($@"{file}", $@"{path}" + $"\\{file.Substring(subfolder.Length + 1)}", true);
                }
                
                string[] movablefolders = Directory.GetDirectories(subfolder);
                foreach (string folder in movablefolders)
                {
                    Directory.Move($@"{folder}", $@"{path}" + $"\\{folder.Substring(subfolder.Length + 1)}");
                }

                Directory.Delete(subfolder);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Handlezip()");
                string omitname = e.StackTrace.Split(@"C:\")[0];
                string errorlocation = e.StackTrace.Split(@"C:\")[1].Substring(11);
                Console.WriteLine(omitname + errorlocation);
                Console.ReadLine();
            }
        }
    }
}
