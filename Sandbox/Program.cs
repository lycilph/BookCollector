using Core.Data;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = @"C:\Users\mtmk\source\repos\BookCollector\BookCollector\bin\Debug\My_Cool_Collection.bcdb";
            var collection = JsonUtils.ReadFromFile<Collection>(filename);
            collection.IsDirty = false;
            Console.WriteLine($"Loaded {collection.Books.Count} books from {filename}");

            var sw = Stopwatch.StartNew();

            ProcessBooks(collection);
            ProcessSeries(collection);
            ProcessMissingBooksInSeries(collection);

            if (collection.IsDirty)
            {
                Console.WriteLine($"Saving {filename}");
                JsonUtils.WriteToFile(filename, collection);
            }

            sw.Stop();
            Console.WriteLine($"Total elapsed time: {sw.Elapsed}");

            Console.Write("Press any key to continue");
            Console.ReadKey();
        }

        private static void ProcessBooks(Collection collection)
        {
            using (var client = new GoodreadsClient())
            {
                foreach (var book in collection.Books)
                {
                    if (book.Tags.ContainsKey("GoodreadsWorkId"))
                    {
                        Console.WriteLine($"{book.Title} - already processed");
                    }
                    else
                    {
                        Console.WriteLine($"Looking up {book.Title} - [{book.Tags["GoodreadsBookId"]}]");

                        var book_id = book.Tags["GoodreadsBookId"];
                        var (goodreads_book, cached_hit) = client.GetBookById(book_id);

                        book.Tags.Add("GoodreadsWorkId", goodreads_book.Work.Id);
                        Console.WriteLine($"Added tag GoodreadsWorkId with value {goodreads_book.Work.Id}");

                        if (!string.IsNullOrWhiteSpace(goodreads_book.Description))
                        {
                            book.Description = goodreads_book.Description;
                            Console.WriteLine($"Added description to book");
                        }

                        if (goodreads_book.SeriesWorks.Any())
                        {
                            book.Tags.Add("GoodreadsSeriesWorkId", goodreads_book.SeriesWorks.First().Id);
                            book.Tags.Add("GoodreadsSeriesId", goodreads_book.SeriesWorks.First().Series.Id);

                            Console.WriteLine($"Added tag GoodreadsSeriesWorkId with value {goodreads_book.SeriesWorks.First().Id}");
                            Console.WriteLine($"Added tag GoodreadsSeriesId with value {goodreads_book.SeriesWorks.First().Series.Id}");
                        }

                        if (!cached_hit)
                        {
                            Console.WriteLine("Waiting for Goodreads");
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
        }

        private static void ProcessSeries(Collection collection)
        {
            var series_ids = collection.Books.Where(b => b.Tags.ContainsKey("GoodreadsSeriesId"))
                                             .Select(b => b.Tags["GoodreadsSeriesId"])
                                             .Distinct()
                                             .ToList();
            Console.WriteLine($"Found {series_ids.Count} series");

            using (var client = new GoodreadsClient())
            {
                foreach (var id in series_ids)
                {
                    var series = collection.Series.FirstOrDefault(s => s.Tags.ContainsKey("GoodreadsSeriesId") && s.Tags["GoodreadsSeriesId"] == id);

                    if (series != null)
                    {
                        Console.WriteLine($"Series {series.Title} - already processed");
                    }
                    else
                    {
                        Console.WriteLine($"Looking up series with id {id}");

                        var (goodreads_series, cached_hit) = client.GetSeriesById(id);
                        Console.WriteLine($"Found \"{goodreads_series.Title.Trim()}\" - primary count {goodreads_series.PrimaryWorkCount}");

                        series = Mapper.Map(goodreads_series);
                        collection.Series.Add(series);

                        if (!cached_hit)
                        {
                            Console.WriteLine("Waiting for Goodreads");
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
        }

        private static void ProcessMissingBooksInSeries(Collection collection)
        {
            var unfinished_series = collection.Series.Where(s => s.Entries.Any(e => e.MissingInCollection)).ToList();
            var complete_series = collection.Series.Except(unfinished_series);
            Console.WriteLine($"Found {unfinished_series.Count} series with missing entries out of {collection.Series.Count} total series in collection");

            Console.WriteLine("Unfinished series");
            foreach (var series in unfinished_series)
            {
                Console.WriteLine($"Found unfinished series {series.Title}");
                foreach (var entry in series.Entries)
                {
                    if (entry.MissingInCollection)
                    {
                        // Try to find the book in the collection
                        var book = collection.Books.FirstOrDefault(b => b.Tags["GoodreadsWorkId"] == entry.Tags["GoodreadsWorkId"]);
                        if (book != null)
                        {
                            // We found the book in the collection
                            entry.Book = book;
                            entry.MissingInCollection = false;
                            Console.WriteLine($" - Book: {entry.Book.Title}");
                        }
                        else
                        {
                            // We did not find it in the collection, so look it up (if this has not already been done)
                            if (entry.Book == null)
                            {
                                using (var client = new GoodreadsClient())
                                {
                                    var book_id = entry.Tags["GoodreadsBestBookId"];
                                    var (goodreads_book, cached_hit) = client.GetBookById(book_id);

                                    entry.Book = Mapper.Map(goodreads_book);

                                    if (!cached_hit)
                                    {
                                        Console.WriteLine("Waiting for Goodreads");
                                        Thread.Sleep(1000);
                                    }
                                }
                            }

                            Console.WriteLine($" - Book: {entry.Book.Title} [missing in collection]");
                        }

                    }
                    else
                    {
                        Console.WriteLine($" - Book: {entry.Book.Title}");
                    }
                }
            }

            Console.WriteLine("Complete series");
            foreach (var series in complete_series)
            {
                Console.WriteLine($"Found completed series {series.Title}");

                if (!series.Entries.Any())
                    Console.WriteLine($"No entries found in series {series.Title} - [{series.Tags["GoodreadsSeriesId"]}]");

                foreach (var entry in series.Entries)
                {
                    Console.WriteLine($" - Book: {entry.Book.Title}");
                }
            }
        }
    }
}
