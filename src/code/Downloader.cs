using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Spectre.Console;

namespace Modinstaller
{
    public partial class Modinstaller
    {
        public static string modversion;

        public static HttpClient client { get; set; } = new();

        public static Dictionary<string, string> Mods { get; } = new()
        {
            {"Town of Us", "https://api.github.com/repos/eDonnes124/Town-Of-Us-R/releases/latest"},
            {"The Other Roles", "https://api.github.com/repos/TheOtherRolesAU/TheOtherRoles/releases/latest"},
            {"Town of Host", "https://api.github.com/repos/tukasa0001/TownOfHost/releases/latest"},
            {"Las Monjas" , "https://api.github.com/repos/KiraYamato94/LasMonjas/releases/latest"},
            {"Town of Host:The Other Roles", "https://api.github.com/repos/music-discussion/TownOfHost-TheOtherRoles/releases"}
        };

        public static async Task Main(string[] args)
        {
            bool useagain = true;
            while (useagain)
            {
                string path = Setfolderpath();
                Choosemod(out string mod);
                Console.WriteLine($"Downloading {mod}");
                await Handlezip(path, mod);
                Movefiles(path, mod, modversion);
                Console.WriteLine("installation of mod complete");
                useagain = AnsiConsole.Confirm("Want to install another mod?");
            }
        }

        public static void Choosemod(out string mod)
        {
            bool confirmed = false;
            mod = string.Empty;

            Console.WriteLine("Use arrow keys to navigate and click enter");
            while (!confirmed)
            {
                mod = AnsiConsole.Prompt(
                                           new SelectionPrompt<string>()
                                               .Title("Select which mod you want to install:")
                                               .PageSize(10)
                                               .AddChoices(new[]
                                               {
                                                    "Town of Us",
                                                    "The Other Roles",
                                                    "Las Monjas",
                                                    "Town of Host",
                                                    "Town of Host:The Other Roles"
                                               }));

                confirmed = AnsiConsole.Confirm($"Confirm you want to install {mod}");
            }
        }

        public static string Setfolderpath()
        {
            bool acceptedpath = false;
            string path = string.Empty;
            while (acceptedpath != true)
            {
                path = AnsiConsole.Ask<string>("Enter the path to your among us folder(copy paste here):");
                if (Directory.Exists(path + "\\Among Us_Data")) acceptedpath = true;
                else Console.WriteLine($"the path '{path}' is not valid; vanilla files could not be found");
            }
            return path;
        }

        public static async Task Handlezip(string path, string selectedmod)
        {
            try
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Modinstaller");
                client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
                client.Timeout = TimeSpan.FromMinutes(30);
                HttpResponseMessage connection;

                string url = Mods[selectedmod];
                //we have to differentiate due to pre-releases
                if (url.Contains("latest"))
                {
                    connection = await Fetchlatestrelease(url);
                }
                else
                {
                    connection = await Fetchfromallreleases(url);
                }

                string zippath = $@"{path}" + "\\mod.zip";
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

                if (Directory.Exists(path + "\\BepInEx")) Directory.Delete(path + "\\BepInEx", true);
                if (Directory.Exists(path + "\\mono")) Directory.Delete(path + "\\mono", true);
                if (Directory.Exists(path + "\\dotnet")) Directory.Delete(path + "\\dotnet", true);
                ZipFile.ExtractToDirectory(zippath, path, true);
                zip.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
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

                string subfolder = modversion != string.Empty ? $@"{path}" + $"\\{mod} {modversion}" : $@"{path}" + $"\\{mod}";
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
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
    }
}
