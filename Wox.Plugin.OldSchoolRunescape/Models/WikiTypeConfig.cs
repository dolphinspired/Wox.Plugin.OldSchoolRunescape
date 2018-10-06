namespace Wox.Plugin.RuneScapeWiki.Models
{
    internal sealed class WikiTypeConfig
    {
        public static WikiTypeConfig Rs = new WikiTypeConfig
        {
            BaseUrl = "https://runescape.wiki",
            IcoPath = @"Images\rs.png"
        };

        public static WikiTypeConfig Osrs = new WikiTypeConfig
        {
            BaseUrl = "https://oldschool.runescape.wiki",
            IcoPath = @"Images\osrs.png"
        };

        public string BaseUrl { get; set; }

        public string IcoPath { get; set; }
    }
}
