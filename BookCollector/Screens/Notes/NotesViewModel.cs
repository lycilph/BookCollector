using Core.Application;
using Core.Data;
using Core.Infrastructure;
using Panda.Dialog;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;

namespace BookCollector.Screens.Notes
{
    public class NotesViewModel : ScreenBase, IModule
    {
        private IStateManager state_manager;
        private IDialogManager dialog_manager;

        public ModuleType Type { get; } = ModuleType.Notes;

        private ReactiveList<NoteViewModel> _Notes;
        public ReactiveList<NoteViewModel> Notes
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

        private ReactiveCommand _AddNoteCommand;
        public ReactiveCommand AddNoteCommand
        {
            get { return _AddNoteCommand; }
            set { this.RaiseAndSetIfChanged(ref _AddNoteCommand, value); }
        }

        private ReactiveCommand _RemoveNoteCommand;
        public ReactiveCommand RemoveNoteCommand
        {
            get { return _RemoveNoteCommand; }
            set { this.RaiseAndSetIfChanged(ref _RemoveNoteCommand, value); }
        }

        private ReactiveCommand _RenameNoteCommand;
        public ReactiveCommand RenameNoteCommand
        {
            get { return _RenameNoteCommand; }
            set { this.RaiseAndSetIfChanged(ref _RenameNoteCommand, value); }
        }

        readonly ObservableAsPropertyHelper<bool> _CanEdit;
        public bool CanEdit { get { return _CanEdit.Value; } }

        public NotesViewModel(IStateManager state_manager, IDialogManager dialog_manager)
        {
            DisplayName = "Notes";
            this.state_manager = state_manager;
            this.dialog_manager = dialog_manager;

            var have_notes = Observable.Merge(this.WhenAny(x => x.Notes, x => x.Value != null && x.Value.Any()),
                                              this.WhenAnyObservable(x => x.Notes.CountChanged)
                                                  .Select(x => x > 0));

            AddNoteCommand = ReactiveCommand.Create(AddNote);
            RemoveNoteCommand = ReactiveCommand.Create(RemoveNote, have_notes);
            RenameNoteCommand = ReactiveCommand.Create(RenameNote, have_notes);

            _CanEdit = have_notes.ToProperty(this, x => x.CanEdit);
        }

        public override void OnActivated()
        {
            Notes = state_manager.CurrentCollection.Notes.Select(n => new NoteViewModel(n)).ToReactiveList();
            SelectedNote = Notes.FirstOrDefault();
        }

        public override void OnDeactivated()
        {
            Notes.DisposeAll();
            Notes = null;
            SelectedNote = null;
        }

        private void AddNote()
        {
            var dialog_result = dialog_manager.ShowInputDialog("New note", "Enter name for note");

            var note_name = "New Note";
            if (dialog_result.Result == true)
                note_name = dialog_result.Input;

            var note = new Note { Name = note_name };
            var vm = new NoteViewModel(note);
            state_manager.CurrentCollection.Add(note);
            Notes.Add(vm);

            SelectedNote = vm;
        }

        private void RemoveNote()
        {
            state_manager.CurrentCollection.Remove(SelectedNote.Obj);
            SelectedNote.Dispose();
            Notes.Remove(SelectedNote);

            SelectedNote = Notes.FirstOrDefault();
        }

        private void RenameNote()
        {
            var dialog_result = dialog_manager.ShowInputDialog("Rename note", "Enter new name for note");
            if (dialog_result.Result == true)
            {
                SelectedNote.Name = dialog_result.Input;
            }
        }
    }
}
