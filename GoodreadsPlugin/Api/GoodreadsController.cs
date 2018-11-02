using Core.Data;
using GoodreadsPlugin.Data;
using Panda.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoodreadsPlugin.Api
{
    public class GoodreadsController
    {
        private GoodreadsClient client;
        private TaskScheduler scheduler;

        public GoodreadsController(GoodreadsClient client)
        {
            this.client = client;

            scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        public async Task ProcessBooksAsync(IEnumerable<Book> books, IProgress<string> progress, CancellationToken token)
        {
            var books_to_process = books.Where(b => !b.Tags.ContainsKey("GoodreadsWorkId")).ToList();
            progress.Report($"Processing {books_to_process.Count} books out of {books.Count()}");

            var total = books_to_process.Count;
            var current = 1;
            foreach (var book in books_to_process)
            {
                progress.Report($"Processing {book.Title} - [{current} of {total}]");

                var goodreads_book = client.GetBookById(book.Tags["GoodreadsBookId"]);
                await Task.Factory.StartNew(() => UpdateBook(book, goodreads_book), CancellationToken.None, TaskCreationOptions.None, scheduler);

                if (token.IsCancellationRequested)
                    break;

                current++;
            }
        }

        public async Task ProcessSeriesAsync(Collection collection, IProgress<string> progress, CancellationToken token)
        {
            await FindNewSeriesAsync(collection, progress, token);

            if (token.IsCancellationRequested)
                return;

            await UpdateSeriesAsync(collection, progress, token);

            if (token.IsCancellationRequested)
                return;

            UpdateSeriesEntries(collection, progress, token);
        }

        private async Task FindNewSeriesAsync(Collection collection, IProgress<string> progress, CancellationToken token)
        {
            progress.Report("Checking for new series");

            var all_series_ids = collection.Books.Where(b => b.Tags.ContainsKey("GoodreadsSeriesId"))
                                                 .Select(b => b.Tags["GoodreadsSeriesId"])
                                                 .Distinct()
                                                 .ToList();
            var known_series_ids = collection.Series.Select(s => s.Tags["GoodreadsSeriesId"])
                                                    .ToList();
            var ids_to_process = all_series_ids.Except(known_series_ids).ToList();

            var total = ids_to_process.Count;
            var current = 1;
            foreach (var id in ids_to_process)
            {
                progress.Report($"Processing {id} - [{current} of {total}]");

                var goodreads_series = client.GetSeriesById(id);
                var series = GoodreadsMapper.Map(goodreads_series);

                progress.Report($" - Found [{series.Title}] containing {series.Entries.Count} entries");

                await Task.Factory.StartNew(() => collection.Add(series), CancellationToken.None, TaskCreationOptions.None, scheduler);

                if (token.IsCancellationRequested)
                    break;

                current++;
            }
        }

        private async Task UpdateSeriesAsync(Collection collection, IProgress<string> progress, CancellationToken token)
        {
            progress.Report("Updating current series");

            // These series were checked more than 7 days ago
            var old_series_to_process = collection.Series.Where(s => s.LastChecked.AddDays(7) < DateTime.Now)
                                                         .ToList();

            var total = old_series_to_process.Count;
            var current = 1;
            foreach (var series in old_series_to_process)
            {
                progress.Report($"Processing {series.Title} - [{current} of {total}]");

                var series_id = series.Tags["GoodreadsSeriesId"];
                var goodreads_series = client.GetSeriesById(series_id);
                var temp_series = GoodreadsMapper.Map(goodreads_series);

                // Find and add missing entries
                var missing_entries = temp_series.Entries.Except(series.Entries, (a, b) => a.Tags["GoodreadsBestBookId"] == b.Tags["GoodreadsBestBookId"])
                                                         .ToList();
                if (missing_entries.Any())
                {
                    progress.Report($" - Adding {missing_entries.Count} entries to {series.Title}");
                    await Task.Factory.StartNew(() => series.Entries.AddRange(missing_entries), CancellationToken.None, TaskCreationOptions.None, scheduler);
                }

                // Remember to update the last_checked property here
                series.LastChecked = DateTime.Now;

                if (token.IsCancellationRequested)
                    break;

                current++;
            }
        }

        private void UpdateSeriesEntries(Collection collection, IProgress<string> progress, CancellationToken token)
        {
            progress.Report("Updating series entries");

            foreach (var series in collection.Series)
            {
                foreach (var entry in series.Entries.Where(e => e.MissingInCollection))
                {
                    // Try to find the book in the collection
                    var book = collection.Books.FirstOrDefault(b => b.Tags["GoodreadsWorkId"] == entry.Tags["GoodreadsWorkId"]);
                    if (book != null)
                    {
                        progress.Report($" - Found {book.Title} already in the collection");

                        // We found the book in the collection
                        entry.Book = book;
                        entry.MissingInCollection = false;
                    }
                    else
                    {
                        // We did not find it in the collection, so look it up (if this has not already been done)
                        if (entry.Book == null)
                        {
                            progress.Report($" - Processing {entry.Tags["GoodreadsBestBookId"]}");

                            var goodreads_book = client.GetBookById(entry.Tags["GoodreadsBestBookId"]);
                            book = GoodreadsMapper.Map(goodreads_book);
                            entry.Book = book;

                            progress.Report($" - - Found [{book.Title}]");
                        }
                    }

                    if (token.IsCancellationRequested)
                        break;
                }
            }
        }

        private void UpdateBook(Book book, GoodreadsBook goodreads_book)
        {
            book.Tags.Add("GoodreadsWorkId", goodreads_book.Work.Id);

            if (!string.IsNullOrWhiteSpace(goodreads_book.Description))
                book.Description = goodreads_book.Description;

            if (goodreads_book.SeriesWorks.Any())
            {
                book.Tags.Add("GoodreadsSeriesWorkId", goodreads_book.SeriesWorks.First().Id);
                book.Tags.Add("GoodreadsSeriesId", goodreads_book.SeriesWorks.First().Series.Id);
            }
        }
    }
}
