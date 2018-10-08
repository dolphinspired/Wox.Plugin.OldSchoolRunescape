using System;
using System.Drawing;
using System.IO;
using System.Net;
using Wox.Plugin.RuneScapeWiki.Models;

namespace Wox.Plugin.RuneScapeWiki
{
    internal static class MwThumbnails
    {
        private static readonly TimeSpan CacheTimeout = TimeSpan.FromDays(30);

        public static string GetIcoPath(MwSearchResult result, WikiTypeConfig config, PluginInitContext context)
        {
            var dir = context?.CurrentPluginMetadata?.PluginDirectory;  // Where cached images will be stored
            var filename = result.PageImage;                            // The filename of the cached image
            var sourceUrl = result.Thumbnail?.Source;                   // Where to download the image from the wiki

            if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(sourceUrl))
            {
                // If any required data is unavailable, use the default image for this wiki
                // This is expected, as not all pages will have thumbnails
                return config.IcoPath;
            }

            var cacheDir = Path.Combine(dir, config.ImageCacheFolder);
            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }

            var cachedImage = Path.Combine(cacheDir, filename);
            if (File.Exists(cachedImage))
            {
                // If the image is cached, check when it was cached
                var lastUpdate = File.GetLastWriteTimeUtc(cachedImage);
                if (DateTime.UtcNow.Subtract(lastUpdate) <= CacheTimeout)
                {
                    // If the cached image is recent enough, return this path
                    return cachedImage;
                }
            }

            Bitmap downloaded = null;
            Bitmap resized = null;
            try
            {
                // If the cached image is too old or does not exist, download and save it to the cache folder
                downloaded = DownloadThumbnail(sourceUrl);
                resized = new Bitmap(downloaded, 28, 28);
                resized.Save(cachedImage);
            }
            catch (Exception e)
            {
                // If anything goes wrong, just use the default image for the wiki
                return config.IcoPath;
            }
            finally
            {
                downloaded?.Dispose();
                resized?.Dispose();
            }

            return cachedImage;
        }

        private static Bitmap DownloadThumbnail(string url)
        {
            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            var bmp = new Bitmap(stream);

            return bmp;
        }
    }
}
