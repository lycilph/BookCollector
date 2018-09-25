using Core;
using Core.Application;
using Core.Infrastructure;
using NLog;
using Panda.Dialog;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System.IO;
using System.Linq;

namespace BookCollector.Screens.Collections
{
    public class CollectionsViewModel : ScreenBase, IModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IStateManager state_manager;
        private IRepository repository;
        private IDialogManager dialog_manager;

        public ModuleType Type { get; } = ModuleType.Collections;

        private ReactiveList<RecentCollectionViewModel> _RecentCollections;
        public ReactiveList<RecentCollectionViewModel> RecentCollections
        {
            get { return _RecentCollections; }
            set { this.RaiseAndSetIfChanged(ref _RecentCollections, value); }
        }

        private ReactiveCommand _SelectCollectionCommand;
        public ReactiveCommand SelectCollectionCommand
        {
            get { return _SelectCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _SelectCollectionCommand, value); }
        }

        private ReactiveCommand _RemoveCollectionCommand;
        public ReactiveCommand RemoveCollectionCommand
        {
            get { return _RemoveCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _RemoveCollectionCommand, value); }
        }

        private ReactiveCommand _NewCollectionCommand;
        public ReactiveCommand NewCollectionCommand
        {
            get { return _NewCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _NewCollectionCommand, value); }
        }

        private ReactiveCommand _OpenCollectionCommand;
        public ReactiveCommand OpenCollectionCommand
        {
            get { return _OpenCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _OpenCollectionCommand, value); }
        }

        public CollectionsViewModel(IStateManager state_manager, IRepository repository, IDialogManager dialog_manager)
        {
            DisplayName = "Collections";
            this.state_manager = state_manager;
            this.repository = repository;
            this.dialog_manager = dialog_manager;

            SelectCollectionCommand = ReactiveCommand.Create<RecentCollectionViewModel>(SelectCollection);
            RemoveCollectionCommand = ReactiveCommand.Create<RecentCollectionViewModel>(RemoveCollection);

            NewCollectionCommand = ReactiveCommand.Create(() => NewCollection());
            OpenCollectionCommand = ReactiveCommand.Create(() => OpenCollection());
        }

        public override void OnActivated()
        {
            RecentCollections = state_manager.GetRecentCollections()
                                             .OrderByDescending(c => c.TimeStamp)
                                             .Select(c => new RecentCollectionViewModel(c))
                                             .ToReactiveList();

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

        private void SelectCollection(RecentCollectionViewModel recent_collection)
        {
            logger.Trace($"Selected collection [{recent_collection.Name}]");

            if (recent_collection.Invalid)
            {
                var dialog_result = dialog_manager.ShowPromtDialog("Invalid Collection", "Do you want to remove it from the list?");
                if (dialog_result.Result == true)
                {
                    state_manager.RemoveFromRecentCollections(recent_collection.Obj);
                    RecentCollections.Remove(recent_collection);
                }
            }
            else
            {
                var collection = repository.LoadCollection(recent_collection.Filename);
                state_manager.CurrentCollection = collection;
                MessageBus.Current.SendMessage(ApplicationMessage.CollectionSelected);
            }
        }

        private void RemoveCollection(RecentCollectionViewModel recent_collection)
        {
            logger.Trace($"Removing collection [{recent_collection.Filename}]");

            if (recent_collection.Invalid != true)
            {
                var dialog_result = dialog_manager.ShowPromtDialog("Removing Collection", "Do you want to delete it from disk?");
                if (dialog_result.Result == true)
                {
                    repository.DeleteCollection(recent_collection.Filename);
                }
            }

            state_manager.RemoveFromRecentCollections(recent_collection.Obj);
            RecentCollections.Remove(recent_collection);
        }

        private void NewCollection()
        {
            var dialog_result = dialog_manager.ShowSafeFileDialog("Create New Collection", Constants.CollectionExtension, Constants.CollectionDialogFilter);
            if (dialog_result.Result == true)
            {
                var name = Path.GetFileNameWithoutExtension(dialog_result.Filename);
                var collection = repository.CreateCollection(name);
                state_manager.CurrentCollection = collection;
                MessageBus.Current.SendMessage(ApplicationMessage.CollectionSelected);
            }
        }

        private void OpenCollection()
        {
            var dialog_result = dialog_manager.ShowOpenFileDialog("Open Existing Collection", Constants.CollectionExtension, Constants.CollectionDialogFilter);
            if (dialog_result.Result == true)
            {
                var collection = repository.LoadCollection(dialog_result.Fullpath);
                state_manager.CurrentCollection = collection;
                MessageBus.Current.SendMessage(ApplicationMessage.CollectionSelected);
            }
        }
    }
}
