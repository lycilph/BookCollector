using BookCollector.Data;
using BookCollector.Goodreads.Data;
using Panda.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookCollector.Goodreads
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
                Metadata = new ObservableDictionary<string, string>
                {
                    {"Bookshelves", goodreads_csv_book.Bookshelves},
                    {"ExclusiveShelf", goodreads_csv_book.ExclusiveShelf},
                    {"Source", goodreads_csv_book.Source},
                    {"GoodreadsBookId", goodreads_csv_book.BookId}
                }
            };
        }

        public static IEnumerable<Book> Map(IEnumerable<GoodreadsCSVBook> goodreads_books)
        {
            return goodreads_books.Select(b => Map(b));
        }
    }
}
