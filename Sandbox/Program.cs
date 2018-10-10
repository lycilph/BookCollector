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

            ProcessBooks(collection, sw);
            //FindSeries(collection);
            //ProcessingSeriesForMissingEntries(collection);
            //ShowUnfinishedSeries(collection);

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

        private static void ShowUnfinishedSeries(Collection collection)
        {
            var unfinished_series = collection.Series.Where(s => s.Entries.Any(e => e.Book == null));
            foreach (var series in unfinished_series)
            {
                Console.WriteLine($"Found unfinished series {series.Title}");
                foreach (var entry in series.Entries)
                {
                    if (entry.Book == null)
                        Console.WriteLine($" - Found missing book for entry {entry.Tags["GoodreadsWorkId"]}");
                    else
                        Console.WriteLine($" - Book: {entry.Book.Title}");
                }
            }
        }

        private static void ProcessingSeriesForMissingEntries(Collection collection)
        {
            foreach (var series in collection.Series)
            {
                Console.WriteLine($"Checking series \"{series.Title}\" for missing books");
                foreach (var entry in series.Entries)
                {
                    if (entry.Book == null)
                        entry.Book = collection.Books.FirstOrDefault(b => b.Tags["GoodreadsWorkId"] == entry.Tags["GoodreadsWorkId"]);

                    if (entry.Book == null)
                        Console.WriteLine($" - Found missing book for entry {entry.Tags["GoodreadsWorkId"]}");
                    else
                        Console.WriteLine($" - Book: {entry.Book.Title}");
                }
            }
        }

        private static void ProcessBooks(Collection collection, Stopwatch sw)
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

                    Console.WriteLine($"Elapsed time: {sw.Elapsed}");
                }
            }
        }

        private static void FindSeries(Collection collection)
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

                        collection.Series.Add(Mapper.Map(goodreads_series));

                        if (!cached_hit)
                        {
                            Console.WriteLine("Waiting for Goodreads");
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
        }

        private static void TestClient()
        {
            var goodreads_client = new GoodreadsClient();

            var book = goodreads_client.GetBookByISBN("0765397528");
            Console.WriteLine(book);
            Console.WriteLine();

            book = goodreads_client.GetBookByISBN("0765329107");
            Console.WriteLine(book);
            Console.WriteLine();

            var series = goodreads_client.GetSeriesById("191900");
            Console.WriteLine(series);
            Console.WriteLine();

            series = goodreads_client.GetSeriesByWorkId("53349516");
            Console.WriteLine(series);
            Console.WriteLine();

            goodreads_client.Dispose();
        }
    }
}
