using System.Collections.Generic;

namespace BookCollector.Application.Processor
{
    public interface IBackgroundProcessor
    {
        List<TypeCountPair> Status { get; }

        void Start();
        void Stop();
        void Add(IProcess process);
        void Clear();
    }
}