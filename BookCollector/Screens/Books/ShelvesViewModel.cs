using BookCollector.Application;
using BookCollector.Dialogs;
using NLog;
using Panda.Collections;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System.Linq;
using System.Reactive;

namespace BookCollector.Screens.Books
{
    public class ShelvesViewModel : ScreenBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IStateManager state_manager;

        private ObservableCollectionEx<ShelfViewModel> _Shelves;
        public ObservableCollectionEx<ShelfViewModel> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        private ReactiveCommand<Unit, Unit> _AddShelfCommand;
        public ReactiveCommand<Unit, Unit> AddShelfCommand
        {
            get { return _AddShelfCommand; }
            set { this.RaiseAndSetIfChanged(ref _AddShelfCommand, value); }
        }

        private ReactiveCommand<ShelfViewModel, Unit> _RenameShelfCommand;
        public ReactiveCommand<ShelfViewModel, Unit> RenameShelfCommand
        {
            get { return _RenameShelfCommand; }
            set { this.RaiseAndSetIfChanged(ref _RenameShelfCommand, value); }
        }

        private ReactiveCommand<ShelfViewModel, Unit> _DeleteShelfCommand;
        public ReactiveCommand<ShelfViewModel, Unit> DeleteShelfCommand
        {
            get { return _DeleteShelfCommand; }
            set { this.RaiseAndSetIfChanged(ref _DeleteShelfCommand, value); }
        }

        public ShelvesViewModel(IStateManager state_manager)
        {
            this.state_manager = state_manager;

            AddShelfCommand = ReactiveCommand.Create(AddShelf);
            RenameShelfCommand = ReactiveCommand.Create<ShelfViewModel>(RenameShelf);
            DeleteShelfCommand = ReactiveCommand.Create<ShelfViewModel>(DeletShelf);
        }

        public override void OnActivated()
        {
            Shelves = state_manager.CurrentCollection
                                   .Shelves
                                   .Select(s => new ShelfViewModel(s))
                                   .ToObservableCollectionEx();
        }

        public override void OnDeactivated()
        {
            Shelves.DisposeAll();
            Shelves.Clear();
            Shelves = null;
        }

        private async void AddShelf()
        {
            var dialog = new InputDialogViewModel
            {
                Title = "Adding Shelf",
                Message = "Enter name",
                Hint = "Shelf Name"
            };
            var result = (bool)await DialogManager.ShowInputDialogAsync(dialog);

            if (result)
            {
                var shelf = state_manager.CurrentCollection.AddShelf(dialog.Input);
                var vm = new ShelfViewModel(shelf);
                Shelves.Add(vm);
            }
        }

        private async void RenameShelf(ShelfViewModel vm)
        {
            logger.Trace($"Renaming shelf [{vm.Name}]");

            var dialog = new InputDialogViewModel
            {
                Title = "Renaming Shelf",
                Message = "Enter new name for shelf",
                Hint = vm.Name
            };
            var result = (bool)await DialogManager.ShowInputDialogAsync(dialog);

            if (true)
            {
                vm.Obj.Name = dialog.Input;
            }
        }

        private async void DeletShelf(ShelfViewModel vm)
        {
            logger.Trace($"Deleting shelf [{vm.Name}]");

            var result = (bool)await DialogManager.ShowPromptDialogAsync("Deleting Shelf", "All books on the shelf will be moved to the default shelf.\n\rDo you want to continue");
            if (result)
            {
                state_manager.CurrentCollection.RemoveShelf(vm.Obj);
                Shelves.Remove(vm);
            }
        }
    }
}
