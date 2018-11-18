using System;
using System.Diagnostics;

namespace BookCollector.Data
{
    [DebuggerDisplay("Filename = {Filename}")]
    public class RecentlyOpenedCollection
    {
        public DateTime TimeStamp { get; set; }
        public string Filename { get; set; }

        public RecentlyOpenedCollection(string filename)
        {
            Filename = filename;
            TimeStamp = DateTime.Now;
        }
    }
}
