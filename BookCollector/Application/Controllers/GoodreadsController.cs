using BookCollector.Application.Processor;
using BookCollector.Data;
using BookCollector.Goodreads;
using BookCollector.Goodreads.Processes;
using NLog;
using ReactiveUI;
using System;
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

        public GoodreadsController(IBackgroundProcessor background_processor, IStateManager state_manager)
        {
            this.background_processor = background_processor;
            this.state_manager = state_manager;

            scheduler = TaskScheduler.FromCurrentSynchronizationContext();

            var obs1 = this.WhenAnyValue(x => x.state_manager.CurrentCollection).Select(_ => Unit.Default);
            var obs2 = this.WhenAnyObservable(x => x.state_manager.CurrentCollection.Books.CollectionChangedEx).Select(_ => Unit.Default);
            Observable.Merge(obs1, obs2)
                      .Subscribe(_ => 
                      {
                          if (state_manager.CurrentCollection == null)
                              logger.Trace("Background processor - skipping as current collection is null");
                          else
                          {
                              logger.Trace("Triggering background processor");
                              for (int i = 0; i < 100; i++)
                                  AddDummy();
                          }
                      });
        }

        public void Initialize()
        {
            logger.Trace("Initializing goodreads controller");
            client = new GoodreadsClient(cache_filename, api_secret_filename);
        }

        public void Exit()
        {
            logger.Trace("Exiting goodreads controller");
            client.Dispose();
            client = null;
        }

        public void LookupBookInformation(Book book)
        {
            var process = new BookInformationProcess(client, book, scheduler);
            background_processor.Add(process);
        }

        public void AddDummy()
        {
            background_processor.Add(new DummyProcess());
        }
    }
}
