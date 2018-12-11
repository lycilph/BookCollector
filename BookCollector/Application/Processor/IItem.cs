using System.Threading;

namespace BookCollector.Application.Processor
{
    public interface IItem
    {
        void Execute(CancellationToken token);
    }
}
