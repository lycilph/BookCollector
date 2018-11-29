﻿using BookCollector.Goodreads.Data;
using Panda.Utils;
using RestSharp;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BookCollector.Goodreads
{
    public class GoodreadsClient : DisposableBase
    {
        private readonly string api_key;
        private readonly RestClient client = new RestClient(@"https://www.goodreads.com");
        private readonly ResponseCache cache;

        public GoodreadsClient(string cache_filename, string api_secret_filename)
        {
            var encoded_api_key = GetType().GetResource(api_secret_filename);
            var encoded_bytes = Convert.FromBase64String(encoded_api_key);
            api_key = Encoding.UTF8.GetString(encoded_bytes);

            cache = new ResponseCache(cache_filename);
            cache.IsDirty = false;
        }

        protected override void DisposeInternal()
        {
            if (cache.IsDirty)
                cache.Save();
        }

        public GoodreadsBook GetBookById(string id, CancellationToken token)
        {
            var request = new RestRequest($"book/show/{id}")
                .AddQueryParameter("format", "xml")
                .AddQueryParameter("key", api_key);
            request.RequestFormat = DataFormat.Xml;
            var uri = client.BuildUri(request).ToString();

            return ExecuteRequest<GoodreadsBook>(request, uri, token);
        }

        private T ExecuteRequest<T>(IRestRequest request, string uri, CancellationToken token, TimeSpan? expiry_time = null) where T : new()
        {
            if (cache.IsCached(uri))
            {
                return cache.Get<T>(uri);
            }
            else
            {
                var response = client.Execute<T>(request);
                cache.Add(uri, response.Content, expiry_time);
                Task.Delay(10000, token).ContinueWith(_ => { }).Wait(); // The ContinueWith eats the TaskCancelledException
                return response.Data;
            }
        }
    }
}