using System;
using System.IO;
using System.Net.Http;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Modinstaller
{
    public sealed class ModZip
    {
        public static async Task Install(string Basepath, string Destinationpath, string mod)
        {
            Modversion modversion = new()
            {
                Version = string.Empty
            };
            if (Destinationpath != string.Empty)
            {
                Console.WriteLine($"Downloading {mod} to {Destinationpath}");
                await DownloadExtractzip(Basepath, Destinationpath, mod, modversion);
                Movefiles(Destinationpath, mod, modversion.Version);
                return;
            }
            Console.WriteLine($"Downloading {mod} to {Basepath}");
            await DownloadExtractzip(Basepath, mod, modversion);
            Movefiles(Basepath, mod, modversion.Version);

            Console.WriteLine($"installation of {mod} complete");
        }

        public static async Task DownloadExtractzip(string Basepath, string selectedmod, Modversion modversion)
        {
            try
            {
                HttpResponseMessage connection;

                string url = Constants.Mods[selectedmod];
                //we have to differentiate due to pre-releases
                if (url.Contains("latest"))
                {
                    connection = await GithubApi.Fetchlatestrelease(url, modversion);
                }
                else
                {
                    connection = await GithubApi.Fetchfromallreleases(url, modversion);
                }

                string zippath = $"{Basepath}" + "\\mod.zip";
                var zip = new FileInfo(zippath);
                if (connection.IsSuccessStatusCode)
                {
                    using var data = await connection.Content.ReadAsStreamAsync();
                    using var datastream = zip.OpenWrite();
                    await data.CopyToAsync(datastream);
                }

                if (Directory.Exists(Basepath + "\\BepInEx")) Directory.Delete(Basepath + "\\BepInEx", true);
                if (Directory.Exists(Basepath + "\\mono")) Directory.Delete(Basepath + "\\mono", true);
                if (Directory.Exists(Basepath + "\\dotnet")) Directory.Delete(Basepath + "\\dotnet", true);
                ZipFile.ExtractToDirectory(zippath, Basepath, true);
                zip.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured downloading and extracting the zip, sending to log file");
                File.AppendAllText(Constants.ErrorLog, $"\n{e}");
            }
        }

        public static async Task DownloadExtractzip(string Basepath, string Destinationpath, string selectedmod, Modversion modversion)
        {
            try
            {
                HttpResponseMessage connection;

                string url = Constants.Mods[selectedmod];
                //we have to differentiate due to pre-releases
                if (url.Contains("latest"))
                {
                    connection = await GithubApi.Fetchlatestrelease(url, modversion);
                }
                else
                {
                    connection = await GithubApi.Fetchfromallreleases(url, modversion);
                }

                if (!Directory.Exists(Destinationpath)) Directory.CreateDirectory(Destinationpath);
                string zippath = $"{Destinationpath}" + "\\mod.zip";
                var zip = new FileInfo(zippath);
                if (connection.IsSuccessStatusCode)
                {
                    using var data = await connection.Content.ReadAsStreamAsync();
                    using var datastream = zip.OpenWrite();
                    await data.CopyToAsync(datastream);
                }
                CopyVanillaFiles(Basepath, Destinationpath);

                if (Directory.Exists(Destinationpath + "\\BepInEx")) Directory.Delete(Destinationpath + "\\BepInEx", true);
                if (Directory.Exists(Destinationpath + "\\mono")) Directory.Delete(Destinationpath + "\\mono", true);
                if (Directory.Exists(Destinationpath + "\\dotnet")) Directory.Delete(Destinationpath + "\\dotnet", true);
                ZipFile.ExtractToDirectory(zippath, Destinationpath, true);
                zip.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured downloading and extracting the zip, sending to log file");
                File.AppendAllText(Constants.ErrorLog, $"\n{e}");
            }
        }

        public static void Movefiles(string path, string mod, string modversion)
        {
            //no intermediate folder thus nothing to move: return
            //tou uses acronym in folder
            //the Name property from the Json class gives the folder name for las monjas
            try
            {
                switch (mod)
                {
                    case "The Other Roles":
                    case "Town of Host":
                    case "Town of Host:The Other Roles":
                        return;
                    case "Town of Us":
                        mod = "ToU";
                        break;
                    case "Las Monjas":
                        mod = modversion;
                        modversion = string.Empty;
                        break;
                }

                string subfolder = modversion != string.Empty ? $"{path}\\{mod} {modversion}" : $"{path}\\{mod}";
                foreach (string file in Directory.GetFiles(subfolder))
                {
                    File.Move($"{file}", $"{path}\\{file[(subfolder.Length + 1)..]}", true);
                }

                foreach (string folder in Directory.GetDirectories(subfolder))
                {
                    Directory.Move($"{folder}", $"{path}\\{folder[(subfolder.Length + 1)..]}");
                }

                Directory.Delete(subfolder);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured moving files, sending to log file");
                File.AppendAllText(Constants.ErrorLog, $"\n{e}");
            }
        }

        public static void CopyVanillaFiles(string originalfolder, string DestinationFolder)
        {
            foreach (string folder in Directory.GetDirectories(originalfolder, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(folder.Replace(originalfolder, DestinationFolder));
            }

            foreach (string file in Directory.GetFiles(originalfolder, "*.*",SearchOption.AllDirectories))
            {
                File.Copy(file, file.Replace(originalfolder, DestinationFolder), true);
            }
        }
    }
}