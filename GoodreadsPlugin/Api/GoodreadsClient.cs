﻿using GoodreadsPlugin.Data;
using Panda.Utils;
using RestSharp;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace GoodreadsPlugin.Api
{
    public class GoodreadsClient : DisposableBase
    {
        private const string cache_filename = "cache.json";
        private const string goodreads_api_secret_filename = "api_secret.txt";
        private readonly string api_key;
        private readonly RestClient client = new RestClient(@"https://www.goodreads.com");
        private readonly ResponseCache cache;

        public GoodreadsClient()
        {
            if (File.Exists(goodreads_api_secret_filename))
            {
                var encoded_api_key = File.ReadAllText(goodreads_api_secret_filename);
                var encoded_bytes = Convert.FromBase64String(encoded_api_key);
                api_key = Encoding.UTF8.GetString(encoded_bytes);
            }
            else
                throw new InvalidOperationException($"Cannot find api secret {goodreads_api_secret_filename}");

            cache = new ResponseCache(cache_filename);
            cache.IsDirty = false;
        }

        public GoodreadsBook GetBookById(string id)
        {
            var request = new RestRequest($"book/show/{id}")
                .AddQueryParameter("format", "xml")
                .AddQueryParameter("key", api_key);
            request.RequestFormat = DataFormat.Xml;
            var uri = client.BuildUri(request).ToString();

            return ExecuteRequest<GoodreadsBook>(request, uri);
        }

        public GoodreadsSeries GetSeriesById(string id)
        {
            var request = new RestRequest($"series/{id}")
                .AddQueryParameter("format", "xml")
                .AddQueryParameter("key", api_key);
            request.RequestFormat = DataFormat.Xml;
            var uri = client.BuildUri(request).ToString();

            return ExecuteRequest<GoodreadsSeries>(request, uri);
        }

        private T ExecuteRequest<T>(IRestRequest request, string uri) where T : new()
        {
            if (cache.Contains(uri))
            {
                return cache.Get<T>(uri);
            }
            else
            {
                var response = client.Execute<T>(request);
                cache.Add(uri, response.Content);
                Thread.Sleep(1000);
                return response.Data;
            }
        }

        protected override void DisposeInternal()
        {
            if (cache.IsDirty)
                cache.Save();
        }
    }
}
