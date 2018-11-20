using BookCollector.Data;
using NLog;
using ReactiveUI;
using System.Collections.Generic;

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
        }

        public void Initialize()
        {
            logger.Trace("Initializing state manager");

            Settings = repository.LoadSettings();

            if (Settings.LoadMostRecentCollectionOnStart && Settings.HasRecentCollections)
            {
                var recent_collection = Settings.GetMostRecentCollection();
                CurrentCollection = repository.LoadCollection(recent_collection.Filename);
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
