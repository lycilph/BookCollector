using NLog;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BookCollector.Application.Processor
{
    public class BackgroundProcessor : IBackgroundProcessor
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private BlockingCollection<IProcess> queue = new BlockingCollection<IProcess>();
        private CancellationTokenSource cts;
        private Task processing_task;

        public int Count { get { return queue.Count; } }

        public void Start()
        {
            logger.Trace("Starting background processor");

            cts = new CancellationTokenSource();
            var token = cts.Token;

            processing_task = Task.Run(() =>
            {
                while (!queue.IsCompleted && !token.IsCancellationRequested)
                {
                    try
                    {
                        var process = queue.Take(token);
                        process.Execute(token);
                    }
                    catch (OperationCanceledException e)
                    {
                        logger.Trace($"Got an OperationCanceledException [{e.Message}]");
                    }
                    
                }
            }, token);
        }

        public void Stop()
        {
            logger.Trace("Stopping background processor");

            cts.Cancel();
            processing_task.Wait();
        }

        public void Add(IProcess process)
        {
            queue.Add(process);
        }
    }
}
