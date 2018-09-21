using Core.Data;
using System.Collections.Generic;

namespace Core.Application
{
    public interface IStateManager
    {
        Settings Settings { get; }
        Collection CurrentCollection { get; set; }

        void Initialize();
        void Exit();

        List<RecentlyOpenedCollection> GetRecentCollections();
        void RemoveFromRecentCollections(RecentlyOpenedCollection recent_collection);
    }
}