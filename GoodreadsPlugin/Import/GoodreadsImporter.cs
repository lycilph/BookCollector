﻿using CsvHelper.Configuration;
using GoodreadsPlugin.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace GoodreadsPlugin.Import
{
    public static class GoodreadsImporter
    {
        public static List<GoodreadsBook> Import(string filename)
        {
            var configuration = new Configuration()
            {
                PrepareHeaderForMatch = header => header?.Trim().Replace(" ", string.Empty).ToLower(),
                HeaderValidated = null,
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim
            };

            var goodreads_books = new List<GoodreadsBook>();

            try
            {
                using (var sr = new StreamReader(filename))
                using (var csv = new TrimmingCsvReader(sr, configuration))
                {
                    goodreads_books = csv.GetRecords<GoodreadsBook>().ToList();
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