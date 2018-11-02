using System;

namespace GoodreadsPlugin.Api
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
