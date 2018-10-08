using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wox.Plugin.RuneScapeWiki.Models;

namespace Wox.Plugin.RuneScapeWiki
{
    internal static class WoxResults
    {
        public static List<Result> WithSearchResults(List<MwSearchResult> results, WikiTypeConfig config)
        {
            return results.Select(x => new Result
            {
                Title = x.Title,
                SubTitle = x.Extract,
                IcoPath = config.IcoPath,
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
    }
}
