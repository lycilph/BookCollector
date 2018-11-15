using Newtonsoft.Json;
using Panda.Utils;
using ReactiveUI;
using System.Diagnostics;

namespace BookCollector.Data
{
    [DebuggerDisplay("Name = {Name}")]
    [JsonObject(MemberSerialization.OptOut)]
    public class Collection : DirtyTrackingBase
    {
        private string _Filename;
        [JsonIgnore]    
        public string Filename
        {
            get { return _Filename; }
            set { this.RaiseAndSetIfChanged(ref _Filename, value); }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }
    }
}
