using BookCollector.Application;
using Panda.Infrastructure;
using ReactiveUI;
using System;

namespace BookCollector.Screens.Common
{
    public class CollectionInformationPartViewModel : ScreenBase
    {
        private IStateManager state_manager;

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private string _Books;
        public string Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        public CollectionInformationPartViewModel(IStateManager state_manager)
        {
            this.state_manager = state_manager;
        }

        public override void OnActivated()
        {
            Name = $"Collections: {state_manager.CurrentCollection.Name}";
            Books = $"Books: {state_manager.CurrentCollection.Books.Count}";
        }
    }
}
