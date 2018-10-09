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
                $"&gsrsearch={HttpUtility.UrlEncode(search)}" +
                "&gsrlimit=6" + // Limit the number of pages returned by the query
                "&prop=extracts|info|pageimages" + // Include these fields in each search result
                "&redirects=1" + // Do not return redirect pages; instead, return the pages that are redirected-to
                "&exsentences=2&exlimit=max&exintro=1&explaintext=1&exsectionformat=plain" + // Describe how extracts should be returned
                "&inprop=url"; // Within info, include URLs that point to the page for each search result
            
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
