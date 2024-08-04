using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Modinstaller
{
    public sealed class GithubApi
    {
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "No sideaffects")]
        public static async Task<HttpResponseMessage> FetchLatestRelease(string url, bool IsMSstore)
        {
            HttpResponseMessage connection;
            Assets linktozip;
            ApiJson assets = await Constants.Client.GetFromJsonAsync<ApiJson>(url);
            
            if (IsMSstore)
            {
                //x64 zips seem to be bigger, I dont know if this will remain reliable though
                linktozip =  assets.Assets.Where(link => link.Browser_download_url.EndsWith("zip"))
                                          .MaxBy(link => link.size);
            }
            else
            {
                linktozip = assets.Assets.Where(link => link.Browser_download_url.EndsWith("zip"))
                                         .MinBy(link => link.size);
            }
            return connection = await Constants.Client.GetAsync(linktozip.Browser_download_url);
        }

        public static async Task<HttpResponseMessage> FetchFromAllReleases(string url, bool IsMSstore)
        {
            HttpResponseMessage connection;
            Assets linktozip;
            List<ApiJson> data = new();
            var jsonstring = Constants.Client.GetAsync(url).Result;
            data = await jsonstring.Content.ReadAsAsync<List<ApiJson>>();
            
            if (IsMSstore)
            {
                linktozip =  data[0].Assets.Where(link => link.Browser_download_url.EndsWith("zip"))
                                          .MaxBy(link => link.size);
            }
            else
            {
                linktozip = data[0].Assets.Where(link => link.Browser_download_url.EndsWith("zip"))
                                         .MinBy(link => link.size);
            }

            System.Console.WriteLine(linktozip.Browser_download_url);
            return connection = await Constants.Client.GetAsync(linktozip.Browser_download_url);
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "No sideaffects")]
        public static async Task FetchMods()
        {
            var request = await Constants.Client.GetAsync(Constants.ModsJson);
            var text = await request.Content.ReadAsStringAsync();
            Constants.Mods = JsonSerializer.Deserialize<Dictionary<string, string>>(text);
        }
    }
}