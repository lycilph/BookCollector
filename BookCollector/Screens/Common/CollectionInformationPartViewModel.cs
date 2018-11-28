using BookCollector.Application;
using Panda.Infrastructure;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;

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

        private string _Notes;
        public string Notes
        {
            get { return _Notes; }
            set { this.RaiseAndSetIfChanged(ref _Notes, value); }
        }

        public CollectionInformationPartViewModel(IStateManager state_manager)
        {
            this.state_manager = state_manager;

            this.WhenAnyValue(x => x.state_manager.CurrentCollection.Name)
                .Subscribe(x => Name = $"Collection: {x}");

            var obs1 = this.WhenAnyValue(x => x.state_manager.CurrentCollection).Select(_ => Unit.Default);
            var obs2 = this.WhenAnyObservable(x => x.state_manager.CurrentCollection.Books.CollectionChangedEx).Select(_ => Unit.Default);
            Observable.Merge(obs1, obs2)
                      .Subscribe(_ => Books = $"Books: {state_manager.CurrentCollection.Books.Count}");

            var obs3 = this.WhenAnyObservable(x => x.state_manager.CurrentCollection.Notes.CollectionChangedEx).Select(_ => Unit.Default);
            Observable.Merge(obs1, obs3)
                      .Subscribe(_ => Notes = $"Notes: {state_manager.CurrentCollection.Notes.Count}");
        }
    }
}
