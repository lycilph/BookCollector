using BookCollector.Data;
using System.Collections.Generic;

namespace BookCollector.Application
{
    public interface IStateManager
    {
        Settings Settings { get; }
        Collection CurrentCollection { get; }

        void Initialize();
        void Exit();

        void SetCurrentCollection(Collection collection);
        void SaveCurrentCollection();

        List<RecentlyOpenedCollection> GetRecentCollections();
        void RemoveFromRecentCollections(RecentlyOpenedCollection recent_collection);
    }
}