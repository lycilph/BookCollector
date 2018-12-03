using BookCollector.Data;

namespace BookCollector.Application.Controllers
{
    public interface IGoodreadsController
    {
        void Exit();
        void Initialize();

        void LookupBookInformation(Book book);
        void AddDummy();
    }
}