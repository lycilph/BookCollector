using Core.Data;
using GoodreadsPlugin.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoodreadsPlugin.Import
{
    public static class GoodreadsMapper
    {
        public static Book Map(GoodreadsBook goodreads_book)
        {
            var authors = new List<string> { goodreads_book.Author };
            if (!string.IsNullOrWhiteSpace(goodreads_book.AdditionalAuthors))
                authors.AddRange(goodreads_book.AdditionalAuthors.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()));

            return new Book
            {
                Title = goodreads_book.Title,
                Authors = authors,
                ISBN = goodreads_book.ISBN,
                ISBN13 = goodreads_book.ISBN13,
                Tags = new Dictionary<string, string>
                {
                    {"Bookshelves", goodreads_book.Bookshelves},
                    {"ExclusiveShelf", goodreads_book.ExclusiveShelf},
                    {"Source", goodreads_book.Source},
                    {"GoodreadsBookId", goodreads_book.BookId}
                }
            };
        }

        public static IEnumerable<Book> Map(IEnumerable<GoodreadsBook> goodreads_books)
        {
            return goodreads_books.Select(gb => Map(gb));
        }
    }
}
