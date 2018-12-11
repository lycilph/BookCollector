using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookCollector.Application.Processor;
using BookCollector.Data;
using BookCollector.Goodreads.Data;

namespace BookCollector.Goodreads.Items
{
    public class BookInformationItem : IItem
    {
        private readonly GoodreadsClient client;
        private readonly Book book;
        private readonly IProgress<string> log;
        private readonly TaskScheduler scheduler;

        public BookInformationItem(GoodreadsClient client, Book book, IProgress<string> log, TaskScheduler scheduler)
        {
            this.client = client;
            this.book = book;
            this.log = log;
            this.scheduler = scheduler;
        }

        public void Execute(CancellationToken token)
        {
            // Check if this has already been processed
            if (book.Metadata.ContainsKey("GoodreadsWorkId"))
                return;

            log.Report($"Processing {book.Title}");

            var id = book.Metadata["GoodreadsBookId"];
            var goodreads_book = client.GetBookById(id, token);

            Task.Factory.StartNew(() => UpdateBook(book, goodreads_book), token, TaskCreationOptions.DenyChildAttach, scheduler);
        }

        private void UpdateBook(Book book, GoodreadsBook goodreads_book)
        {
            book.Metadata.Add("GoodreadsWorkId", goodreads_book.Work.Id);

            if (!string.IsNullOrWhiteSpace(goodreads_book.Description))
                book.Description = goodreads_book.Description;

            if (goodreads_book.SeriesWorks.Any())
            {
                book.Metadata.Add("GoodreadsSeriesWorkId", goodreads_book.SeriesWorks.First().Id);
                book.Metadata.Add("GoodreadsSeriesId", goodreads_book.SeriesWorks.First().Series.Id);
            }
        }
    }
}
