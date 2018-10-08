using System.Collections.Generic;

namespace Wox.Plugin.RuneScapeWiki.Models
{
    internal class MwQueryResponse
    {
        public MwQueryResponseQuery Query { get; set; }
    }

    internal class MwQueryResponseQuery
    {
        public List<MwSearchResult> Search { get; set; }
    }

    internal class MwSearchResult
    {
        public string Title { get; set; }

        public int PageId { get; set; }

        public string Snippet { get; set; }
    }
}
