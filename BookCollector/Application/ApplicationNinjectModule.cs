using BookCollector.Screens.Books;
using BookCollector.Screens.Collections;
using BookCollector.Screens.Logs;
using BookCollector.Screens.Modules;
using BookCollector.Screens.Shell;
using Core.Application;
using Core.Infrastructure;
using Ninject.Modules;
using NLog;
using Panda.Dialog;

namespace BookCollector.Application
{
    public class ApplicationNinjectModule : NinjectModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override void Load()
        {
            logger.Trace("Initializing dependencies for application");

            Bind<IApplicationController>().To<ApplicationController>().InSingletonScope();
            Bind<IStateManager>().To<StateManager>().InSingletonScope();
            Bind<IRepository>().To<Repository>().InSingletonScope();
            Bind<IDialogManager>().To<DialogManager>().InSingletonScope();

            Bind<IShellViewModel>().To<ShellViewModel>().InSingletonScope();
            Bind<IModule, IModulesViewModel>().To<ModulesViewModel>().InSingletonScope();
            Bind<IModule>().To<BooksViewModel>().InSingletonScope();
            Bind<IModule>().To<CollectionsViewModel>().InSingletonScope();
            Bind<IModule>().To<LogsViewModel>().InSingletonScope();
        }
    }
}
