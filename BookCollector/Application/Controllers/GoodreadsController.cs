using BookCollector.Application.Processor;
using BookCollector.Data;
using BookCollector.Goodreads;
using BookCollector.Goodreads.Items;
using NLog;
using Panda.Utils;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BookCollector.Application.Controllers
{
    public class GoodreadsController : ReactiveObject, IGoodreadsController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string cache_filename = "cache.json";
        private const string api_secret_filename = "goodreads_api_secret.txt";

        private IBackgroundProcessor background_processor;
        private IStateManager state_manager;
        private GoodreadsClient client;
        private TaskScheduler scheduler;
        private IProgress<string> progress;

        public GoodreadsController(IBackgroundProcessor background_processor, IStateManager state_manager)
        {
            this.background_processor = background_processor;
            this.state_manager = state_manager;

            scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            progress = new Progress<string>(str => logger.Trace(str));
        }

        public void Initialize()
        {
            logger.Trace("Initializing goodreads controller");
            client = new GoodreadsClient(cache_filename, api_secret_filename);

            // Check if the collection was changed
            var obs1 = this.WhenAnyValue(x => x.state_manager.CurrentCollection).Select(_ => Unit.Default);
            // Check if the number of books was changed
            var obs2 = this.WhenAnyObservable(x => x.state_manager.CurrentCollection.Books.CollectionChangedEx).Select(_ => Unit.Default);
            // Check the collection for stuff to process
            Observable.Merge(obs1, obs2)
                      .Where(_ => state_manager.CurrentCollection != null)
                      .Subscribe(_ => CheckCollection());
        }

        public void Exit()
        {
            logger.Trace("Exiting goodreads controller");
            client.Dispose();
            client = null;
        }

        private void CheckCollection()
        {
            // Check for books to look up
            var check_books = CheckBooks();
            // Check for series to look up
            var check_series = CheckSeries();

            // Check for series that needs to be updated
            // Check for series entries that needs to be updated

            // See if collection needs to check again
            if (check_books || check_series)
                background_processor.Add(new CheckCollectionItem(CheckCollection));
        }

        private bool CheckBooks()
        {
            var books_to_check = state_manager.CurrentCollection
                                              .Books
                                              .Where(b => !b.Metadata.ContainsKey("GoodreadsWorkId"))
                                              .ToList();

            foreach (var book in books_to_check)
                background_processor.Add(new BookInformationItem(client, book, progress, scheduler));

            return books_to_check.Any();
        }

        private bool CheckSeries()
        {
            var collection = state_manager.CurrentCollection;
            var all_series_ids = collection.Books.Where(b => b.Metadata.ContainsKey("GoodreadsSeriesId"))
                                                 .Select(b => b.Metadata["GoodreadsSeriesId"])
                                                 .Distinct();
            var known_series_ids = collection.Series.Select(s => s.Metadata["GoodreadsSeriesId"]);
            var ids_to_check = all_series_ids.Except(known_series_ids)
                                                .ToList();

            foreach (var id in ids_to_check)
                background_processor.Add(new SeriesInformationItem(client, id, collection, progress, scheduler));

            return ids_to_check.Any();
        }
    }
}
