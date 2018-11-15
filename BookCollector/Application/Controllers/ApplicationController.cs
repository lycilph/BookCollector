using BookCollector.Application.Messages;
using BookCollector.Screens.Books;
using BookCollector.Screens.Collections;
using BookCollector.Screens.Import;
using BookCollector.Screens.Shell;
using NLog;
using ReactiveUI;
using System;

namespace BookCollector.Application.Controllers
{
    public class ApplicationController : IApplicationController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IStateManager state_manager;
        private IShellViewModel shell;

        public ApplicationController(IStateManager state_manager, IShellViewModel shell)
        {
            this.state_manager = state_manager;
            this.shell = shell;
        }

        public void Initialize()
        {
            logger.Trace("Initializing application controller");
            HookUpMessages();
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
        }

        private void HandleApplicationMessage(ApplicationMessage message)
        {
            logger.Trace($"Got an application message [{message}]");

            switch (message)
            {
                case ApplicationMessage.ShellLoaded:
                    shell.NavigateTo(typeof(ICollectionsModule));
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
                    shell.NavigateTo(typeof(ICollectionsModule));
                    break;
                case NavigationMessage.Import:
                    shell.NavigateTo(typeof(IImportModule));
                    break;
                case NavigationMessage.Books:
                    shell.NavigateTo(typeof(IBooksModule));
                    break;
                default:
                    throw new ArgumentException($"Unhandled navigation message {message}");
            }
        }
    }
}
