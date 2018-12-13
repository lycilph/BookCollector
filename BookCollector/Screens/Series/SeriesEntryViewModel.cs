using BookCollector.Data;
using Panda.Infrastructure;

namespace BookCollector.Screens.Series
{
    public class SeriesEntryViewModel : ItemViewModel<SeriesEntry>
    {
        public string Title { get { return (Obj.Book != null ? Obj.Book.Title : "[Unknown]"); } }
        public bool MissingInCollection { get { return Obj.MissingInCollection; } }

        public SeriesEntryViewModel(SeriesEntry obj) : base(obj) {}
    }
}
