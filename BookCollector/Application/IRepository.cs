using BookCollector.Data;

namespace BookCollector.Application
{
    public interface IRepository
    {
        Collection CreateCollection(string filename);
    }
}