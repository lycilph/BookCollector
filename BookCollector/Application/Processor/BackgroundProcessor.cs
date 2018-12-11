using NLog;
using ReactiveUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookCollector.Application.Processor
{
    public class BackgroundProcessor : ReactiveObject, IBackgroundProcessor
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private BlockingCollection<IItem> collection = new BlockingCollection<IItem>();
        private CancellationTokenSource cts;
        private Task processing_task;

        private List<TypeCountPair> _Status;
        public List<TypeCountPair> Status
        {
            get { return _Status; }
            set { this.RaiseAndSetIfChanged(ref _Status, value); }
        }

        public void Start()
        {
            logger.Trace("Starting background processor");

            cts = new CancellationTokenSource();
            var token = cts.Token;

            processing_task = Task.Run(() =>
            {
                while (!collection.IsCompleted && !token.IsCancellationRequested)
                {
                    try
                    {
                        if (collection.TryTake(out IItem item, 250, token))
                            item.Execute(token);
                    }
                    catch (OperationCanceledException e)
                    {
                        logger.Trace($"Got an OperationCanceledException [{e.Message}]");
                    }

                    Status = collection.GroupBy(i => i.GetType())
                                  .Select(g => new TypeCountPair(g.Key, g.Count()))
                                  .ToList();
                }
            }, token);
        }

        public void Stop()
        {
            logger.Trace("Stopping background processor");

            cts.Cancel();
            processing_task.Wait();
        }

        public void Add(IItem item)
        {
            collection.Add(item);
        }

        public void Clear()
        {
            logger.Trace("Clearing background queue");

            Task.Run(() =>
            {
                while (collection.TryTake(out IItem _)) { }
            });
        }
    }
}
