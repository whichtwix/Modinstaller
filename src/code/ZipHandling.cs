using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Modinstaller
{
    public sealed class ModZip
    {
        public static async Task Install(string Basepath, string Destinationpath, string mod, bool IsMSstore)
        {
            if (Destinationpath != string.Empty)
            {
                Console.WriteLine($"Downloading {mod} to {Destinationpath}");
                await DownloadExtractzip(Basepath, Destinationpath, mod, IsMSstore);
                Movefiles(Destinationpath);
                ValidateZip(Destinationpath, IsMSstore);
                Console.WriteLine($"installation of {mod} complete");
                return;
            }
            Console.WriteLine($"Downloading {mod} to {Basepath}");
            await DownloadExtractzip(Basepath, mod, IsMSstore);
            Movefiles(Basepath);
            ValidateZip(Basepath, IsMSstore);

            Console.WriteLine($"installation of {mod} complete");
        }

        public static async Task DownloadExtractzip(string Basepath, string selectedmod, bool IsMSstore)
        {
            try
            {
                HttpResponseMessage connection;

                string url = Constants.Mods[selectedmod];
                //we have to differentiate due to pre-releases
                if (url.Contains("latest"))
                {
                    connection = await GithubApi.FetchLatestRelease(url, IsMSstore);
                }
                else
                {
                    connection = await GithubApi.FetchFromAllReleases(url, IsMSstore);
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

        public static async Task DownloadExtractzip(string Basepath, string Destinationpath, string selectedmod, bool IsMSstore)
        {
            try
            {
                HttpResponseMessage connection;

                string url = Constants.Mods[selectedmod];
                //we have to differentiate due to pre-releases
                if (url.Contains("latest"))
                {
                    connection = await GithubApi.FetchLatestRelease(url, IsMSstore);
                }
                else
                {
                    connection = await GithubApi.FetchFromAllReleases(url, IsMSstore);
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

        public static void Movefiles(string path)
        {
            try
            {
                var folders = Directory.GetDirectories(path);

                var subfolder = folders.FirstOrDefault(x => Directory.Exists($"{x}\\BepInEx") && Directory.Exists($"{x}\\dotnet"));
                if (subfolder == null) return;
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

        public static void ValidateZip(string path, bool IsMSstore)
        {
            if (!IsMSstore && File.Exists(path + "\\dotnet\\Microsoft.DiaSymReader.Native.amd64.dll"))
            {
                Console.WriteLine("The game was detected as Steam/Epic/Itch earlier but a Microsoft specific dll was found");
                Console.WriteLine("May need to download the correct zip yourself");
            }
            if (IsMSstore && File.Exists(path + "\\dotnet\\Microsoft.DiaSymReader.Native.x86.dll"))
            {
                Console.WriteLine("The game was detected as Microsoft earlier but a Non-Microsoft specific dll was found");
                Console.WriteLine("May need to download the correct zip yourself");
            }
        }
    }
}