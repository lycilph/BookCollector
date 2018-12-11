using System;
using System.Threading;
using BookCollector.Application.Processor;

namespace BookCollector.Goodreads.Items
{
    public class CheckCollectionItem : IItem
    {
        private Action action;

        public CheckCollectionItem(Action action)
        {
            this.action = action;
        }

        public void Execute(CancellationToken token)
        {
            action();
        }
    }
}
