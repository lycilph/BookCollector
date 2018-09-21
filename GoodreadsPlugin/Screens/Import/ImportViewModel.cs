using Core.Infrastructure;
using Microsoft.Win32;
using NLog;
using Panda.Infrastructure;
using ReactiveUI;

namespace GoodreadsPlugin.Screens.Import
{
    public class ImportViewModel : ScreenBase, IModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        //private IApplicationController application_controller;
        //private IStateManager state_manager;

        public ModuleType Type { get; } = ModuleType.Plugin;

        private string _Filename;
        public string Filename
        {
            get { return _Filename; }
            set { this.RaiseAndSetIfChanged(ref _Filename, value); }
        }

        //private ReactiveList<GoodreadsBook> _Books = new ReactiveList<GoodreadsBook>();
        //public ReactiveList<GoodreadsBook> Books
        //{
        //    get { return _Books; }
        //    set { this.RaiseAndSetIfChanged(ref _Books, value); }
        //}

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

        public ImportViewModel()
        {
            DisplayName = "Goodreads Import";

            OpenFileCommand = ReactiveCommand.Create(OpenFile);
        }

        //public ImportViewModel(IApplicationController application_controller, IStateManager state_manager)
        //{
        //    DisplayName = "Goodreads Import";
        //    this.application_controller = application_controller;
        //    this.state_manager = state_manager;

        //    OpenFileCommand = ReactiveCommand.Create(OpenFile);

        //    var have_imported_books = this.WhenAny(x => x.Books, x => x.Value != null && x.Value.Any());
        //    ImportBooksCommand = ReactiveCommand.Create(ImportBooks, have_imported_books);
        //}

        //public override void OnDeactivated()
        //{
        //    Clear();
        //}

        private void OpenFile()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                //        logger.Trace($"Importing from {ofd.FileName}");
                //        Filename = ofd.FileName;

                //        Books = GoodreadsImporter.Import(Filename).ToReactiveList();

                //        // Check if books have already been imported (ie. is already in the collection)
                //        Books.Apply(b => b.IsDuplicate = state_manager.IsInCollection(b.Title, b.ISBN, b.ISBN13));
            }
        }

        //private void ImportBooks()
        //{
        //    var imported_books = GoodreadsMapper.Map(Books.Where(b => b.IsDuplicate == false));
        //    Clear();

        //    application_controller.ImportBooks(imported_books);
        //}

        //private void Clear()
        //{
        //    Books.Clear();
        //    Filename = string.Empty;
        //}
    }
}
