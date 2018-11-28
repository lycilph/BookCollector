using Newtonsoft.Json;
using ReactiveUI;
using System.Diagnostics;

namespace BookCollector.Data
{
    [DebuggerDisplay("Name = {Name}")]
    [JsonObject(MemberSerialization.OptOut)]
    public class Note : ReactiveObject
    {
        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }
    }
}
