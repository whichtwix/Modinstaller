using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Modinstaller
{
    public sealed class Presetfile
    {
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        public static List<PresetsJson> GetPresets()
        {
            string file = File.ReadAllText(Constants.PresetsJson);
            return (List<PresetsJson>) JsonSerializer.Deserialize(file, typeof(List<PresetsJson>));
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        public static async void WriteJson(PresetsJson preset)
        {
            if (!File.Exists(Constants.PresetsJson))
            {
                Directory.CreateDirectory(Constants.PresetsJson.Replace("\\InstallPresets.json", string.Empty));
                List<PresetsJson> presets = new() { preset };
                string Serial = JsonSerializer.Serialize(presets, Constants.opts);
                await File.WriteAllTextAsync(Constants.PresetsJson, Serial);
                return;
            }
            var currentfile = GetPresets();
            currentfile = currentfile.FindAll(x => x.Mod != preset.Mod);
            currentfile.Add(preset);
            string serial = JsonSerializer.Serialize(currentfile, Constants.opts);
            await File.WriteAllTextAsync(Constants.PresetsJson, serial);
        }

        public static bool ClashingPaths(List<PresetsJson> presets, bool CheckBasefolders)
        {
            if (CheckBasefolders)
            {
                return presets.ConvertAll(x => x.BaseFolder).Distinct().Count() != presets.Count;
            }
            return presets.ConvertAll(x => x.DestinationFolder).Distinct().Count() != presets.Count;
        }

        public static bool AllEmptyDestinations(List<PresetsJson> presets)
        {
            return presets.ConvertAll(x => x.DestinationFolder).TrueForAll(x => x?.Length == 0);
        }
    }
}