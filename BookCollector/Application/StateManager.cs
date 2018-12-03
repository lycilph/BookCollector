using BookCollector.Application.Messages;
using BookCollector.Data;
using NLog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace BookCollector.Application
{
    public class StateManager : ReactiveObject, IStateManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IRepository repository;

        private Settings _Settings;
        public Settings Settings
        {
            get { return _Settings; }
            private set { this.RaiseAndSetIfChanged(ref _Settings, value); }
        }

        private Collection _CurrentCollection;
        public Collection CurrentCollection
        {
            get { return _CurrentCollection; }
            private set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        public StateManager(IRepository repository)
        {
            this.repository = repository;

            // This is used when reindexing the collection for the search engine
            var collection_changed = this.WhenAnyValue(x => x.CurrentCollection);
            var books_changed = this.WhenAnyObservable(x => x.CurrentCollection.Books.SomethingChanged);
            var obs = Observable.Merge(collection_changed.Select(_ => Unit.Default),
                                       books_changed.Select(_ => Unit.Default))
                                .Select(_ => ApplicationMessage.CollectionChanged)
                                .Sample(TimeSpan.FromMilliseconds(500));
            MessageBus.Current.RegisterMessageSource(obs);
        }

        public void Initialize()
        {
            logger.Trace("Initializing state manager");

            Settings = repository.LoadSettings();

            if (Settings.LoadMostRecentCollectionOnStart && Settings.HasRecentCollections)
            {
                var recent_collection = Settings.GetMostRecentCollection();
                var collection = repository.LoadCollection(recent_collection.Filename);
                SetCurrentCollection(collection);
            }
        }

        public void Exit()
        {
            logger.Trace("Exiting state manager");

            repository.SaveCollection(CurrentCollection);
            repository.SaveSettings(Settings);
        }

        public void SetCurrentCollection(Collection collection)
        {
            CurrentCollection = collection;

            if (collection != null)
                Settings.AddOrUpdateRecentCollection(collection);
        }

        public void SaveCurrentCollection()
        {
            if (CurrentCollection != null)
                repository.SaveCollection(CurrentCollection);
        }

        public List<RecentlyOpenedCollection> GetRecentCollections()
        {
            return Settings.RecentCollections;
        }

        public void RemoveFromRecentCollections(RecentlyOpenedCollection recent_collection)
        {
            Settings.RemoveFromRecentCollections(recent_collection);
        }
    }
}
