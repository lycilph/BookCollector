using BookCollector.Application;
using BookCollector.Application.Controllers;
using BookCollector.Application.Messages;
using BookCollector.Screens.Common;
using MaterialDesignColors;
using Panda.Infrastructure;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace BookCollector.Screens.Settings
{
    public class SettingsModuleViewModel : ScreenBase, ISettingsModule
    {
        private IStateManager state_manager;
        private IThemeController theme_controller;
        private int current_snackbar_message_duration;

        public bool LoadOnStart
        {
            get { return state_manager.Settings.LoadMostRecentCollectionOnStart; }
            set { state_manager.Settings.LoadMostRecentCollectionOnStart = value; }
        }

        public int SnackbarMessageDuration
        {
            get { return state_manager.Settings.SnackbarMessageDuration; }
            set
            {
                state_manager.Settings.SnackbarMessageDuration = value;
                this.RaisePropertyChanged();
            }
        }

        private List<Swatch> _PrimaryColors;
        public List<Swatch> PrimaryColors
        {
            get { return _PrimaryColors; }
            set { this.RaiseAndSetIfChanged(ref _PrimaryColors, value); }
        }

        private Swatch _SelectedPrimaryColor;
        public Swatch SelectedPrimaryColor
        {
            get { return _SelectedPrimaryColor; }
            set { this.RaiseAndSetIfChanged(ref _SelectedPrimaryColor, value); }
        }

        private List<Swatch> _AccentColors;
        public List<Swatch> AccentColors
        {
            get { return _AccentColors; }
            set { this.RaiseAndSetIfChanged(ref _AccentColors, value); }
        }

        private Swatch _SelectedAccentColor;
        public Swatch SelectedAccentColor
        {
            get { return _SelectedAccentColor; }
            set { this.RaiseAndSetIfChanged(ref _SelectedAccentColor, value); }
        }

        private bool _IsThemeDark;
        public bool IsThemeDark
        {
            get { return _IsThemeDark; }
            set { this.RaiseAndSetIfChanged(ref _IsThemeDark, value); }
        }

        public bool EnableShelfMatching
        {
            get { return state_manager.Settings.EnableShelfMatching; }
            set
            {
                state_manager.Settings.EnableShelfMatching = value;
                this.RaisePropertyChanged();
            }
        }

        public bool CreateUnmatchedShelves
        {
            get { return state_manager.Settings.CreateUnmatchedShelves; }
            set { state_manager.Settings.CreateUnmatchedShelves = value; }
        }

        public int MaxEditDistance
        {
            get { return state_manager.Settings.MaxEditDistanceForShelfMatching; }
            set
            {
                state_manager.Settings.MaxEditDistanceForShelfMatching = value;
                this.RaisePropertyChanged();
            }
        }

        private ApplicationNavigationPartViewModel _ApplicationNavigationPart;
        public ApplicationNavigationPartViewModel ApplicationNavigationPart
        {
            get { return _ApplicationNavigationPart; }
            set { this.RaiseAndSetIfChanged(ref _ApplicationNavigationPart, value); }
        }

        private ToolsNavigationPartViewModel _ToolsNavigationPart;
        public ToolsNavigationPartViewModel ToolsNavigationPart
        {
            get { return _ToolsNavigationPart; }
            set { this.RaiseAndSetIfChanged(ref _ToolsNavigationPart, value); }
        }

        public SettingsModuleViewModel(IStateManager state_manager,
                                       IThemeController theme_controller,
                                       ApplicationNavigationPartViewModel application_navigation_part,
                                       ToolsNavigationPartViewModel tools_navigation_part)
        {
            this.state_manager = state_manager;
            this.theme_controller = theme_controller;
            ApplicationNavigationPart = application_navigation_part;
            ToolsNavigationPart = tools_navigation_part;

            this.WhenAnyValue(x => x.SelectedPrimaryColor)
                .Where(swatch => swatch != null)
                .Select(swatch => swatch.Name)
                .Subscribe(swatch_name =>
                {
                    state_manager.Settings.PrimaryColorName = swatch_name;
                    theme_controller.SetPrimary(swatch_name);
                });

            this.WhenAnyValue(x => x.SelectedAccentColor)
                .Where(swatch => swatch != null)
                .Select(swatch => swatch.Name)
                .Subscribe(swatch_name =>
                {
                    state_manager.Settings.AccentColorName = swatch_name;
                    theme_controller.SetAccent(swatch_name);
                });

            this.WhenAnyValue(x => x.IsThemeDark)
                .Subscribe(x => 
                {
                    state_manager.Settings.IsDarkTheme = IsThemeDark;
                    theme_controller.SetBase(IsThemeDark);
                });
        }

        public override void OnActivated()
        {
            current_snackbar_message_duration = state_manager.Settings.SnackbarMessageDuration;

            PrimaryColors = theme_controller.GetPrimaryColors();
            AccentColors = theme_controller.GetAccentColors();

            SelectedPrimaryColor = PrimaryColors.SingleOrDefault(s => s.Name == state_manager.Settings.PrimaryColorName);
            SelectedAccentColor = AccentColors.SingleOrDefault(s => s.Name == state_manager.Settings.AccentColorName);
            IsThemeDark = state_manager.Settings.IsDarkTheme;
        }

        public override void OnDeactivated()
        {
            // Send message if duration has changed
            if (current_snackbar_message_duration != state_manager.Settings.SnackbarMessageDuration)
                MessageBus.Current.SendMessage(ApplicationMessage.SnackbarMessageDurationUpdated);
        }
    }
}
