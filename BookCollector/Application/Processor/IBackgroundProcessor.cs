namespace BookCollector.Application.Processor
{
    public interface IBackgroundProcessor
    {
        int Count { get; }

        void Start();
        void Stop();
        void Add(IProcess process);
    }
}