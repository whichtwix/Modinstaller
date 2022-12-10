using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Modinstaller
{
    public partial class Modinstaller
    {
        public static async Task<HttpResponseMessage> Fetchlatestrelease(string url)
        {
            HttpResponseMessage connection;
            Json assets = await client.GetFromJsonAsync<Json>(url);
            Assets linktozip = assets.Assets.Find(link => link.Browser_download_url.EndsWith("zip"));
            modversion = assets.Name;
            return connection = await client.GetAsync(linktozip.Browser_download_url);
        }

        public static async Task<HttpResponseMessage> Fetchfromallreleases(string url)
        {
            HttpResponseMessage connection;
            List<Json> data = new();
            var jsonstring = client.GetAsync(url).Result;
            data = await jsonstring.Content.ReadAsAsync<List<Json>>();
            Json zip = data.Find(x => x.Assets[0].Browser_download_url.Contains("zip"));
            System.Console.WriteLine(zip.Assets[0].Browser_download_url);
            return connection = await client.GetAsync(zip.Assets[0].Browser_download_url);
        }
    }
}