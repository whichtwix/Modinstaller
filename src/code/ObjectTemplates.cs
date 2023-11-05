using System.Collections.Generic;

namespace Modinstaller
{
    public sealed class ApiJson
    {
        public string Name { get; set; }

        public List<Assets> Assets { get; set; }
    }

    public sealed class Assets
    {
        public string Browser_download_url { get; set; }
    }

    public sealed class PresetsJson
    {
        public string Mod { get; set; }

        public string BaseFolder { get; set; }

        public string DestinationFolder { get; set; }
    }
}