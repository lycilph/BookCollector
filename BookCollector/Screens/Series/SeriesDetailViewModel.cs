using Core.Data;
using Panda.Infrastructure;
using ReactiveUI;
using System.Collections.Generic;

namespace BookCollector.Screens.Series
{
    public class SeriesDetailViewModel : ItemViewModel<Core.Data.Series>
    {
        public string Title { get { return Obj.Title; } }
        public int EntriesCount { get { return Obj.Entries.Count; } }
        public ReactiveList<SeriesEntry> Entries { get { return Obj.Entries; } }
        public Dictionary<string, string> Tags { get { return Obj.Tags; } }

        public SeriesDetailViewModel(Core.Data.Series obj) : base(obj) {}
    }
}
