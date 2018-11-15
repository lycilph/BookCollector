using BookCollector.Data;
using ReactiveUI;

namespace BookCollector.Application
{
    public class StateManager : ReactiveObject, IStateManager
    {
        private Collection _CurrentCollection;
        public Collection CurrentCollection
        {
            get { return _CurrentCollection; }
            private set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        public void SetCurrentCollection(Collection collection)
        {
            CurrentCollection = collection;
        }
    }
}
