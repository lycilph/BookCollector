using BookCollector.Application;
using BookCollector.Data;
using BookCollector.Screens.Dialogs;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System.Linq;
using System.Reactive;

namespace BookCollector.Screens.Books
{
    public class ShelvesViewModel : ScreenBase
    {
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

        public ShelvesViewModel(IStateManager state_manager)
        {
            this.state_manager = state_manager;

            AddShelfCommand = ReactiveCommand.Create(AddShelf);
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

            if (result == true)
            {
                var shelf = state_manager.CurrentCollection.AddShelf(dialog.Input);

                var vm = new ShelfViewModel(shelf);
                Shelves.Add(vm);
            }
        }
    }
}
