using System;

namespace Wox.Plugin.OldSchoolRunescape.Models
{
    public class WikiaSearchItem : IComparable<WikiaSearchItem>
    {
        public int Quality { get; set; }

        public string Url { get; set; }

        public int Ns { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Snippet { get; set; }

        public int SearchTermSimilarity { get; private set; }

        /// <summary>
        /// Quality is returned on a 1-100 scale (or is it 0-99?).
        /// SearchTermSimilarity must first be set by <see cref="SetSearchTermSimilarity"/>
        /// SearchTermSimilarity is given preference, but ties will go to the item of higher article quality.
        /// </summary>
        /// <param name="other">The <see cref="WikiaSearchItem"/> to compare to</param>
        /// <returns>int, describing the comparison</returns>
        public int CompareTo(WikiaSearchItem other)
        {
            return (this.SearchTermSimilarity*100 + this.Quality) - (other?.SearchTermSimilarity*100 + other?.Quality) ?? 1;
        }

        /// <summary>
        /// Sets the <see cref="SearchTermSimilarity"/> field to the value of the number of characters shared between
        /// the provided search term and this item's <see cref="Title"/> field, from left-to-right (sequential, non-breaking)
        /// </summary>
        /// <param name="search">The search term to compare this item's <see cref="Title"/> against</param>
        public void SetSearchTermSimilarity(string search)
        {
            if (string.IsNullOrEmpty(search) || string.IsNullOrEmpty(this.Title))
            {
                return;
            }

            var score = 0;

            var c1 = this.Title.ToCharArray();
            var c2 = search.ToCharArray();
            var minLength = Math.Min(c1.Length, c2.Length);

            for (var i = 0; i < minLength; i++)
            {
                if (c1[i] != c2[i])
                {
                    break;
                }

                score = i;
            }

            this.SearchTermSimilarity = score;
        }
    }
}
