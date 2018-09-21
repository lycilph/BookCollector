using Core.Application;
using Core.Data;
using NLog;
using ReactiveUI;
using System;
using System.Collections.Generic;
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
            set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        public StateManager(IRepository repository)
        {
            this.repository = repository;

            // Update settings after collection has changed
            this.WhenAnyValue(x => x.CurrentCollection)
                .Where(x => x != null)
                .Subscribe(x => Settings.AddOrUpdateRecentCollection(x));
        }

        public void Initialize()
        {
            logger.Trace("Initializing state manager");

            Settings = repository.LoadSettings();

            if (Settings.LoadMostRecentCollectionOnStart)
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
