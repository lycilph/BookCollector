using BookCollector.Application.Processor;
using BookCollector.Data;
using BookCollector.Goodreads;
using BookCollector.Goodreads.Processes;
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

            var obs1 = this.WhenAnyValue(x => x.state_manager.CurrentCollection).Select(_ => Unit.Default);
            var obs2 = this.WhenAnyObservable(x => x.state_manager.CurrentCollection.Books.CollectionChangedEx).Select(_ => Unit.Default);
            Observable.Merge(obs1, obs2)
                      .Subscribe(_ =>
                      {
                          if (state_manager.CurrentCollection == null)
                              logger.Trace("Background processor - skipping as current collection is null");
                          else
                              CheckCollection();
                      });
        }

        public void Exit()
        {
            logger.Trace("Exiting goodreads controller");
            client.Dispose();
            client = null;
        }

        private void CheckCollection()
        {
            state_manager.CurrentCollection.Books
                .Where(b => !b.Metadata.ContainsKey("GoodreadsWorkId"))
                .Apply(LookupBookInformation);
        }

        private void LookupBookInformation(Book book)
        {
            var process = new BookInformationProcess(client, book, progress, scheduler);
            background_processor.Add(process);
        }
    }
}
