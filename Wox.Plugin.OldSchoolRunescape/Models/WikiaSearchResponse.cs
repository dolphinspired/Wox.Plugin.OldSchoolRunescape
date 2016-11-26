using System.Collections.Generic;

namespace Wox.Plugin.OldSchoolRunescape.Models
{
    public class WikiaSearchResponse
    {
        public int Batches { get; set; }

        public int Total { get; set; }

        public int CurrentBatch { get; set; }

        public int Next { get; set; }

        public List<WikiaSearchItem> Items { get; set; }
    }
}
