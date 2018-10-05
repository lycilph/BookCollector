using Core.Data;
using System;
using System.Linq;
using System.Threading;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = @"C:\Users\mtmk\source\repos\BookCollector\BookCollector\bin\Debug\Test.bcdb";
            var collection = JsonUtils.ReadFromFile<Collection>(filename);
            Console.WriteLine($"Loaded {collection.Books.Count} books from {filename}");

            using (var client = new GoodreadsClient())
            {
                foreach (var book in collection.Books.Skip(2).Take(3))
                {
                    if (book.Tags.ContainsKey("WorkId"))
                    {
                        Console.WriteLine($"{book.Title} - already processed");
                    }
                    else
                    {
                        Console.WriteLine($"Looking up {book.Title}");

                        var (goodreads_book, cached_hit) = client.GetBookByISBN(book.ISBN);

                        book.Tags.Add("WorkId", goodreads_book.Work.Id);
                        Console.WriteLine($"Add tag WorkId with value {goodreads_book.Work.Id}");

                        if (goodreads_book.SeriesWorks.Any())
                        {
                            book.Tags.Add("SeriesWorkId", goodreads_book.SeriesWorks.First().Id);
                            book.Tags.Add("SeriesId", goodreads_book.SeriesWorks.First().Series.Id);

                            Console.WriteLine($"Add tag SeriesWorkId with value {goodreads_book.SeriesWorks.First().Id}");
                            Console.WriteLine($"Add tag SeriesId with value {goodreads_book.SeriesWorks.First().Series.Id}");
                        }

                        if (!cached_hit)
                        {
                            Console.WriteLine("Waiting for Goodreads");
                            Thread.Sleep(1000);
                        }
                    }
                }
            }

            Console.WriteLine($"Saving {filename}");
            JsonUtils.WriteToFile(filename, collection);

            Console.Write("Press any key to continue");
            Console.ReadKey();
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
