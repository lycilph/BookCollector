using System.Collections.Generic;

namespace GoodreadsPlugin.Data
{
    public class GoodreadsSeries
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SeriesWorksCount { get; set; }
        public string PrimaryWorkCount { get; set; }
        public List<GoodreadsWork> SeriesWorks { get; set; }
    }
}