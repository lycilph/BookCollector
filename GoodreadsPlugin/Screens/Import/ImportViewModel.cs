using Core.Application;
using Core.Infrastructure;
using GoodreadsPlugin.Data;
using GoodreadsPlugin.Import;
using Microsoft.Win32;
using NLog;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System.Linq;

namespace GoodreadsPlugin.Screens.Import
{
    public class ImportViewModel : ScreenBase, IModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IStateManager state_manager;

        public ModuleType Type { get; } = ModuleType.Plugin;

        private string _Filename;
        public string Filename
        {
            get { return _Filename; }
            set { this.RaiseAndSetIfChanged(ref _Filename, value); }
        }

        private ReactiveList<GoodreadsCSVBook> _Books = new ReactiveList<GoodreadsCSVBook>();
        public ReactiveList<GoodreadsCSVBook> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private ReactiveList<ShelfViewModel> _Shelves = new ReactiveList<ShelfViewModel>();
        public ReactiveList<ShelfViewModel> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        private ReactiveCommand _OpenFileCommand;
        public ReactiveCommand OpenFileCommand
        {
            get { return _OpenFileCommand; }
            set { this.RaiseAndSetIfChanged(ref _OpenFileCommand, value); }
        }

        private ReactiveCommand _ImportBooksCommand;
        public ReactiveCommand ImportBooksCommand
        {
            get { return _ImportBooksCommand; }
            set { this.RaiseAndSetIfChanged(ref _ImportBooksCommand, value); }
        }

        private ReactiveCommand _SelectAllCommand;
        public ReactiveCommand SelectAllCommand
        {
            get { return _SelectAllCommand; }
            set { this.RaiseAndSetIfChanged(ref _SelectAllCommand, value); }
        }

        private ReactiveCommand _DeselectAllCommand;
        public ReactiveCommand DeselectAllCommand
        {
            get { return _DeselectAllCommand; }
            set { this.RaiseAndSetIfChanged(ref _DeselectAllCommand, value); }
        }

        public ImportViewModel(IStateManager state_manager)
        {
            DisplayName = "Goodreads Import";
            this.state_manager = state_manager;

            OpenFileCommand = ReactiveCommand.Create(OpenFile);

            var have_imported_books = this.WhenAny(x => x.Books, x => x.Value != null && x.Value.Any());
            ImportBooksCommand = ReactiveCommand.Create(ImportBooks, have_imported_books);

            var have_shelves = this.WhenAny(x => x.Shelves, x => x.Value != null && x.Value.Any());
            SelectAllCommand = ReactiveCommand.Create(() => Shelves.Apply(s => s.Selected = true), have_shelves);
            DeselectAllCommand = ReactiveCommand.Create(() => Shelves.Apply(s => s.Selected = false), have_shelves);
        }

        public override void OnDeactivated()
        {
            Clear();
        }

        private void OpenFile()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                logger.Trace($"Importing from {ofd.FileName}");
                Filename = ofd.FileName;

                Books = GoodreadsImporter.Import(Filename).ToReactiveList();
                // Check if books have already been imported (ie. is already in the collection)
                Books.Apply(b => b.IsDuplicate = state_manager.CurrentCollection.IsBookInCollection(b.Title, b.ISBN, b.ISBN13));

                Shelves = Books.Select(b => b.ExclusiveShelf)
                               .Distinct()
                               .OrderBy(s => s)
                               .Select(s => new ShelfViewModel(s))
                               .ToReactiveList();
            }
        }

        private void ImportBooks()
        {
            var selected_shelves = Shelves.Where(s => s.Selected)
                                          .Select(s => s.Name)
                                          .ToList();
            var books_on_selected_shelves = Books.Where(b => b.IsDuplicate == false && selected_shelves.Contains(b.ExclusiveShelf));
            var imported_books = GoodreadsMapper.Map(books_on_selected_shelves);
            state_manager.CurrentCollection.Add(imported_books);

            Clear();
            MessageBus.Current.SendMessage(ApplicationMessage.NavigateToBooks);
        }

        private void Clear()
        {
            Books = null;
            Shelves = null;
            Filename = string.Empty;
        }
    }
}
