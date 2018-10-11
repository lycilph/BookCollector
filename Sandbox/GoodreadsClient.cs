using RestSharp;
using RestSharp.Deserializers;
using Sandbox.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Sandbox
{
    public class GoodreadsClient : IDisposable
    {
        private const string cache_filename = "cache.json";
        private const string goodreads_api_secret_filename = "api_secret.txt";
        private readonly string api_key;
        private bool disposed = false; // To detect redundant calls
        private Dictionary<string, string> cache = new Dictionary<string, string>();
        private RestClient client = new RestClient(@"https://www.goodreads.com");

        public GoodreadsClient()
        {
            if (File.Exists(cache_filename))
                cache = JsonUtils.ReadFromFile<Dictionary<string, string>>(cache_filename);

            if (File.Exists(goodreads_api_secret_filename))
                api_key = File.ReadAllText(goodreads_api_secret_filename);
            else
                throw new InvalidOperationException($"Cannot find api secret {goodreads_api_secret_filename}");
        }

        private (T result, bool cache_hit) ExecuteRequest<T>(IRestRequest request, string uri) where T : new()
        {
            if (cache.ContainsKey(uri))
            {
                Console.WriteLine($"Found {uri} in cache");
                var content = cache[uri];
                var rest_response = new RestResponse
                {
                    Content = content
                };
                var deserializer = new XmlDeserializer();
                var response = deserializer.Deserialize<T>(rest_response);
                return (response, true);
            }
            else
            {
                Console.WriteLine($"Getting {uri} from the web");
                var response = client.Execute<T>(request);
                cache.Add(uri, response.Content);
                Thread.Sleep(1000);
                return (response.Data, false);
            }
        }

        public (GoodreadsBook result, bool cache_hit) GetBookById(string id)
        {
            var request = new RestRequest($"book/show/{id}")
                .AddQueryParameter("format", "xml")
                .AddQueryParameter("key", api_key);
            request.RequestFormat = DataFormat.Xml;
            var uri = client.BuildUri(request).ToString();

            return ExecuteRequest<GoodreadsBook>(request, uri);
        }

        public (GoodreadsBook result, bool cache_hit) GetBookByISBN(string isbn)
        {
            var request = new RestRequest($"book/isbn/{isbn}")
                .AddQueryParameter("format", "xml")
                .AddQueryParameter("key", api_key);
            request.RequestFormat = DataFormat.Xml;
            var uri = client.BuildUri(request).ToString();

            return ExecuteRequest<GoodreadsBook>(request, uri);
        }

        public (GoodreadsSeries, bool cache_hit) GetSeriesById(string id)
        {
            var request = new RestRequest($"series/{id}")
                .AddQueryParameter("format", "xml")
                .AddQueryParameter("key", api_key);
            request.RequestFormat = DataFormat.Xml;
            var uri = client.BuildUri(request).ToString();

            return ExecuteRequest<GoodreadsSeries>(request, uri);
        }


        public (GoodreadsSeries, bool cache_hit) GetSeriesByWorkId(string id)
        {
            var request = new RestRequest($"series/work/{id}")
                .AddQueryParameter("format", "xml")
                .AddQueryParameter("key", api_key);
            request.RequestFormat = DataFormat.Xml;
            var uri = client.BuildUri(request).ToString();

            return ExecuteRequest<GoodreadsSeries>(request, uri);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                JsonUtils.WriteToFile(cache_filename, cache);
            }

            disposed = true;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
    }
}
