using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Modinstaller
{
    public sealed class GithubApi
    {
        public static async Task<HttpResponseMessage> Fetchlatestrelease(string url, Modversion modversion)
        {
            HttpResponseMessage connection;
            ApiJson assets = await Constants.Client.GetFromJsonAsync<ApiJson>(url);
            Assets linktozip = assets.Assets.Find(link => link.Browser_download_url.EndsWith("zip"));
            modversion.Version = assets.Name;
            return connection = await Constants.Client.GetAsync(linktozip.Browser_download_url);
        }

        public static async Task<HttpResponseMessage> Fetchfromallreleases(string url, Modversion modversion)
        {
            HttpResponseMessage connection;
            List<ApiJson> data = new();
            var jsonstring = Constants.Client.GetAsync(url).Result;
            data = await jsonstring.Content.ReadAsAsync<List<ApiJson>>();
            ApiJson zip = data.Find(x => x.Assets[0].Browser_download_url.Contains("zip"));
            modversion.Version = zip.Name;
            System.Console.WriteLine(zip.Assets[0].Browser_download_url);
            return connection = await Constants.Client.GetAsync(zip.Assets[0].Browser_download_url);
        }
    }
}