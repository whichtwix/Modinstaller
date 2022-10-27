using System.Collections.Generic;

namespace Modinstaller
{
    public sealed class Json
    {
        public string Name { get; set; }
        public List<Assets> Assets { get; set; }
    }
    public class Assets
    {
        public string Browser_download_url { get; set; }
    }
}