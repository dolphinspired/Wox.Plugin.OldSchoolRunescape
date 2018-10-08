using System.Collections.Generic;

namespace Wox.Plugin.RuneScapeWiki.Models
{
    internal class MwQueryResponse
    {
        public MwQueryResponseQuery Query { get; set; }
    }

    internal class MwQueryResponseQuery
    {
        /// <summary>
        /// A list of seach results, where key = PageId.
        /// </summary>
        public Dictionary<string, MwSearchResult> Pages { get; set; }
    }

    internal class MwSearchResult
    {
        public int PageId { get; set; }

        /// <summary>
        /// The page's title. Friendly text, ready to use.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Order of search relevance, where lower value = more relevant.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// A few characters or sentences taken from the beginning page. Friendly text, ready to use.
        /// </summary>
        public string Extract { get; set; }
        
        public string FullUrl { get; set; }

        public string EditUrl { get; set; }

        public string CanonicalUrl { get; set; }

        public MwSearchResultThumnbnail Thumbnail { get; set; }

        /// <summary>
        /// The filename of the image representing the page, with extension.
        /// </summary>
        public string PageImage { get; set; }
    }

    internal class MwSearchResultThumnbnail
    {
        /// <summary>
        /// The full url to access this page's thumbnail for download.
        /// </summary>
        public string Source { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
