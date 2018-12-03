using System.Threading;
using System.Threading.Tasks;
using BookCollector.Application.Processor;

namespace BookCollector.Goodreads.Processes
{
    public class DummyProcess : IProcess
    {
        public void Execute(CancellationToken token)
        {
            Task.Delay(500, token).ContinueWith(_ => { }).Wait();
        }
    }
}
