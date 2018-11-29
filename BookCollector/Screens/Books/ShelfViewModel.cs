using BookCollector.Data;
using Panda.Collections;
using Panda.Infrastructure;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class ShelfViewModel : ItemViewModel<Shelf>
    {
        public string Name { get { return Obj.Name; } }
        public ObservableCollectionEx<Book> Books { get { return Obj.Books; } }
        public bool IsDefault { get { return Obj.IsDefault; } }

        private bool _Selected = true;
        public bool Selected
        {
            get { return _Selected; }
            set { this.RaiseAndSetIfChanged(ref _Selected, value); }
        }

        public ShelfViewModel(Shelf obj) : base(obj) { }
    }
}
