﻿using BookCollector.Data;
using Panda.Infrastructure;
using System.Collections.Generic;

namespace BookCollector.Screens.Books
{
    public class BookDetailViewModel : ItemViewModel<Book>
    {
        public string Title { get { return Obj.Title; } }
        public string Authors { get { return string.Join(",", Obj.Authors); } }
        public string ISBN { get { return Obj.ISBN; } }
        public string ISBN13 { get { return Obj.ISBN13; } }
        public string Description { get { return Obj.Description; } }
        public Dictionary<string, string> Metadata { get { return Obj.Metadata; } }

        public BookDetailViewModel(Book obj) : base(obj) { }
    }
}
