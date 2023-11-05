using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Modinstaller
{
    public sealed class Constants
    {
        public static readonly string PresetsJson = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\Modinstaller\\InstallPresets.json";

        public static readonly string ErrorLog = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\Modinstaller\\ErrorLog.txt";

        public static readonly string ModsJson = "https://github.com/whichtwix/Modinstaller/raw/Master/ModList.json";

        public static readonly JsonSerializerOptions opts = new() { WriteIndented = true };

        public static Dictionary<string, string> Mods = new();

        public static readonly Dictionary<string, Func<Task>> Actions = new()
        {
            {"Install mod by giving inputs yourself", ActionsFromChoice.InstallByUser},

            {"Install by using saved presets", ActionsFromChoice.InstallByJson},

            {"Add or create presets", ActionsFromChoice.AddToJson},

            {"Delete presets", ActionsFromChoice.RemoveFromJson},

            {"View presets", ActionsFromChoice.ViewPresets}
        };

        public static readonly HttpClient Client = new()
        {
            DefaultRequestHeaders =
            {
                {"User-Agent", "Modinstaller"},
                {"X-GitHub-Api-Version", "2022-11-28"}
            },
            Timeout = TimeSpan.FromMinutes(30)
        };
    }
}