using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Wox.Plugin.OldSchoolRunescape.Models;

namespace Wox.Plugin.OldSchoolRunescape
{
    public class Main : IPlugin
    {
        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            var searchKey = HttpUtility.UrlEncode(string.Join("+", query.Terms));
            var route = $"http://2007.runescape.wikia.com/api/v1/Search/List?query={searchKey}&limit=10&minArticleQuality=10&batch=1&namespaces=0%2C14";
            var apiResponse = GetApiResponse(route);

            var searchResponse = JsonConvert.DeserializeObject<WikiaSearchResponse>(apiResponse);
            var sortedItems = SortByTitleSimilarity(searchResponse.Items, query);

            var results = sortedItems.Select(x => new Result
            {
                Title = x.Title,
                SubTitle = Regex.Replace(x.Snippet, "<[^>]*>", ""), // quick-and-dirty "strip HTML"
                IcoPath = "Images\\osrs.png",
                Action = a =>
                {
                    if (!string.IsNullOrEmpty(x.Url))
                    {
                        System.Diagnostics.Process.Start(x.Url);
                    }

                    return true;
                }
            }).ToList();

            return results;
        }

        private static string GetApiResponse(string route)
        {
            WebRequest request = WebRequest.Create(route);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(dataStream);
            string responseContent = streamReader.ReadToEnd();

            streamReader.Close();
            dataStream.Close();
            response.Close();

            return responseContent;
        }

        private static List<WikiaSearchItem> SortByTitleSimilarity(List<WikiaSearchItem> items, Query query)
        {
            if (items == null)
            {
                return null;
            }

            if (items.Count <= 1 || string.IsNullOrEmpty(query?.Search))
            {
                return items;
            }
            
            items.ForEach(r => r.SetSearchTermSimilarity(query.Search));
            items.Sort();

            return items;
        }
    }
}
