using BookCollector.Data;
using System.IO;

namespace BookCollector.Application
{
    public class Repository : IRepository
    {
        public Collection CreateCollection(string filename)
        {
            var collection = new Collection
            {
                Name = Path.GetFileNameWithoutExtension(filename),
                Filename = filename
            };
            return collection;
        }
    }
}
