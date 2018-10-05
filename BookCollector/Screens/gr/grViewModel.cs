using Core.Infrastructure;
using NLog;
using Panda.Infrastructure;
using ReactiveUI;
using RestSharp;

namespace BookCollector.Screens.gr
{
    public class grViewModel : ScreenBase, IModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ModuleType Type { get; } = ModuleType.Plugin;

        private ReactiveCommand _TestCommand;
        public ReactiveCommand TestCommand
        {
            get { return _TestCommand; }
            set { this.RaiseAndSetIfChanged(ref _TestCommand, value); }
        }

        public grViewModel()
        {
            DisplayName = "GR";

            TestCommand = ReactiveCommand.Create(Test);
        }

        private void Test()
        {
            var client = new RestClient("https://www.goodreads.com");
            var request = new RestRequest("book/isbn/0765397528").AddQueryParameter("key", "XJA3uzcNTPrWIJdfCHqVxQ");
            var uri = client.BuildUri(request);

            var response = client.Execute(request);
            var content = response.Content;
        }
    }
}
