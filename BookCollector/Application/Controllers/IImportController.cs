using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Application.Controllers
{
    public interface IImportController
    {
        void Import(List<Book> books);
    }
}