using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Wox.Plugin.RuneScapeWiki.Models;
using HtmlAgilityPack;

namespace Wox.Plugin.RuneScapeWiki
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
            // Use OSRS config if specified, fall back on RS config otherwise.
            WikiTypeConfig config = query.ActionKeyword == "osw" ? WikiTypeConfig.Osrs : WikiTypeConfig.Rs;

            var searchKey = HttpUtility.UrlEncode(string.Join("+", query.Terms));
            var route = $"{config.BaseUrl}/?search={searchKey}&fulltext=1&limit=10";

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
                extractedResults = ExtractSearchResults(html, config);
            }
            catch (Exception e)
            {
                return ToErrorResult("Translation Error", e.Message);
            }

            var results = extractedResults.Select(x => new Result
            {
                Title = x.Title,
                SubTitle = CleanSnippet(x.Snippet),
                IcoPath = config.IcoPath,
                Action = a =>
                {
                    if (!string.IsNullOrEmpty(x.Url))
                    {
                        // Open the URL in your default browser via some Windows magic
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

        private static List<MwSearchResultFromHtml> ExtractSearchResults(HtmlDocument html, WikiTypeConfig config)
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
                    Url = $"{config.BaseUrl}{headerRelativeUrl}"
                });
            }

            return list;
        }

        private static string CleanSnippet(string snippet)
        {
            if (string.IsNullOrWhiteSpace(snippet))
            {
                return string.Empty;
            }

            // quick-and-dirty "strip HTML"
            var ret = Regex.Replace(snippet, "<[^>]*>", "");

            // attempt to get rid of some annoying wiki markup (bracketed text)
            ret = Regex.Replace(ret, @"\[[^\]]*]", "");

            return ret;
        }

        private static List<Result> ToErrorResult(string title, string message)
        {
            return new List<Result>
            {
                new Result {
                    Title = title,
                    SubTitle = message,
                    IcoPath = @"Images\error.png",
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
