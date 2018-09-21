using BookCollector.Screens.Shell;
using Core.Application;
using Ninject;
using NLog;
using Panda.Infrastructure;
using Panda.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace BookCollector.Application
{
    public class Bootstrapper : BootstrapperBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public IKernel Kernel { get; private set; }

        public override void InitializeLogging()
        {
            LogManager.Configuration.AddTarget("memory", new MemoryTarget());
            LogManager.Configuration.AddRuleForAllLevels("memory");
            LogManager.Configuration.Reload();
        }

        protected override void Configure()
        {
            logger.Trace("Configuring BookCollector");

            Kernel = new StandardKernel();
            Kernel.Load(AssemblySource.Assemblies);
        }

        protected override List<Assembly> SelectAssemblies()
        {
            var assemblies = base.SelectAssemblies();

            // Adding plugin assemblies to the list
            logger.Trace("Adding plugin assemblies to AssemblySource");

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.EnumerateFiles(path, "*plugin.dll")
                     .Apply(plugin => assemblies.Add(Assembly.LoadFile(plugin)));

            return assemblies;
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
