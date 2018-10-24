namespace GoodreadsPlugin.Data
{
    // This class models a work as defined by the Goodreads API.
    // A work is the root concept of something written. Each book
    // is a published edition of a piece of work. Most work properties
    // are aggregate information over all the editions of a work.
    public class GoodreadsWork
    {
        public string Id { get; set; }
        public string UserPosition { get; set; }
        public GoodreadsSeries Series { get; set; }
        public GoodreadsWork Work { get; set; }
        public GoodreadsBook BestBook { get; set; }
    }
}
