using BookCollector.Application;
using BookCollector.Dialogs;
using NLog;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System.Linq;
using System.Reactive;

namespace BookCollector.Screens.Notes
{
    public class NotesViewModel : ScreenBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IStateManager state_manager;

        private ObservableCollectionEx<NoteViewModel> _Notes;
        public ObservableCollectionEx<NoteViewModel> Notes
        {
            get { return _Notes; }
            set { this.RaiseAndSetIfChanged(ref _Notes, value); }
        }

        private NoteViewModel _SelectedNote;
        public NoteViewModel SelectedNote
        {
            get { return _SelectedNote; }
            set { this.RaiseAndSetIfChanged(ref _SelectedNote, value); }
        }

        private ReactiveCommand<Unit, Unit> _AddNoteCommand;
        public ReactiveCommand<Unit, Unit> AddNoteCommand
        {
            get { return _AddNoteCommand; }
            set { this.RaiseAndSetIfChanged(ref _AddNoteCommand, value); }
        }

        private ReactiveCommand<NoteViewModel, Unit> _RenameNoteCommand;
        public ReactiveCommand<NoteViewModel, Unit> RenameNoteCommand
        {
            get { return _RenameNoteCommand; }
            set { this.RaiseAndSetIfChanged(ref _RenameNoteCommand, value); }
        }

        private ReactiveCommand<NoteViewModel, Unit> _DeleteNoteCommand;
        public ReactiveCommand<NoteViewModel, Unit> DeleteNoteCommand
        {
            get { return _DeleteNoteCommand; }
            set { this.RaiseAndSetIfChanged(ref _DeleteNoteCommand, value); }
        }

        public NotesViewModel(IStateManager state_manager)
        {
            this.state_manager = state_manager;

            AddNoteCommand = ReactiveCommand.Create(AddNote);
            RenameNoteCommand = ReactiveCommand.Create<NoteViewModel>(RenameNote);
            DeleteNoteCommand = ReactiveCommand.Create<NoteViewModel>(DeleteNote);
        }

        public override void OnActivated()
        {
            Notes = state_manager.CurrentCollection
                                 .Notes
                                 .Select(n => new NoteViewModel(n))
                                 .ToObservableCollectionEx();
            SelectedNote = Notes.FirstOrDefault();
        }

        public override void OnDeactivated()
        {
            Notes.DisposeAll();
            Notes.Clear();
            Notes = null;
            SelectedNote = null;
        }

        private async void AddNote()
        {
            var dialog = new InputDialogViewModel
            {
                Title = "Adding Note",
                Message = "Enter name",
                Hint = "Note Name"
            };
            var result = (bool)await DialogManager.ShowInputDialogAsync(dialog);

            if (result)
            {
                var note = state_manager.CurrentCollection.AddNote(dialog.Input);
                var vm = new NoteViewModel(note);
                Notes.Add(vm);
                SelectedNote = vm;
            }
        }

        private async void RenameNote(NoteViewModel vm)
        {
            logger.Trace($"Renaming note [{vm.Name}]");

            var dialog = new InputDialogViewModel
            {
                Title = "Renaming Note",
                Message = "Enter new name for note",
                Hint = vm.Name
            };
            var result = (bool)await DialogManager.ShowInputDialogAsync(dialog);

            if (true)
            {
                vm.Obj.Name = dialog.Input;
            }
        }

        private async void DeleteNote(NoteViewModel vm)
        {
            logger.Trace($"Deleting note [{vm.Name}]");

            var result = (bool)await DialogManager.ShowPromptDialogAsync("Deleting Note", "Are you sure?");
            if (result)
            {
                state_manager.CurrentCollection.RemoveNote(vm.Obj);
                Notes.Remove(vm);
                SelectedNote = Notes.FirstOrDefault();
            }
        }
    }
}
