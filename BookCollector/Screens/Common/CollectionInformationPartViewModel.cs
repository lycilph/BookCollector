using BookCollector.Application;
using Panda.Infrastructure;
using ReactiveUI;
using System;

namespace BookCollector.Screens.Common
{
    public class CollectionInformationPartViewModel : ScreenBase
    {
        private IStateManager state_manager;

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        public CollectionInformationPartViewModel(IStateManager state_manager)
        {
            this.state_manager = state_manager;

            this.WhenAnyValue(x => x.state_manager.CurrentCollection)
                .Subscribe(x => Name = $"Collection: {x.Name}");
        }
    }
}
