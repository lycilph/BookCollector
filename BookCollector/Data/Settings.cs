using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Settings // This is NOT based on DirtyTrackingBase since it will always be dirty if a collection is opened (the timestamp changes every time)
    {
        public bool LoadMostRecentCollectionOnStart { get; set; } = false;
        public int SnackbarMessageDuration { get; set; } = 2;
        public bool EnableShelfMatching { get; set; } = true;
        public bool CreateUnmatchedShelves { get; set; } = true;
        public int MaxEditDistanceForShelfMatching { get; set; } = 3;

        public bool HasRecentCollections { get { return RecentCollections.Any(); } }
        public List<RecentlyOpenedCollection> RecentCollections { get; set; } = new List<RecentlyOpenedCollection>();

        public void AddOrUpdateRecentCollection(Collection collection)
        {
            var roc = RecentCollections.Find(c => string.Equals(c.Filename, collection.Filename, StringComparison.InvariantCultureIgnoreCase));
            if (roc == null)
                RecentCollections.Add(new RecentlyOpenedCollection(collection.Filename));
            else
                roc.TimeStamp = DateTime.Now;
        }

        public RecentlyOpenedCollection GetMostRecentCollection()
        {
            return RecentCollections.OrderByDescending(c => c.TimeStamp)
                                    .FirstOrDefault();
        }

        public void RemoveFromRecentCollections(RecentlyOpenedCollection recent_collection)
        {
            RecentCollections.Remove(recent_collection);
        }
    }
}
