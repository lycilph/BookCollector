using BookCollector.Application;
using BookCollector.Application.Messages;
using BookCollector.Goodreads;
using BookCollector.Goodreads.Data;
using BookCollector.Screens.Common;
using BookCollector.Dialogs;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;

namespace BookCollector.Screens.Import
{
    public class ImportModuleViewModel : ScreenBase, IImportModule
    {
        private IStateManager state_manager;

        private string _FullFilename;
        public string FullFilename
        {
            get { return _FullFilename; }
            set { this.RaiseAndSetIfChanged(ref _FullFilename, value); }
        }

        private string _Filename;
        public string Filename
        {
            get { return _Filename; }
            set { this.RaiseAndSetIfChanged(ref _Filename, value); }
        }

        private ObservableCollection<GoodreadsCSVBook> _Books;
        public ObservableCollection<GoodreadsCSVBook> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private ObservableCollection<ShelfViewModel> _Shelves;
        public ObservableCollection<ShelfViewModel> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        private ApplicationNavigationPartViewModel _ApplicationNavigationPart;
        public ApplicationNavigationPartViewModel ApplicationNavigationPart
        {
            get { return _ApplicationNavigationPart; }
            set { this.RaiseAndSetIfChanged(ref _ApplicationNavigationPart, value); }
        }

        private ToolsNavigationPartViewModel _ToolsNavigationPart;
        public ToolsNavigationPartViewModel ToolsNavigationPart
        {
            get { return _ToolsNavigationPart; }
            set { this.RaiseAndSetIfChanged(ref _ToolsNavigationPart, value); }
        }

        private ReactiveCommand<Unit, Unit> _OpenFileCommand;
        public ReactiveCommand<Unit, Unit> OpenFileCommand
        {
            get { return _OpenFileCommand; }
            set { this.RaiseAndSetIfChanged(ref _OpenFileCommand, value); }
        }

        private ReactiveCommand<Unit, Unit> _ImportBooksCommand;
        public ReactiveCommand<Unit, Unit> ImportBooksCommand
        {
            get { return _ImportBooksCommand; }
            set { this.RaiseAndSetIfChanged(ref _ImportBooksCommand, value); }
        }

        private ReactiveCommand<Unit, Unit> _SelectAllShelvesCommand;
        public ReactiveCommand<Unit, Unit> SelectAllShelvesCommand
        {
            get { return _SelectAllShelvesCommand; }
            set { this.RaiseAndSetIfChanged(ref _SelectAllShelvesCommand, value); }
        }

        private ReactiveCommand<Unit, Unit> _DeselectAllShelvesCommand;
        public ReactiveCommand<Unit, Unit> DeselectAllShelvesCommand
        {
            get { return _DeselectAllShelvesCommand; }
            set { this.RaiseAndSetIfChanged(ref _DeselectAllShelvesCommand, value); }
        }

        public ImportModuleViewModel(IStateManager state_manager,
                                     ApplicationNavigationPartViewModel application_navigation_part,
                                     ToolsNavigationPartViewModel tools_navigation_part)
        {
            this.state_manager = state_manager;
            ApplicationNavigationPart = application_navigation_part;
            ToolsNavigationPart = tools_navigation_part;

            this.WhenAnyValue(x => x.FullFilename)
                .Subscribe(x => Filename = Path.GetFileName(x));

            OpenFileCommand = ReactiveCommand.Create(OpenFile);

            var have_imported_books = this.WhenAny(x => x.Books, x => x.Value != null && x.Value.Any());
            ImportBooksCommand = ReactiveCommand.Create(ImportBooks, have_imported_books);

            var have_shelves = this.WhenAny(x => x.Shelves, x => x.Value != null && x.Value.Any());
            SelectAllShelvesCommand = ReactiveCommand.Create(() => Shelves.Apply(s => s.Selected = true), have_shelves);
            DeselectAllShelvesCommand = ReactiveCommand.Create(() => Shelves.Apply(s => s.Selected = false), have_shelves);
        }

        public override void OnActivated()
        {
            ApplicationNavigationPart.Activate();
            ToolsNavigationPart.Activate();
        }

        public override void OnDeactivated()
        {
            ApplicationNavigationPart.Deactivate();
            ToolsNavigationPart.Deactivate();

            Clear();
        }

        private void OpenFile()
        {
            var ext = ".csv";
            var filter = $"CSV file (*{ext})|*{ext}";
            var (result, filename) = DialogManager.ShowOpenFileDialog("Open ", ext, filter);

            if (result == true)
            {
                FullFilename = filename;

                Books = GoodreadsImporter.Import(FullFilename).ToObservableCollection();
                Shelves = Books.Select(b => b.ExclusiveShelf)
                               .Distinct()
                               .OrderBy(s => s)
                               .Select(s => new ShelfViewModel(s))
                               .ToObservableCollection();

                // Check if books have already been imported (ie. is already in the collection)
                Books.Apply(b => b.IsDuplicate = state_manager.CurrentCollection.IsBookInCollection(b.Title, b.ISBN, b.ISBN13));
            }
        }

        private void ImportBooks()
        {
            var selected_shelves = Shelves.Where(s => s.Selected)
                                          .Select(s => s.Name)
                                          .ToList();
            var books_on_selected_shelves = Books.Where(b => b.IsDuplicate == false && selected_shelves.Contains(b.ExclusiveShelf));
            var imported_books = GoodreadsMapper.Map(books_on_selected_shelves).ToList();

            // Temporary - until shelf matching is implemented
            var default_shelf = state_manager.CurrentCollection.GetDefaultshelf();
            imported_books.Apply(b =>
            {
                b.Shelf = default_shelf;
                default_shelf.Add(b);
            });

            state_manager.CurrentCollection.Add(imported_books);

            MessageBus.Current.SendMessage(NavigationMessage.Books);
            MessageBus.Current.SendMessage(new InformationMessage($"{imported_books.Count} books were imported"));
        }

        private void Clear()
        {
            Books = null;
            Shelves = null;
            FullFilename = string.Empty;
        }
    }
}