using Core.Infrastructure;
using GoodreadsPlugin.Screens.Import;
using Ninject.Modules;
using NLog;

namespace GoodreadsPlugin
{
    public class GoodreadsNinjectModule : NinjectModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override void Load()
        {
            logger.Trace("Initializing Goodreads plugin dependencies");

            // Modules in this plugin
            Bind<IModule>().To<ImportViewModel>().InSingletonScope();
        }
    }
}
