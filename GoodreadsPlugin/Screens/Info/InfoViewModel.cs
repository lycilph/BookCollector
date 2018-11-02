using Core.Application;
using Core.Infrastructure;
using GoodreadsPlugin.Api;
using NLog;
using Panda.Infrastructure;
using ReactiveUI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoodreadsPlugin.Screens.Info
{
    public class InfoViewModel : ScreenBase, IModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IStateManager state_manager;
        private GoodreadsClient goodreads_client;
        private GoodreadsController goodreads_controller;
        private CancellationTokenSource cancellation_token_source;
        private Task current_task;
        private IProgress<string> progress;

        public ModuleType Type { get; } = ModuleType.Plugin;

        private ReactiveList<string> _Messages = new ReactiveList<string>();
        public ReactiveList<string> Messages
        {
            get { return _Messages; }
            set { this.RaiseAndSetIfChanged(ref _Messages, value); }
        }

        private ReactiveCommand _ProcessBooksCommand;
        public ReactiveCommand ProcessBooksCommand
        {
            get { return _ProcessBooksCommand; }
            set { this.RaiseAndSetIfChanged(ref _ProcessBooksCommand, value); }
        }

        private ReactiveCommand _ProcessSeriesCommand;
        public ReactiveCommand ProcessSeriesCommand
        {
            get { return _ProcessSeriesCommand; }
            set { this.RaiseAndSetIfChanged(ref _ProcessSeriesCommand, value); }
        }

        private ReactiveCommand _StopCommand;
        public ReactiveCommand StopCommand
        {
            get { return _StopCommand; }
            set { this.RaiseAndSetIfChanged(ref _StopCommand, value); }
        }

        private ReactiveCommand _ClearMessagesCommand;
        public ReactiveCommand ClearMessagesCommand
        {
            get { return _ClearMessagesCommand; }
            set { this.RaiseAndSetIfChanged(ref _ClearMessagesCommand, value); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        public InfoViewModel(IStateManager state_manager)
        {
            DisplayName = "Goodreads Info";
            this.state_manager = state_manager;

            ProcessBooksCommand = ReactiveCommand.Create(ProcessBooks);
            ProcessSeriesCommand = ReactiveCommand.Create(ProcessSeries);
            StopCommand = ReactiveCommand.Create(Stop);
            ClearMessagesCommand = ReactiveCommand.Create(() => Messages.Clear());

            progress = new Progress<string>(Messages.Add);
        }

        public override void OnActivated()
        {
            goodreads_client = new GoodreadsClient();
            goodreads_controller = new GoodreadsController(goodreads_client);
        }

        public override void OnDeactivated()
        {
            if (current_task != null)
            {
                Stop();
                current_task.Wait();
                current_task = null;
            }

            goodreads_controller = null;

            goodreads_client.Dispose();
            goodreads_client = null;
        }

        private void ProcessBooks()
        {
            IsBusy = true;
            Messages.Clear();

            cancellation_token_source = new CancellationTokenSource();
            var token = cancellation_token_source.Token;

            var books = state_manager.CurrentCollection.Books;

            current_task = Task.Run(async () =>
            {
                await goodreads_controller.ProcessBooksAsync(books, progress, token);
            }, token);

            current_task.ContinueWith(parent => 
            {
                IsBusy = false;
                progress.Report("Done");
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ProcessSeries()
        {
            IsBusy = true;
            Messages.Clear();

            cancellation_token_source = new CancellationTokenSource();
            var token = cancellation_token_source.Token;

            var collection = state_manager.CurrentCollection;

            current_task = Task.Run(async () =>
            {
                await goodreads_controller.ProcessSeriesAsync(collection, progress, token);
            }, token);

            current_task.ContinueWith(parent =>
            {
                IsBusy = false;
                progress.Report("Done");
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Stop()
        {
            if (cancellation_token_source != null)
                cancellation_token_source.Cancel();
        }
    }
}
