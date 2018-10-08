using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Wox.Plugin.RuneScapeWiki.Models;
using System.Diagnostics;

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
            var search = query.Terms.Length > 1 ? string.Join(" ", query.Terms.Skip(1)) : string.Empty;

            if (string.IsNullOrEmpty(search))
            {
                return Results.ToWoxResultsInitial(config);
            }
            
            List<MwSearchResult> mwSearchResults;
            try
            {
                mwSearchResults = MwApi.QuerySearchAsync(search, config).Result;
            }
            catch (Exception e)
            {
                return Results.ToWoxResultsError("Translation Error", e.Message, config);
            }

            List<Result> results;
            if (!mwSearchResults.Any())
            {
                results = Results.ToWoxResultsEmpty(search, config);
            }
            else
            {
                results = Results.ToWoxResults(mwSearchResults, config);
            }

            return results;
        }

        
    }
}
