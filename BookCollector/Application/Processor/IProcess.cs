using System.Threading;

namespace BookCollector.Application.Processor
{
    public interface IProcess
    {
        void Execute(CancellationToken token);
    }
}
