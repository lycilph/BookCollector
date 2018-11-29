using System.Collections.Generic;

namespace BookCollector.Goodreads.Data
{
    public class GoodreadsBook
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public GoodreadsWork Work { get; set; }
        public List<GoodreadsWork> SeriesWorks { get; set; }
    }
}
