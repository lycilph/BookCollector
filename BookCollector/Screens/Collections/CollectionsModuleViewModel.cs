using BookCollector.Application;
using BookCollector.Application.Messages;
using BookCollector.Data;
using NLog;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace BookCollector.Screens.Collections
{
    public class CollectionsModuleViewModel : ScreenBase, ICollectionsModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IRepository repository;
        private IStateManager state_manager;

        private ObservableCollection<RecentCollectionViewModel> _RecentCollections;
        public ObservableCollection<RecentCollectionViewModel> RecentCollections
        {
            get { return _RecentCollections; }
            set { this.RaiseAndSetIfChanged(ref _RecentCollections, value); }
        }

        private ReactiveCommand<RecentCollectionViewModel, Unit> _SelectCollectionCommand;
        public ReactiveCommand<RecentCollectionViewModel, Unit> SelectCollectionCommand
        {
            get { return _SelectCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _SelectCollectionCommand, value); }
        }

        private ReactiveCommand<RecentCollectionViewModel, Unit> _RemoveCollectionCommand;
        public ReactiveCommand<RecentCollectionViewModel, Unit> RemoveCollectionCommand
        {
            get { return _RemoveCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _RemoveCollectionCommand, value); }
        }

        private ReactiveCommand<Unit, Unit> _NewCollectionCommand;
        public ReactiveCommand<Unit, Unit> NewCollectionCommand
        {
            get { return _NewCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _NewCollectionCommand, value); }
        }

        private ReactiveCommand<Unit, Unit> _OpenCollectionCommand;
        public ReactiveCommand<Unit, Unit> OpenCollectionCommand
        {
            get { return _OpenCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _OpenCollectionCommand, value); }
        }

        public CollectionsModuleViewModel(IRepository repository, IStateManager state_manager)
        {
            this.repository = repository;
            this.state_manager = state_manager;

            SelectCollectionCommand = ReactiveCommand.Create<RecentCollectionViewModel>(SelectCollection);
            RemoveCollectionCommand = ReactiveCommand.Create<RecentCollectionViewModel>(RemoveCollection);
            NewCollectionCommand = ReactiveCommand.Create(NewCollection);
            OpenCollectionCommand = ReactiveCommand.Create(OpenCollectionAsync);

            var rc = new List<RecentlyOpenedCollection>
            {
                new RecentlyOpenedCollection("ABC"),
                new RecentlyOpenedCollection("DEF"),
                new RecentlyOpenedCollection("GHI")
            };

            RecentCollections = rc.Select(r => new RecentCollectionViewModel(r) { Name = "123" })
                                  .ToObservableCollection();
            RecentCollections.First().Invalid = true;
        }

        private void SelectCollection(RecentCollectionViewModel recent_collection)
        {
            logger.Trace($"Selected collection [{recent_collection.Name}]");
            MessageBus.Current.SendMessage(NavigationMessage.Import);
        }

        private void RemoveCollection(RecentCollectionViewModel recent_collection)
        {
            logger.Trace($"Removing collection [{recent_collection.Filename}]");
        }

        private void NewCollection()
        {
            var filter = $"Collection file (*{Constants.CollectionExtension})|*{Constants.CollectionExtension}";
            var (result, filename) = DialogManager.ShowSaveFileDialog("Create New Collection", Constants.CollectionExtension, filter);

            if (result == true)
            {
                var collection = repository.CreateCollection(filename);
                state_manager.SetCurrentCollection(collection);
                MessageBus.Current.SendMessage(NavigationMessage.Books);
            }
        }

        private async void OpenCollectionAsync()
        {
            await DialogManager.ShowMessageDialogAsync("Test", "Testing");

            var result = await DialogManager.ShowPromptDialogAsync("Info", "Are you sure?");
            var accept = (bool)result;

            if (accept)
                await DialogManager.ShowMessageDialogAsync("Reply", "You accepted");
            else
                await DialogManager.ShowMessageDialogAsync("Reply", "You declined");
        }
    }
}
