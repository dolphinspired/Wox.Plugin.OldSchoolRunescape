using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Wox.Plugin.RuneScapeWiki.Models;

namespace Wox.Plugin.RuneScapeWiki
{
    internal static class MwApi
    {
        static MwApi()
        {
            Client = new HttpClient();
            Client.Timeout = TimeSpan.FromSeconds(10);
        }

        private static readonly HttpClient Client = new HttpClient();

        public static async Task<List<MwSearchResult>> QuerySearchAsync(string search, WikiTypeConfig config)
        {
            var enc = HttpUtility.UrlEncode(search);
            var url = $"{config.BaseUrl}/api.php?action=query&format=json&list=search&srsearch={enc}&srlimit=10";
            var request = new HttpRequestMessage(HttpMethod.Get, url);            

            var response = await Client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var queryResult = JsonConvert.DeserializeObject<MwQueryResponse>(content);

            return queryResult.Query.Search;
        }
    }
}
