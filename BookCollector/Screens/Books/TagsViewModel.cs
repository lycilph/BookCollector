using BookCollector.Application;
using Panda.Infrastructure;

namespace BookCollector.Screens.Books
{
    public class TagsViewModel : ScreenBase
    {
        private IStateManager state_manager;

        public TagsViewModel(IStateManager state_manager)
        {
            this.state_manager = state_manager;
        }
    }
}
