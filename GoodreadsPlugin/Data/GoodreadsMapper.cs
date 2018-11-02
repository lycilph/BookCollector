using Core.Data;
using Panda.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoodreadsPlugin.Data
{
    public static class GoodreadsMapper
    {
        public static Book Map(GoodreadsCSVBook goodreads_csv_book)
        {
            var authors = new List<string> { goodreads_csv_book.Author };
            if (!string.IsNullOrWhiteSpace(goodreads_csv_book.AdditionalAuthors))
                authors.AddRange(goodreads_csv_book.AdditionalAuthors.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()));

            return new Book
            {
                Title = goodreads_csv_book.Title,
                Authors = authors,
                ISBN = goodreads_csv_book.ISBN,
                ISBN13 = goodreads_csv_book.ISBN13,
                Tags = new Dictionary<string, string>
                {
                    {"Bookshelves", goodreads_csv_book.Bookshelves},
                    {"ExclusiveShelf", goodreads_csv_book.ExclusiveShelf},
                    {"Source", goodreads_csv_book.Source},
                    {"GoodreadsBookId", goodreads_csv_book.BookId}
                }
            };
        }

        public static Series Map(GoodreadsSeries goodreads_series)
        {
            var works = Map(goodreads_series.SeriesWorks);
            return new Series
            {
                Title = goodreads_series.Title.Trim(),
                LastChecked = DateTime.Now,
                Entries = works.Where(w => w.Position >= 0)
                               .OrderBy(w => w.Position)
                               .ToReactiveList(),
                Tags = new Dictionary<string, string>
                {
                    {"GoodreadsSeriesId", goodreads_series.Id}
                }
            };
        }

        public static SeriesEntry Map(GoodreadsWork goodreads_work)
        {
            bool result = int.TryParse(goodreads_work.UserPosition, out int position);

            return new SeriesEntry
            {
                Position = result ? position : -1,
                Tags = new Dictionary<string, string>
                {
                    {"GoodreadsSeriesWorkId", goodreads_work.Id},
                    {"GoodreadsWorkId", goodreads_work.Work.Id},
                    {"GoodreadsBestBookId", goodreads_work.BestBook.Id}
                }
            };
        }

        public static Book Map(GoodreadsBook goodreads_book)
        {
            return new Book
            {
                Title = goodreads_book.Title
            };
        }

        public static IEnumerable<Book> Map(IEnumerable<GoodreadsCSVBook> goodreads_books)
        {
            return goodreads_books.Select(b => Map(b));
        }

        public static IEnumerable<SeriesEntry> Map(IEnumerable<GoodreadsWork> goodreads_works)
        {
            return goodreads_works.Select(w => Map(w));
        }
    }
}
