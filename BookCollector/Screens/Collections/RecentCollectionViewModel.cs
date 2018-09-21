using Core.Data;
using Panda.Infrastructure;
using ReactiveUI;

namespace BookCollector.Screens.Collections
{
    public class RecentCollectionViewModel : ItemViewModel<RecentlyOpenedCollection>
    {
        public string Filename { get { return Obj.Filename; } }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private bool _Invalid = false;
        public bool Invalid
        {
            get { return _Invalid; }
            set { this.RaiseAndSetIfChanged(ref _Invalid, value); }
        }

        public RecentCollectionViewModel(RecentlyOpenedCollection obj) : base(obj) {}
    }
}
