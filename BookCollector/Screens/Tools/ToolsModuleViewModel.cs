using BookCollector.Application.Processor;
using BookCollector.Goodreads.Items;
using BookCollector.Screens.Common;
using Panda.Infrastructure;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace BookCollector.Screens.Tools
{
    public class ToolsModuleViewModel : ScreenBase, IToolsModule
    {
        private IBackgroundProcessor background_processor;

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

        private int _BookInformationProcessCount;
        public int BookInformationProcessCount
        {
            get { return _BookInformationProcessCount; }
            set { this.RaiseAndSetIfChanged(ref _BookInformationProcessCount, value); }
        }

        private int _SeriesInformationProcessCount;
        public int SeriesInformationProcessCount
        {
            get { return _SeriesInformationProcessCount; }
            set { this.RaiseAndSetIfChanged(ref _SeriesInformationProcessCount, value); }
        }

        public ToolsModuleViewModel(ApplicationNavigationPartViewModel application_navigation_part,
                                    ToolsNavigationPartViewModel tools_navigation_part,
                                    IBackgroundProcessor background_processor)
        {
            ApplicationNavigationPart = application_navigation_part;
            ToolsNavigationPart = tools_navigation_part;
            this.background_processor = background_processor;

            this.WhenAnyValue(x => x.background_processor.Status)
                .Where(s => s != null)
                .Select(s => s.FirstOrDefault(p => p.Type == typeof(BookInformationItem)))
                .Subscribe(p => BookInformationProcessCount = (p == null ? 0 : p.Count));


            this.WhenAnyValue(x => x.background_processor.Status)
                .Where(s => s != null)
                .Select(s => s.FirstOrDefault(p => p.Type == typeof(SeriesInformationItem)))
                .Subscribe(p => SeriesInformationProcessCount = (p == null ? 0 : p.Count));
        }
    }
}
