using BookCollector.Screens.Common;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace BookCollector.Screens.Notes
{
    public class NotesModuleViewModel : CollectionModuleBase, INotesModule
    {
        private NotesViewModel _NotesList;
        public NotesViewModel NotesList
        {
            get { return _NotesList; }
            set { this.RaiseAndSetIfChanged(ref _NotesList, value); }
        }

        readonly ObservableAsPropertyHelper<bool> _CanEdit;
        public bool CanEdit { get { return _CanEdit.Value; } }

        public NotesModuleViewModel(ApplicationNavigationPartViewModel application_navigation_part,
                                    CollectionsNavigationPartViewModel collections_navigation_part,
                                    ToolsNavigationPartViewModel tools_navigation_part,
                                    CollectionInformationPartViewModel collection_information_part,
                                    NotesViewModel notes_view_model)
            : base(application_navigation_part, collections_navigation_part, tools_navigation_part, collection_information_part)
        {
            NotesList = notes_view_model;

            var have_selected_note = this.WhenAnyValue(x => x.NotesList.SelectedNote)
                                         .Select(x => x != null);

            _CanEdit = have_selected_note.ToProperty(this, x => x.CanEdit);
        }

        public override void OnActivated()
        {
            base.OnActivated(); // CollectionModuleBase handles activation of common parts
            NotesList.Activate();
        }

        public override void OnDeactivated()
        {
            base.OnDeactivated(); // CollectionModuleBase handles deactivation of common parts
            NotesList.Deactivate();
        }
    }
}
