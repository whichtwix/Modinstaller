using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Modinstaller
{
    public sealed class GithubApi
    {
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "No sideaffects")]
        public static async Task<HttpResponseMessage> Fetchlatestrelease(string url)
        {
            HttpResponseMessage connection;
            ApiJson assets = await Constants.Client.GetFromJsonAsync<ApiJson>(url);
            Assets linktozip = assets.Assets.Find(link => link.Browser_download_url.EndsWith("zip"));
            return connection = await Constants.Client.GetAsync(linktozip.Browser_download_url);
        }

        public static async Task<HttpResponseMessage> Fetchfromallreleases(string url)
        {
            HttpResponseMessage connection;
            List<ApiJson> data = new();
            var jsonstring = Constants.Client.GetAsync(url).Result;
            data = await jsonstring.Content.ReadAsAsync<List<ApiJson>>();
            ApiJson zip = data.Find(x => x.Assets[0].Browser_download_url.Contains("zip"));
            System.Console.WriteLine(zip.Assets[0].Browser_download_url);
            return connection = await Constants.Client.GetAsync(zip.Assets[0].Browser_download_url);
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