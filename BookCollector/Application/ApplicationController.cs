using BookCollector.Screens.Shell;
using Core.Application;
using Core.Infrastructure;
using NLog;
using ReactiveUI;
using System;

namespace BookCollector.Application
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

            logger.Trace("Hooking up application messages");
            MessageBus.Current
                      .Listen<ApplicationMessage>()
                      .Subscribe(HandleApplicationMessage);

            state_manager.Initialize();
        }

        public void Exit()
        {
            logger.Trace("Exiting application controller");

            state_manager.Exit();
        }

        private void HandleApplicationMessage(ApplicationMessage message)
        {
            logger.Trace($"Got an application message {message}");

            switch (message)
            {
                case ApplicationMessage.ShellLoaded:
                    {
                        if (state_manager.CurrentCollection == null)
                            shell.NavigateTo(ModuleType.Collections);
                        else
                            shell.NavigateTo(ModuleType.Books);
                    }
                    break;
                case ApplicationMessage.NavigateToBooks:
                    shell.NavigateTo(ModuleType.Books);
                    break;
                case ApplicationMessage.NavigateToCollections:
                    shell.NavigateTo(ModuleType.Collections);
                    break;
                case ApplicationMessage.NavigateToSettings:
                    shell.NavigateTo(ModuleType.Settings);
                    break;
                case ApplicationMessage.NavigateToModules:
                    shell.NavigateTo(ModuleType.Modules);
                    break;
                default:
                    throw new ArgumentException($"Unhandled application message {message}");
            }
        }
    }
}
