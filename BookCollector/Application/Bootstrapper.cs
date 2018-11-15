using System;
using System.Windows;
using BookCollector.Application.Controllers;
using BookCollector.Screens.Shell;
using Ninject;
using NLog;
using Panda.Infrastructure;

namespace BookCollector.Application
{
    public class Bootstrapper : BootstrapperBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public IKernel Kernel { get; private set; }

        protected override void Configure()
        {
            logger.Trace("Configuring BookCollector");

            Kernel = new StandardKernel();
            Kernel.Load(AssemblySource.Assemblies);
        }

        protected override object GetInstance(Type type)
        {
            logger.Trace($"Getting instance for {type.Name} from Ninject Kernel");

            return Kernel.Get(type);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            logger.Trace("Starting up BookCollector");

            var controller = Kernel.Get<IApplicationController>();
            controller.Initialize();

            DisplayRootViewFor<IShellViewModel>();
        }

        protected override void OnExit(object sender, ExitEventArgs e)
        {
            logger.Trace("Exiting BookCollector");

            var controller = Kernel.Get<IApplicationController>();
            controller.Exit();
        }
    }
}
