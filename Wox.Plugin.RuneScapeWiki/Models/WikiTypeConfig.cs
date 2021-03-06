﻿namespace Wox.Plugin.RuneScapeWiki.Models
{
    internal sealed class WikiTypeConfig
    {
        public static WikiTypeConfig Rs = new WikiTypeConfig
        {
            WikiName = "RuneScape Wiki",
            BaseUrl = "https://runescape.wiki",
            IcoPath = @"Images\rs.png",
            ImageCacheFolder = @"_cache\img\rsw"
        };

        public static WikiTypeConfig Osrs = new WikiTypeConfig
        {
            WikiName = "Old School RuneScape Wiki",
            BaseUrl = "https://oldschool.runescape.wiki",
            IcoPath = @"Images\osrs.png",
            ImageCacheFolder = @"_cache\img\osw"
        };

        public string WikiName { get; set; }

        public string BaseUrl { get; set; }

        public string IcoPath { get; set; }

        public string ImageCacheFolder { get; set; }
    }
}
