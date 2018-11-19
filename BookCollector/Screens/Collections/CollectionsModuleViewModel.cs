using BookCollector.Application;
using BookCollector.Application.Messages;
using BookCollector.Screens.Dialogs;
using NLog;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
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

        private ReactiveCommand<RecentCollectionViewModel, Unit> _RenameCollectionCommand;
        public ReactiveCommand<RecentCollectionViewModel, Unit> RenameCollectionCommand
        {
            get { return _RenameCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _RenameCollectionCommand, value); }
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
            RenameCollectionCommand = ReactiveCommand.Create<RecentCollectionViewModel>(RenameCollection);
            NewCollectionCommand = ReactiveCommand.Create(NewCollection);
            OpenCollectionCommand = ReactiveCommand.Create(OpenCollection);
        }

        public override void OnActivated()
        {
            RecentCollections = state_manager.GetRecentCollections()
                                             .OrderByDescending(c => c.TimeStamp)
                                             .Select(c => new RecentCollectionViewModel(c))
                                             .ToObservableCollection();

            foreach (var c in RecentCollections)
            {
                var collection = repository.LoadCollection(c.Filename);
                if (collection == null)
                {
                    c.Name = "[Invalid]";
                    c.Invalid = true;
                }
                else
                    c.Name = collection.Name;
            }

            // Save the CurrentCollection in case it is replaced
            if (state_manager.CurrentCollection != null)
                repository.SaveCollection(state_manager.CurrentCollection);
        }

        public override void OnDeactivated()
        {
            RecentCollections.DisposeAll();
            RecentCollections = null;
        }

        private async void SelectCollection (RecentCollectionViewModel recent_collection)
        {
            logger.Trace($"Selected collection [{recent_collection.Name} - {recent_collection.Name}]");

            if (recent_collection.Invalid)
            {
                var result = (bool) await DialogManager.ShowPromptDialogAsync("Invalid Collection", "Do you want to remove it from the list?");
                if (result == true)
                {
                    state_manager.RemoveFromRecentCollections(recent_collection.Obj);
                    RecentCollections.Remove(recent_collection);
                }
            }
            else
            {
                var collection = repository.LoadCollection(recent_collection.Filename);
                state_manager.SetCurrentCollection(collection);
                MessageBus.Current.SendMessage(NavigationMessage.Books);
            }
        }

        private async void RemoveCollection(RecentCollectionViewModel recent_collection)
        {
            logger.Trace($"Removing collection [{recent_collection.Name} - {recent_collection.Filename}]");

            if (recent_collection.Invalid != true)
            {
                var result = (bool)await DialogManager.ShowPromptDialogAsync("Removing Collection", "Do you want to delete it from disk?");
                if (result == true)
                {
                    repository.DeleteCollection(recent_collection.Filename);
                }
            }

            state_manager.RemoveFromRecentCollections(recent_collection.Obj);
            RecentCollections.Remove(recent_collection);
        }

        private async void RenameCollection(RecentCollectionViewModel recent_collection)
        {
            logger.Trace($"Renaming collection [{recent_collection.Name} - {recent_collection.Filename}]");

            if (recent_collection.Invalid)
            {
                logger.Trace("Collection is invalid, so cannot rename it");
                return;
            }

            var vm = new InputDialogViewModel
            {
                Title = "Renaming Collection",
                Message = "Enter new name for collection",
                Hint = recent_collection.Name
            };
            var result = (bool)await DialogManager.ShowInputDialogAsync(vm);

            if (result == true)
            {
                var collection = repository.LoadCollection(recent_collection.Filename);
                if (collection != null)
                {
                    recent_collection.Name = vm.Input;
                    collection.Name = vm.Input;

                    repository.SaveCollection(collection);
                }
            }
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

        private void OpenCollection()
        {
            var filter = $"Collection file (*{Constants.CollectionExtension})|*{Constants.CollectionExtension}";
            var (result, filename) = DialogManager.ShowOpenFileDialog("Open Existing Collection", Constants.CollectionExtension, filter);

            if (result == true)
            {
                var collection = repository.LoadCollection(filename);
                state_manager.SetCurrentCollection(collection);
                MessageBus.Current.SendMessage(NavigationMessage.Books);
            }
        }
    }
}