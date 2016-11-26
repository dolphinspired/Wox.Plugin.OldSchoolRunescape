﻿using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
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
            string searchKey = HttpUtility.UrlEncode(string.Join("+", query.Terms));
            string route = $"http://2007.runescape.wikia.com/api/v1/Search/List?query={searchKey}&limit=10&minArticleQuality=10&batch=1&namespaces=0%2C14";
            string apiResponse = GetApiResponse(route);

            WikiaSearchResponse searchResponse = JsonConvert.DeserializeObject<WikiaSearchResponse>(apiResponse);

            List<Result> results = searchResponse.Items.Select(x => new Result
            {
                Title = x.Title,
                SubTitle = x.Snippet,
                IcoPath = "Images\\osrs.png",
                Action = a =>
                {
                    var url = HttpUtility.UrlEncode(x.Url);
                    if (!string.IsNullOrEmpty(url))
                    {
                        System.Diagnostics.Process.Start(url);
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
    }
}
