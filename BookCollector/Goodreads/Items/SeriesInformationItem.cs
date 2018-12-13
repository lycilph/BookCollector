using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookCollector.Application.Processor;
using BookCollector.Data;

namespace BookCollector.Goodreads.Items
{
    public class SeriesInformationItem : IItem
    {
        private readonly GoodreadsClient client;
        private readonly string id;
        private readonly Collection collection;
        private readonly IProgress<string> log;
        private readonly TaskScheduler scheduler;

        public SeriesInformationItem(GoodreadsClient client, string id, Collection collection, IProgress<string> log, TaskScheduler scheduler)
        {
            this.client = client;
            this.id = id;
            this.collection = collection;
            this.log = log;
            this.scheduler = scheduler;
        }

        public void Execute(CancellationToken token)
        {
            // Check if this has already been processed
            if (collection.Series.Any(s => s.Metadata["GoodreadsSeriesId"] == id))
                return;

            var goodreads_series = client.GetSeriesById(id, token);
            var series = GoodreadsMapper.Map(goodreads_series);
            series.LastChecked = DateTime.Now;

            log.Report($"Processing {series.Title}");

            Task.Factory.StartNew(() => collection.Add(series), token, TaskCreationOptions.DenyChildAttach, scheduler)
                        .Wait(token);
        }
    }
}
