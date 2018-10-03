using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Wox.Plugin.OldSchoolRunescape.Models;
using HtmlAgilityPack;

namespace Wox.Plugin.OldSchoolRunescape
{
    public class Main : IPlugin
    {
        private PluginInitContext _context;
        private const string BaseUrl = "https://oldschool.runescape.wiki";

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            var searchKey = HttpUtility.UrlEncode(string.Join("+", query.Terms));
            var route = $"{BaseUrl}/?search={searchKey}&fulltext=1&limit=10";

            HtmlDocument html;
            try
            {
                html = GetApiResponse(route);
            }
            catch (Exception e)
            {
                return ToErrorResult("Network Error", e.Message);
            }

            List<MwSearchResultFromHtml> extractedResults;
            try
            {
                extractedResults = ExtractSearchResults(html);
            }
            catch (Exception e)
            {
                return ToErrorResult("Translation Error", e.Message);
            }

            var results = extractedResults.Select(x => new Result
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

        private static HtmlDocument GetApiResponse(string route)
        {
            WebRequest request = WebRequest.Create(route);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            var html = new HtmlDocument();
            html.Load(dataStream);

            dataStream.Close();
            response.Close();

            return html;
        }

        private static List<MwSearchResultFromHtml> ExtractSearchResults(HtmlDocument html)
        {
            var list = new List<MwSearchResultFromHtml>();

            var searchResults = html.DocumentNode
                .Descendants("ul")
                .FirstOrDefault(x => x.HasClass("mw-search-results"))?
                .Descendants("li");

            if (searchResults == null || !searchResults.Any())
            {
                return list;
            };

            foreach (var searchResult in searchResults)
            {
                var divs = searchResult.Descendants("div");

                var headerAnchor = divs.FirstOrDefault(d => d.HasClass("mw-search-result-heading"))?.Descendants("a").FirstOrDefault();
                var headerTitle = headerAnchor?.GetAttributeValue("title", "");
                var headerRelativeUrl = headerAnchor?.GetAttributeValue("href", "");

                var resultText = divs.FirstOrDefault(d => d.HasClass("searchresult"))?.InnerHtml;

                if (string.IsNullOrEmpty(headerTitle) || string.IsNullOrEmpty(headerRelativeUrl))
                {
                    // Don't add result to the list if it has no title/url for whatever reason
                    continue;
                }

                list.Add(new MwSearchResultFromHtml
                {
                    Title = headerTitle,
                    Snippet = resultText,
                    Url = $"{BaseUrl}{headerRelativeUrl}"
                });
            }

            return list;
        }

        private static List<Result> ToErrorResult(string title, string message)
        {
            return new List<Result>
            {
                new Result {
                    Title = title,
                    SubTitle = message,
                    IcoPath = "Images\\osrs.png",
                    Action = a =>
                    {
                        return false;
                    }
                }
            };
        }
    }

    internal static class Extensions
    {
        public static bool HasClass(this HtmlNode node, string className)
        {
            return node.Attributes.Contains("class") && node.Attributes["class"].Value == className;
        }
    }
}
