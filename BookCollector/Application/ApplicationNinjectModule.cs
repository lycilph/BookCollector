using BookCollector.Application.Controllers;
using BookCollector.Application.Processor;
using BookCollector.Data;
using BookCollector.Screens.Books;
using BookCollector.Screens.Collections;
using BookCollector.Screens.Common;
using BookCollector.Screens.Import;
using BookCollector.Screens.Logs;
using BookCollector.Screens.Notes;
using BookCollector.Screens.Series;
using BookCollector.Screens.Settings;
using BookCollector.Screens.Shell;
using BookCollector.Screens.Tools;
using Ninject.Modules;
using NLog;
using Panda.Search;

namespace BookCollector.Application
{
    public class ApplicationNinjectModule : NinjectModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override void Load()
        {
            logger.Trace("Initializing dependencies for application");

            BindAsSingleton<IApplicationController, ApplicationController>();
            BindAsSingleton<IImportController, ImportController>();
            BindAsSingleton<IGoodreadsController, GoodreadsController>();
            BindAsSingleton<IThemeController, ThemeController>();
            BindAsSingleton<IStateManager, StateManager>();
            BindAsSingleton<IRepository, Repository>();
            BindAsSingleton<ISearchEngine<Book>, SearchEngine<Book>>();
            BindAsSingleton<IBackgroundProcessor, BackgroundProcessor>();

            BindAsSingleton<IShellViewModel, ShellViewModel>();
            BindAsSingleton<ICollectionsModule, IModule, CollectionsModuleViewModel>();
            BindAsSingleton<ISettingsModule, IModule, SettingsModuleViewModel>();
            BindAsSingleton<IBooksModule, IModule, BooksModuleViewModel>();
            BindAsSingleton<INotesModule, IModule, NotesModuleViewModel>();
            BindAsSingleton<ISeriesModule, IModule, SeriesModuleViewModel>();
            BindAsSingleton<IImportModule, IModule, ImportModuleViewModel>();
            BindAsSingleton<ILogsModule, IModule, LogsModuleViewModel>();
            BindAsSingleton<IToolsModule, IModule, ToolsModuleViewModel>();

            BindSelfAsSingleton<ApplicationNavigationPartViewModel>();
            BindSelfAsSingleton<CollectionsNavigationPartViewModel>();
            BindSelfAsSingleton<ToolsNavigationPartViewModel>();
            BindSelfAsSingleton<CollectionInformationPartViewModel>();
        }

        private void BindSelfAsSingleton<TImplementation>()
        {
            Bind<TImplementation>().ToSelf().InSingletonScope();
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
