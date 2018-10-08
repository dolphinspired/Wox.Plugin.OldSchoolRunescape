using System;
using System.Collections.Generic;
using System.Linq;
using Wox.Plugin.RuneScapeWiki.Models;

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
            var keyword = query.ActionKeyword.ToLowerInvariant();
            var search = query.Terms.Length > 1 ? string.Join(" ", query.Terms.Skip(1)) : string.Empty;

            // Use OSRS config if specified, fall back on RS config otherwise.
            WikiTypeConfig config = keyword == Keywords.Osw ? WikiTypeConfig.Osrs : WikiTypeConfig.Rs;

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
