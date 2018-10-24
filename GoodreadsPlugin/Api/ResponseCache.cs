using Core.Utility;
using Newtonsoft.Json;
using Panda.Utils;
using RestSharp;
using RestSharp.Deserializers;
using System.Collections.Generic;
using System.IO;

namespace GoodreadsPlugin.Api
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ResponseCache : DirtyTrackingBase
    {
        private string filename;
        [JsonProperty]
        private Dictionary<string, string> cache = new Dictionary<string, string>();

        public ResponseCache(string filename)
        {
            Load(filename);
        }

        public void Load(string filename)
        {
            this.filename = filename;

            if (File.Exists(filename))
                cache = JsonUtils.ReadFromFile<Dictionary<string, string>>(filename);
        }

        public void Save()
        {
            JsonUtils.WriteToFile(filename, cache);
        }

        public bool Contains(string uri)
        {
            return cache.ContainsKey(uri);
        }

        public T Get<T>(string uri)
        {
            var content = cache[uri];
            var rest_response = new RestResponse { Content = content };
            var deserializer = new XmlDeserializer();
            return deserializer.Deserialize<T>(rest_response);
        }

        public void Add(string uri, string content)
        {
            cache.Add(uri, content);
        }
    }
}
