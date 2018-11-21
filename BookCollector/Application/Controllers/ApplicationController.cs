using BookCollector.Application.Messages;
using BookCollector.Data;
using BookCollector.Screens.Books;
using BookCollector.Screens.Collections;
using BookCollector.Screens.Import;
using BookCollector.Screens.Settings;
using BookCollector.Screens.Shell;
using NLog;
using Panda.Search;
using Panda.Utils;
using ReactiveUI;
using System;

namespace BookCollector.Application.Controllers
{
    public class ApplicationController : IApplicationController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string stopwords_filename = "stopwords_en.txt";

        private IStateManager state_manager;
        private IShellViewModel shell;
        private ISearchEngine<Book> search_engine;

        public ApplicationController(IStateManager state_manager, IShellViewModel shell, ISearchEngine<Book> search_engine)
        {
            this.state_manager = state_manager;
            this.shell = shell;
            this.search_engine = search_engine;
        }

        public void Initialize()
        {
            logger.Trace("Initializing application controller");
            HookUpMessages();
            InitializeSearchEngine();
            state_manager.Initialize();
        }

        public void Exit()
        {
            logger.Trace("Exiting application controller");
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
                    shell.NavigateTo(typeof(ICollectionsModule), show_windows_commands: false);
                    break;
                case NavigationMessage.Import:
                    shell.NavigateTo(typeof(IImportModule));
                    break;
                case NavigationMessage.Books:
                    shell.NavigateTo(typeof(IBooksModule));
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

        public void IndexCollection()
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
