using System;

namespace BookCollector.Goodreads.Cache
{
    public class CacheEntry
    {
        public string Content { get; set; }
        public DateTime? Expiry { get; set; }

        public CacheEntry(string content, DateTime? expiry)
        {
            Content = content;
            Expiry = expiry;
        }
    }
}
