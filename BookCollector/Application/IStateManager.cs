using BookCollector.Data;

namespace BookCollector.Application
{
    public interface IStateManager
    {
        Collection CurrentCollection { get; }

        void SetCurrentCollection(Collection collection);
    }
}