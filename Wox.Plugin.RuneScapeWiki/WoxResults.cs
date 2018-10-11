using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Wox.Plugin.RuneScapeWiki.Models;

namespace Wox.Plugin.RuneScapeWiki
{
    internal static class WoxResults
    {
        public static List<Result> WithSearchResults(List<MwSearchResult> results, WikiTypeConfig config, PluginInitContext context)
        {
            return results.Select(x => new Result
            {
                Title = x.Title,
                SubTitle = CleanExtract(x.Extract),
                IcoPath = config.IcoPath,
                //IcoPath = MwThumbnails.GetIcoPath(x, config, context), // Removed image thumbnail functionality, not stable
                Action = a =>
                {
                    // Open the URL in your default browser via some Windows magic
                    Process.Start(x.CanonicalUrl);
                    return true;
                }
            }).ToList();
        }

        public static List<Result> NoSearchQuery(WikiTypeConfig config)
        {
            return new List<Result>
            {
                new Result
                {
                    Title = $"Search the {config.WikiName}",
                    IcoPath = config.IcoPath
                }
            };
        }

        public static List<Result> NoSearchResults(string search, WikiTypeConfig config)
        {
            return new List<Result>
            {
                new Result
                {
                    Title = "No results",
                    SubTitle = $"No results found for search term: '{search}'",
                    IcoPath = config.IcoPath
                },
                new Result
                {
                    Title = "Create page",
                    SubTitle = $"Create a new page for '{search}'",
                    IcoPath = config.IcoPath,
                    Action = a =>
                    {
                        var url = $"{config.BaseUrl}/w/{HttpUtility.UrlEncode(search)}?action=edit";
                        Process.Start(url);
                        return true;
                    }
                }
            };
        }

        public static List<Result> Error(string error, string description, WikiTypeConfig config)
        {
            return new List<Result>
            {
                new Result
                {
                    Title = error,
                    SubTitle = description,
                    IcoPath = config.IcoPath,
                    Action = a =>
                    {
                        return false;
                    }
                }
            };
        }

        private static string CleanExtract(string extract)
        {
            if (string.IsNullOrEmpty(extract))
            {
                return extract;
            }

            return Regex.Replace(extract, @"\r\n?|\n", " ");
        }
    }
}
