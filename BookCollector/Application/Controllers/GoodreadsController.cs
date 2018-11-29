using BookCollector.Application.Processor;
using BookCollector.Data;
using BookCollector.Goodreads;
using BookCollector.Goodreads.Processes;
using NLog;
using System.Threading.Tasks;

namespace BookCollector.Application.Controllers
{
    public class GoodreadsController : IGoodreadsController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string cache_filename = "cache.json";
        private const string api_secret_filename = "goodreads_api_secret.txt";

        private IBackgroundProcessor background_processor;
        private GoodreadsClient client;
        private TaskScheduler scheduler;

        public GoodreadsController(IBackgroundProcessor background_processor)
        {
            this.background_processor = background_processor;
            scheduler = TaskScheduler.FromCurrentSynchronizationContext();
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
    }
}
