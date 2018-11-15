using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Settings
    {
        public List<RecentlyOpenedCollection> RecentCollections { get; set; } = new List<RecentlyOpenedCollection>();

        public void AddOrUpdateRecentCollection(Collection collection)
        {
            var roc = RecentCollections.Find(c => string.Equals(c.Filename, collection.Filename, StringComparison.InvariantCultureIgnoreCase));
            if (roc == null)
                RecentCollections.Add(new RecentlyOpenedCollection(collection.Filename));
            else
                roc.TimeStamp = DateTime.Now;
        }
    }
}
