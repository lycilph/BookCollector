﻿using BookCollector.Application;
using BookCollector.Screens.Modules;
using Core.Application;
using Core.Infrastructure;
using NLog;
using Panda.Infrastructure;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookCollector.Screens.Shell
{
    public class ShellViewModel : ViewAwareScreenBase, IShellViewModel
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string application_name = "Book Collector";
        private readonly List<ModuleType> shell_modules = new List<ModuleType> { ModuleType.Collections, ModuleType.Settings };
        private IModule previous_module;

        private IStateManager state_manager;
        private IEnumerable<IModule> Modules { get; set; }
        private IModulesViewModel modules_view_model;

        private IModule _CurrentModule;
        public IModule CurrentModule
        {
            get { return _CurrentModule; }
            set
            {
                if (_CurrentModule != value)
                {
                    _CurrentModule?.Deactivate();
                    previous_module = CurrentModule;
                    this.RaiseAndSetIfChanged(ref _CurrentModule, value);
                    _CurrentModule?.Activate();
                }
            }
        }

        private string _CollectionName;
        public string CollectionName
        {
            get { return _CollectionName; }
            set { this.RaiseAndSetIfChanged(ref _CollectionName, value); }
        }

        private ReactiveCommand _ToggleLogCommand;
        public ReactiveCommand ToggleLogCommand
        {
            get { return _ToggleLogCommand; }
            set { this.RaiseAndSetIfChanged(ref _ToggleLogCommand, value); }
        }

        public ShellViewModel(IStateManager state_manager, IEnumerable<IModule> Modules, IModulesViewModel modules_view_model)
        {
            DisplayName = application_name;
            this.state_manager = state_manager;
            this.Modules = Modules;
            this.modules_view_model = modules_view_model;

            this.WhenAnyValue(x => x.state_manager.CurrentCollection, x => x.state_manager.CurrentCollection.Name)
                .Subscribe(x =>
                {
                    DisplayName = $"{application_name} - {state_manager.CurrentCollection?.Name}";
                    CollectionName = state_manager.CurrentCollection?.Name;
                });

            ToggleLogCommand = ReactiveCommand.Create(() =>
            {
                logger.Trace($"Toggling log module");
                if (CurrentModule.Type == ModuleType.Logs)
                    CurrentModule = previous_module;
                else
                    CurrentModule = Modules.FirstOrDefault(m => m.Type == ModuleType.Logs);
            });
        }

        public void NavigateTo(ModuleType module)
        {
            logger.Trace($"Navigating to [{module}] module");
            if (shell_modules.Contains(module))
            {
                CurrentModule = Modules.FirstOrDefault(m => m.Type == module);
            }
            else
            {
                CurrentModule = modules_view_model;
                modules_view_model.SetModule(module);
            }
        }

        protected override void OnViewLoaded(object view)
        {
            MessageBus.Current.SendMessage(ApplicationMessage.ShellLoaded);
        }

        protected override void OnViewUnloaded(object view)
        {
            CurrentModule?.Deactivate();
        }
    }
}