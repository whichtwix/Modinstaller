using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

namespace Modinstaller
{
    public sealed class Presetfile
    {
        public static List<PresetsJson> GetPresets()
        {
            string file = File.ReadAllText(Constants.Jsonpath);
            return (List<PresetsJson>) JsonSerializer.Deserialize(file, typeof(List<PresetsJson>));
        }

        public static bool IsEpicGames(List<PresetsJson> presets)
        {
            int count =  0;
            foreach (var preset in presets)
            {
                if (Directory.Exists(preset.BaseFolder + @"\\.egstore") || Directory.Exists(preset.DestinationFolder + @"\\.egstore"))
                {
                    count++;
                }
            }
            return count != 0;
        }

        public static bool ClashingPaths(List<PresetsJson> presets, bool CheckBasefolders)
        {
            if (CheckBasefolders)
            {
                return presets.ConvertAll(x => x.BaseFolder).Distinct().Count() == 1;
            }
            else
            {
                return presets.ConvertAll(x => x.DestinationFolder).Distinct().Count() == 1;
            }
        }
    }
}