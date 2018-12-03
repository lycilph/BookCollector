using BookCollector.Application.Messages;
using BookCollector.Application.Processor;
using BookCollector.Data;
using BookCollector.Screens.Books;
using BookCollector.Screens.Collections;
using BookCollector.Screens.Import;
using BookCollector.Screens.Logs;
using BookCollector.Screens.Notes;
using BookCollector.Screens.Notes.AvalonEdit;
using BookCollector.Screens.Series;
using BookCollector.Screens.Settings;
using BookCollector.Screens.Shell;
using NHunspell;
using NLog;
using Panda.Search;
using Panda.Utils;
using ReactiveUI;
using System;
using System.Text;

namespace BookCollector.Application.Controllers
{
    public class ApplicationController : IApplicationController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string stopwords_filename = "stopwords_en.txt";
        private const string aff_filename = "en_US.aff";
        private const string dic_filename = "en_US.dic";

        private IStateManager state_manager;
        private IShellViewModel shell;
        private ISearchEngine<Book> search_engine;
        private IBackgroundProcessor background_processor;
        private IGoodreadsController goodreads_controller;

        public ApplicationController(IStateManager state_manager, 
                                     IShellViewModel shell, 
                                     ISearchEngine<Book> search_engine, 
                                     IBackgroundProcessor background_processor, 
                                     IGoodreadsController goodreads_controller)
        {
            this.state_manager = state_manager;
            this.shell = shell;
            this.search_engine = search_engine;
            this.background_processor = background_processor;
            this.goodreads_controller = goodreads_controller;
        }

        public void Initialize()
        {
            logger.Trace("Initializing application controller");
            HookUpMessages();
            InitializeSearchEngine();
            InitializeSpellChecker();
            state_manager.Initialize();
            goodreads_controller.Initialize();
            background_processor.Start();
        }

        public void Exit()
        {
            logger.Trace("Exiting application controller");
            background_processor.Stop();
            goodreads_controller.Exit();
            state_manager.Exit();
        }

        private void HookUpMessages()
        {
            logger.Trace("Hooking up application messages");
            MessageBus.Current
                      .Listen<ApplicationMessage>()
                      .Subscribe(HandleApplicationMessage);

            logger.Trace("Hooking up navigation messages");
            MessageBus.Current
                      .Listen<NavigationMessage>()
                      .Subscribe(HandleNavigationMessage);

            logger.Trace("Hooking up information messages");
            MessageBus.Current
                      .Listen<InformationMessage>()
                      .Subscribe(HandleInformationMessages);
        }

        private void InitializeSearchEngine()
        {
            logger.Trace("Initializing search engine");
            var stopwords = ResourceExtensions.GetResource(stopwords_filename);
            search_engine.Initialize(stopwords);
        }

        private void InitializeSpellChecker()
        {
            logger.Trace("Initializing spell checker");
            var en_aff = ResourceExtensions.GetResource(aff_filename);
            var en_aff_data = Encoding.ASCII.GetBytes(en_aff);
            var en_dic = ResourceExtensions.GetResource(dic_filename);
            var en_dic_data = Encoding.ASCII.GetBytes(en_dic);
            SpellChecker.Default.HunspellInstance = new Hunspell(en_aff_data, en_dic_data);
        }

        private void HandleApplicationMessage(ApplicationMessage message)
        {
            logger.Trace($"Got an application message [{message}]");

            switch (message)
            {
                case ApplicationMessage.ShellLoaded:
                    if (state_manager.CurrentCollection == null)
                        shell.NavigateTo(typeof(ICollectionsModule), show_windows_commands: false);
                    else
                        shell.NavigateTo(typeof(IBooksModule));
                    break;
                case ApplicationMessage.SnackbarMessageDurationUpdated:
                    shell.UpdateSnackbarQueue();
                    break;
                case ApplicationMessage.CollectionChanged:
                    IndexCollection();
                    break;
                default:
                    throw new ArgumentException($"Unhandled application message {message}");
            }
        }

        private void HandleNavigationMessage(NavigationMessage message)
        {
            logger.Trace($"Got a navigation message [{message}]");

            switch (message)
            {
                case NavigationMessage.Collections:
                    NavigateToCollectionsModule();
                    break;
                case NavigationMessage.Import:
                    shell.NavigateTo(typeof(IImportModule));
                    break;
                case NavigationMessage.Logs:
                    shell.NavigateTo(typeof(ILogsModule));
                    break;
                case NavigationMessage.Books:
                    shell.NavigateTo(typeof(IBooksModule));
                    break;
                case NavigationMessage.Series:
                    shell.NavigateTo(typeof(ISeriesModule));
                    break;
                case NavigationMessage.Notes:
                    shell.NavigateTo(typeof(INotesModule));
                    break;
                case NavigationMessage.Settings:
                    shell.NavigateTo(typeof(ISettingsModule));
                    break;
                default:
                    throw new ArgumentException($"Unhandled navigation message {message}");
            }
        }

        private void HandleInformationMessages(InformationMessage message)
        {
            logger.Trace($"Got an information message [{message.Content}]");

            if (!string.IsNullOrWhiteSpace(message.ActionContent) && message.ActionHandler != null)
                shell.ShowMessage(message.Content, message.ActionContent, message.ActionHandler);
            else
                shell.ShowMessage(message.Content);
        }

        private void NavigateToCollectionsModule()
        {
            // Make sure this is cleared in case current collection is replaced
            background_processor.Clear();
            // Save the CurrentCollection in case it is replaced
            state_manager.SaveCurrentCollection();
            // Do the actual navigation to the collections module
            shell.NavigateTo(typeof(ICollectionsModule), show_windows_commands: false);
        }

        private void IndexCollection()
        {
            if (state_manager.CurrentCollection == null)
            {
                logger.Trace("Nothing to index, skipping");
                return;
            }

            var books = state_manager.CurrentCollection.Books;
            search_engine.Index(books, b => string.Join(", ", b.Title, b.Description, string.Join(", ", b.Authors)));
        }
    }
}
