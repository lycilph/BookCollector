using Core.Application;
using Core.Data;
using Core.Infrastructure;
using GoodreadsPlugin.Api;
using GoodreadsPlugin.Data;
using NLog;
using Panda.Infrastructure;
using ReactiveUI;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoodreadsPlugin.Screens.Info
{
    public class InfoViewModel : ScreenBase, IModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IStateManager state_manager;
        private GoodreadsClient goodreads_client;
        private CancellationTokenSource cancellation_token_source;
        private Task current_task;
        private IProgress<string> progress;
        private IProgress<(Book, GoodreadsBook)> update_book;
        private IProgress<Series> add_series;

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

        private ReactiveCommand _UpdateIncompleteSeriesCommand;
        public ReactiveCommand UpdateIncompleteSeriesCommand
        {
            get { return _UpdateIncompleteSeriesCommand; }
            set { this.RaiseAndSetIfChanged(ref _UpdateIncompleteSeriesCommand, value); }
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
            UpdateIncompleteSeriesCommand = ReactiveCommand.Create(UpdateIncompleteSeries);
            StopCommand = ReactiveCommand.Create(Stop);
            ClearMessagesCommand = ReactiveCommand.Create(() => Messages.Clear());

            progress = new Progress<string>(Messages.Add);
            update_book = new Progress<(Book, GoodreadsBook)>(input => UpdateBook(input.Item1, input.Item2));
            add_series = new Progress<Series>(AddSeries);
        }

        public override void OnActivated()
        {
            goodreads_client = new GoodreadsClient();
        }

        public override void OnDeactivated()
        {
            if (current_task != null)
            {
                Stop();
                current_task.Wait();
                current_task = null;
            }

            goodreads_client.Dispose();
            goodreads_client = null;
        }

        private void ProcessBooks()
        {
            IsBusy = true;
            Messages.Clear();

            var books_to_process = state_manager.CurrentCollection.Books.Where(b => !b.Tags.ContainsKey("GoodreadsWorkId")).ToList();
            progress.Report($"Processing {books_to_process.Count} books out of {state_manager.CurrentCollection.Books.Count}");

            cancellation_token_source = new CancellationTokenSource();
            current_task = Task.Run(() =>
            {
                var total = books_to_process.Count;
                var current = 1;
                foreach (var book in books_to_process)
                {
                    progress.Report($"Processing {book.Title} - [{current} of {total}]");

                    var goodreads_book = goodreads_client.GetBookById(book.Tags["GoodreadsBookId"]);
                    update_book.Report((book, goodreads_book));

                    if (cancellation_token_source.Token.IsCancellationRequested)
                        break;

                    current++;
                }
            }, cancellation_token_source.Token);

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

            var all_series_ids = state_manager.CurrentCollection.Books.Where(b => b.Tags.ContainsKey("GoodreadsSeriesId"))
                                                                      .Select(b => b.Tags["GoodreadsSeriesId"])
                                                                      .Distinct()
                                                                      .ToList();
            var processed_series_ids = state_manager.CurrentCollection.Series.Select(s => s.Tags["GoodreadsSeriesId"])
                                                                             .ToList();
            var ids_to_process = all_series_ids.Except(processed_series_ids).ToList();
            progress.Report($"Processing {ids_to_process.Count} series out of {all_series_ids.Count}");

            cancellation_token_source = new CancellationTokenSource();
            current_task = Task.Run(() =>
            {
                var total = ids_to_process.Count;
                var current = 1;
                foreach (var id in ids_to_process)
                {
                    progress.Report($"Processing {id} - [{current} of {total}]");

                    var goodreads_series = goodreads_client.GetSeriesById(id);
                    var series = GoodreadsMapper.Map(goodreads_series);
                    add_series.Report(series);

                    progress.Report($" - Found [{series.Title}] containing {series.Entries.Count} entries");

                    if (cancellation_token_source.Token.IsCancellationRequested)
                        break;

                    ProcessSeriesEntries(series, cancellation_token_source.Token);

                    current++;
                }
            }, cancellation_token_source.Token);

            current_task.ContinueWith(parent =>
            {
                IsBusy = false;
                progress.Report("Done");
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UpdateIncompleteSeries()
        {
            IsBusy = true;
            Messages.Clear();

            var series_to_process = state_manager.CurrentCollection.Series.Where(s => s.Entries.Any(e => e.MissingInCollection))
                                                                          .ToList();

            cancellation_token_source = new CancellationTokenSource();
            current_task = Task.Run(() =>
            {
                var total = series_to_process.Count;
                var current = 1;
                foreach (var series in series_to_process)
                {
                    progress.Report($"Processing {series.Title} - [{current} of {total}]");

                    ProcessSeriesEntries(series, cancellation_token_source.Token);

                    if (cancellation_token_source.Token.IsCancellationRequested)
                        break;

                    current++;
                }
            }, cancellation_token_source.Token);

            current_task.ContinueWith(parent =>
            {
                IsBusy = false;
                progress.Report("Done");
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ProcessSeriesEntries(Series series, CancellationToken cancellation_token)
        {
            foreach (var entry in series.Entries.Where(e => e.MissingInCollection))
            {
                // Try to find the book in the collection
                var book = state_manager.CurrentCollection.Books.FirstOrDefault(b => b.Tags["GoodreadsWorkId"] == entry.Tags["GoodreadsWorkId"]);
                if (book != null)
                {
                    // We found the book in the collection
                    entry.Book = book;
                    entry.MissingInCollection = false;
                }
                else
                {
                    // We did not find it in the collection, so look it up (if this has not already been done)
                    if (entry.Book == null)
                    {
                        progress.Report($" - Processing {entry.Tags["GoodreadsBestBookId"]}");

                        var goodreads_book = goodreads_client.GetBookById(entry.Tags["GoodreadsBestBookId"]);
                        book = GoodreadsMapper.Map(goodreads_book);
                        entry.Book = book;

                        progress.Report($" - Found [{book.Title}]");
                    }
                }

                if (cancellation_token.IsCancellationRequested)
                    break;
            }
        }

        private void Stop()
        {
            if (cancellation_token_source != null)
                cancellation_token_source.Cancel();
        }

        private void UpdateBook(Book book, GoodreadsBook goodreads_book)
        {
            book.Tags.Add("GoodreadsWorkId", goodreads_book.Work.Id);

            if (!string.IsNullOrWhiteSpace(goodreads_book.Description))
                book.Description = goodreads_book.Description;

            if (goodreads_book.SeriesWorks.Any())
            {
                book.Tags.Add("GoodreadsSeriesWorkId", goodreads_book.SeriesWorks.First().Id);
                book.Tags.Add("GoodreadsSeriesId", goodreads_book.SeriesWorks.First().Series.Id);
            }
        }

        private void AddSeries(Series series)
        {
            state_manager.CurrentCollection.Add(series);
        }
    }
}
