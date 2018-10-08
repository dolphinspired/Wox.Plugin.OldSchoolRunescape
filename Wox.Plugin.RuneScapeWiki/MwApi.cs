using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var url = $"{config.BaseUrl}/api.php?action=query&format=json" +
                "&generator=search" +
                "&prop=extracts|info|pageimages" +
                "&redirects=1" +
                "&exsentences=2&exlimit=max&exintro=1&explaintext=1&exsectionformat=plain" +
                "&inprop=url" +
                $"&gsrsearch={HttpUtility.UrlEncode(search)}";

            var response = await Client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var queryResult = JsonConvert.DeserializeObject<MwQueryResponse>(content);

            // Sort results by search relevance
            var orderedResults = queryResult.Query.Pages
                .Select(x => x.Value)
                .OrderBy(x => x.Index)
                .ToList();

            return orderedResults;
        }
    }
}
