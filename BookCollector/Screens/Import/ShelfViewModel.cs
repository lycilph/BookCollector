using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class ShelfViewModel : ReactiveObject
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set { this.RaiseAndSetIfChanged(ref _Selected, value); }
        }

        public ShelfViewModel(string name)
        {
            Name = name;
            Selected = true;
        }
    }
}
