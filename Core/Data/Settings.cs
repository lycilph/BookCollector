using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Settings
    {
        public bool LoadMostRecentCollectionOnStart { get; set; } = false;

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
