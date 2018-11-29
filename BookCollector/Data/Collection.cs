using Newtonsoft.Json;
using Panda.Collections;
using Panda.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BookCollector.Data
{
    [DebuggerDisplay("Name = {Name}, Books = {Books.Count}")]
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

        private ObservableCollectionEx<Book> _Books = new ObservableCollectionEx<Book>();
        public ObservableCollectionEx<Book> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private ObservableCollectionEx<Note> _Notes = new ObservableCollectionEx<Note>();
        public ObservableCollectionEx<Note> Notes
        {
            get { return _Notes; }
            set { this.RaiseAndSetIfChanged(ref _Notes, value); }
        }

        private ObservableCollectionEx<Shelf> _Shelves = new ObservableCollectionEx<Shelf>();
        public ObservableCollectionEx<Shelf> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        private ObservableCollectionEx<Tag> _Tags = new ObservableCollectionEx<Tag>();
        public ObservableCollectionEx<Tag> Tags
        {
            get { return _Tags; }
            set { this.RaiseAndSetIfChanged(ref _Tags, value); }
        }

        public bool IsBookInCollection(string title, string isbn, string isbn13)
        {
            return Books.Any(b => // Check if title exists in collection
                                  string.Equals(b.Title, title, StringComparison.InvariantCultureIgnoreCase) ||
                                  // Check if isbn are not empty and exists in collection
                                  (!string.IsNullOrWhiteSpace(isbn) &&
                                   !string.IsNullOrWhiteSpace(b.ISBN) &&
                                   string.Equals(b.ISBN, isbn, StringComparison.InvariantCultureIgnoreCase)) ||
                                  // Check if isbn13 are not empty and exists in collection
                                  (!string.IsNullOrWhiteSpace(isbn13) &&
                                   !string.IsNullOrWhiteSpace(b.ISBN13) &&
                                   string.Equals(b.ISBN13, isbn13, StringComparison.InvariantCultureIgnoreCase)));
        }

        public void Add(IEnumerable<Book> books)
        {
            Books.AddRange(books);
        }

        public Shelf AddShelf(string name)
        {
            var shelf = new Shelf { Name = name };
            Shelves.Add(shelf);
            return shelf;
        }

        public void RemoveShelf(Shelf shelf)
        {
            var default_shelf = GetDefaultshelf();
            var books_to_move = shelf.Books.ToList();

            books_to_move.Apply(b => b.Shelf = default_shelf);
            Shelves.Remove(shelf);
        }

        public Shelf GetDefaultshelf()
        {
            return Shelves.First(s => s.IsDefault);
        }

        public Note AddNote(string name)
        {
            var note = new Note { Name = name };
            Notes.Add(note);
            return note;
        }

        public void RemoveNote(Note note)
        {
            Notes.Remove(note);
        }
    }
}
