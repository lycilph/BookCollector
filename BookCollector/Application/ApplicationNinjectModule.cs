using BookCollector.Application.Controllers;
using BookCollector.Screens.Books;
using BookCollector.Screens.Collections;
using BookCollector.Screens.Import;
using BookCollector.Screens.Shell;
using Ninject.Modules;
using NLog;

namespace BookCollector.Application
{
    public class ApplicationNinjectModule : NinjectModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override void Load()
        {
            logger.Trace("Initializing dependencies for application");

            BindAsSingleton<IApplicationController, ApplicationController>();
            BindAsSingleton<IStateManager, StateManager>();
            BindAsSingleton<IRepository, Repository>();

            BindAsSingleton<IShellViewModel, ShellViewModel>();
            BindAsSingleton<ICollectionsModule, IModule, CollectionsModuleViewModel>();
            BindAsSingleton<IBooksModule, IModule, BooksModuleViewModel>();
            BindAsSingleton<IImportModule, IModule, ImportModuleViewModel>();
        }

        private void BindAsSingleton<TInterface, TImplementation>() where TImplementation : TInterface
        {
            Bind<TInterface>().To<TImplementation>().InSingletonScope();
        }

        private void BindAsSingleton<TInterface1, TInterface2, TImplementation>() where TImplementation : TInterface1, TInterface2
        {
            Bind<TInterface1, TInterface2>().To<TImplementation>().InSingletonScope();
        }
    }
}
