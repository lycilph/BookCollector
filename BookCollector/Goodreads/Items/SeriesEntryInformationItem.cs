using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookCollector.Application.Processor;
using BookCollector.Data;

namespace BookCollector.Goodreads.Items
{
    public class SeriesEntryInformationItem : IItem
    {
        private readonly GoodreadsClient client;
        private readonly SeriesEntry entry;
        private readonly Collection collection;
        private readonly IProgress<string> log;
        private readonly TaskScheduler scheduler;

        public SeriesEntryInformationItem(GoodreadsClient client, SeriesEntry entry, Collection collection, IProgress<string> log, TaskScheduler scheduler)
        {
            this.client = client;
            this.entry = entry;
            this.collection = collection;
            this.log = log;
            this.scheduler = scheduler;
        }

        public void Execute(CancellationToken token)
        {
            var book = collection.Books.FirstOrDefault(b => b.Metadata["GoodreadsWorkId"] == entry.Metadata["GoodreadsWorkId"]);
            var found_in_collection = true;

            if (book == null)
            {
                var goodreads_book = client.GetBookById(entry.Metadata["GoodreadsBestBookId"], token);
                book = GoodreadsMapper.Map(goodreads_book);
                found_in_collection = false;
            }

            log.Report($"Processing {book.Title}");

            Task.Factory.StartNew(() => Update(book, found_in_collection), token, TaskCreationOptions.DenyChildAttach, scheduler)
                        .Wait(token);
        }

        private void Update(Book book, bool found_in_collection)
        {
            entry.Book = book;
            entry.MissingInCollection = !found_in_collection;
            entry.LastChecked = DateTime.Now;
        }
    }
}
