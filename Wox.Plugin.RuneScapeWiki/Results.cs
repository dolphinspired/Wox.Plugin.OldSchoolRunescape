using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Wox.Plugin.RuneScapeWiki.Models;

namespace Wox.Plugin.RuneScapeWiki
{
    internal static class Results
    {
        public static List<Result> ToWoxResults(List<MwSearchResult> results, WikiTypeConfig config)
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

        public static List<Result> ToWoxResultsInitial(WikiTypeConfig config)
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

        public static List<Result> ToWoxResultsEmpty(string search, WikiTypeConfig config)
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

        public static List<Result> ToWoxResultsError(string error, string description, WikiTypeConfig config)
        {
            return new List<Result>
            {
                new Result {
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

        #region Private methods

        private static string CleanTitle(string title)
        {
            // Should fix apostrophes, quotes, etc.
            var ret = HttpUtility.HtmlDecode(title);

            return ret;
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

            // Should fix apostrophes, quotes, etc.
            ret = HttpUtility.HtmlDecode(ret);

            return ret;
        }

        #endregion
    }
}
