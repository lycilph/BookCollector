using BookCollector.Goodreads.Data;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace BookCollector.Goodreads
{
    public static class GoodreadsImporter
    {
        public static List<GoodreadsCSVBook> Import(string filename)
        {
            var configuration = new Configuration()
            {
                PrepareHeaderForMatch = header => header?.Trim().Replace(" ", string.Empty).ToLower(),
                HeaderValidated = null,
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim
            };

            var goodreads_books = new List<GoodreadsCSVBook>();

            try
            {
                using (var sr = new StreamReader(filename))
                using (var csv = new TrimmingCsvReader(sr, configuration))
                {
                    goodreads_books = csv.GetRecords<GoodreadsCSVBook>().ToList();
                }
            }
            catch (Exception)
            {
                MessageBox.Show($"Couldn't import the file {filename}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return goodreads_books;
        }
    }
}
