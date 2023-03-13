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

        public static bool ClashingPaths(List<PresetsJson> presets, bool CheckBasefolders)
        {
            if (CheckBasefolders)
            {
                return presets.ConvertAll(x => x.BaseFolder).Distinct().Count() != presets.Count;
            }
            else
            {
                return presets.ConvertAll(x => x.DestinationFolder).Distinct().Count() != presets.Count;
            }
        }

        public static bool AllEmptyDestinations(List<PresetsJson> presets)
        {
            return presets.ConvertAll(x => x.DestinationFolder).TrueForAll(x => x?.Length == 0);
        }
    }
}