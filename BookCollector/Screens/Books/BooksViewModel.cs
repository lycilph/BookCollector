using Core.Infrastructure;
using Panda.Infrastructure;

namespace BookCollector.Screens.Books
{
    public class BooksViewModel : ScreenBase, IModule
    {
        public ModuleType Type { get; } = ModuleType.Books;

        public BooksViewModel()
        {
            DisplayName = "Books";
        }
    }
}
