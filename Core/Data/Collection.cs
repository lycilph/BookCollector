﻿using Core.Utility;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Core.Data
{
    [DebuggerDisplay("Name = {Name}")]
    [JsonObject(MemberSerialization.OptOut)]
    public class Collection : DirtyTrackingBase
    {
        private string _Filename;
        [JsonIgnore]
        public string Filename
        {
            get { return _Filename; }
            set { this.RaiseAndSetIfChanged(ref _Filename, value); }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private ReactiveList<Book> _Books = new ReactiveList<Book>();
        public ReactiveList<Book> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private ReactiveList<Note> _Notes = new ReactiveList<Note>();
        public ReactiveList<Note> Notes
        {
            get { return _Notes; }
            set { this.RaiseAndSetIfChanged(ref _Notes, value); }
        }

        private ReactiveList<Series> _Series = new ReactiveList<Series>();
        public ReactiveList<Series> Series
        {
            get { return _Series; }
            set { this.RaiseAndSetIfChanged(ref _Series, value); }
        }

        public bool IsBookInCollection(string title, string isbn, string isbn13)
        {
            return Books.Any(b => string.Equals(b.Title, title, StringComparison.InvariantCultureIgnoreCase) ||
                                  string.Equals(b.ISBN, isbn, StringComparison.InvariantCultureIgnoreCase) ||
                                  string.Equals(b.ISBN13, isbn13, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Add(IEnumerable<Book> books)
        {
            Books.AddRange(books);
        }

        public void Add(Series series)
        {
            Series.Add(series);
        }

        public void Add(Note note)
        {
            Notes.Add(note);
        }

        public void Remove(Note note)
        {
            Notes.Remove(note);
        }
    }
}
