using Newtonsoft.Json;
using Panda.Utils;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.IO;

namespace BookCollector.Goodreads
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ResponseCache : DirtyTrackingBase
    {
        private string filename;
        private XmlDeserializer deserializer = new XmlDeserializer();
        [JsonProperty]
        private Dictionary<string, CacheEntry> cache = new Dictionary<string, CacheEntry>();

        public ResponseCache(string filename)
        {
            Load(filename);
        }

        public void Load(string filename)
        {
            this.filename = filename;

            if (File.Exists(filename))
                cache = JsonUtils.ReadFromFile<Dictionary<string, CacheEntry>>(filename);
        }

        public void Save()
        {
            JsonUtils.WriteToFile(filename, cache);
        }

        public bool IsCached(string uri)
        {
            // Check for expiry of the cached data
            if (cache.TryGetValue(uri, out CacheEntry entry))
            {
                return !entry.Expiry.HasValue || DateTime.Now < entry.Expiry;
            }
            else
                return false;
        }

        public T Get<T>(string uri)
        {
            var entry = cache[uri];
            var rest_response = new RestResponse { Content = entry.Content };
            return deserializer.Deserialize<T>(rest_response);
        }

        public void Add(string uri, string content, TimeSpan? expiry_time = null)
        {
            var expiry = DateTime.Now + expiry_time;
            var entry = new CacheEntry(content, expiry);

            if (cache.ContainsKey(uri))
                cache[uri] = entry;
            else
                cache.Add(uri, entry);
        }
    }
}
