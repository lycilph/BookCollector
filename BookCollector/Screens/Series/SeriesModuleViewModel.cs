using BookCollector.Application;
using BookCollector.Application.Controllers;
using BookCollector.Application.Processor;
using BookCollector.Dialogs;
using BookCollector.Goodreads.Processes;
using BookCollector.Screens.Common;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BookCollector.Screens.Series
{
    public class SeriesModuleViewModel : CollectionModuleBase, ISeriesModule
    {
        private IStateManager state_manager;
        private IGoodreadsController goodreads_controller;
        private IBackgroundProcessor background_processor;

        private ReactiveCommand<Unit, Unit> _AddCommand;
        public ReactiveCommand<Unit, Unit> AddCommand
        {
            get { return _AddCommand; }
            set { this.RaiseAndSetIfChanged(ref _AddCommand, value); }
        }

        private ReactiveCommand<Unit, Unit> _ClearQueue;
        public ReactiveCommand<Unit, Unit> ClearQueue
        {
            get { return _ClearQueue; }
            set { this.RaiseAndSetIfChanged(ref _ClearQueue, value); }
        }

        private int _Count;
        public int Count
        {
            get { return _Count; }
            set { this.RaiseAndSetIfChanged(ref _Count, value); }
        }

        public SeriesModuleViewModel(ApplicationNavigationPartViewModel application_navigation_part,
                                     CollectionsNavigationPartViewModel collections_navigation_part,
                                     ToolsNavigationPartViewModel tools_navigation_part,
                                     CollectionInformationPartViewModel collection_information_part,
                                     IStateManager state_manager,
                                     IGoodreadsController goodreads_controller, 
                                     IBackgroundProcessor background_processor)
            : base(application_navigation_part, collections_navigation_part, tools_navigation_part, collection_information_part)
        {
            this.state_manager = state_manager;
            this.goodreads_controller = goodreads_controller;
            this.background_processor = background_processor;

            AddCommand = ReactiveCommand.Create(Add);
            ClearQueue = ReactiveCommand.Create(Clear);

            this.WhenAnyValue(x => x.background_processor.Status)
                .Where(s => s != null)
                .Select(s => s.FirstOrDefault(p => p.Type == typeof(DummyProcess)))
                .Subscribe(p => Count = (p == null ? 0 : p.Count));
        }

        private void Add()
        {
            //foreach (var book in state_manager.CurrentCollection.Books.Where(b => !b.Metadata.ContainsKey("GoodreadsWorkId")).Take(10))
            //    goodreads_controller.LookupBookInformation(book);
            for (int i = 0; i < 100; i++)
                goodreads_controller.AddDummy();
        }

        private void Clear()
        {
            background_processor.Clear();
        }
    }
}
