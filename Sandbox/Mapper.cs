using Core.Data;
using Panda.Utils;
using Sandbox.Model;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox
{
    public static class Mapper
    {
        public static Series Map(GoodreadsSeries goodreads_series)
        {
            var works = Map(goodreads_series.SeriesWorks);
            return new Series
            {
                Title = goodreads_series.Title.Trim(),
                Entries = works.Where(w => w.Position >= 0)
                               .OrderBy(w => w.Position)
                               .ToReactiveList(),
                Tags = new Dictionary<string, string>
                {
                    {"GoodreadsSeriesId", goodreads_series.Id}
                }
            };
        }

        public static SeriesEntry Map(GoodreadsWork goodreads_work)
        {
            bool result = int.TryParse(goodreads_work.UserPosition, out int position);

            return new SeriesEntry
            {
                Position = result ? position : -1,
                Tags = new Dictionary<string, string>
                {
                    {"GoodreadsSeriesWorkId", goodreads_work.Id},
                    {"GoodreadsWorkId", goodreads_work.Work.Id},
                    {"GoodreadsBestBookId", goodreads_work.BestBook.Id}
                }
            };
        }

        public static Book Map(GoodreadsBook goodreads_book)
        {
            return new Book
            {
                Title = goodreads_book.Title
            };
        }

        public static IEnumerable<SeriesEntry> Map(IEnumerable<GoodreadsWork> goodreads_works)
        {
            return goodreads_works.Select(w => Map(w));
        }
    }
}
