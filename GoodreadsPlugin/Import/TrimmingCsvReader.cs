using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace GoodreadsPlugin.Import
{
    public class TrimmingCsvReader : CsvReader
    {
        public TrimmingCsvReader(TextReader reader, Configuration configuration) : base(reader, configuration) { }

        public override string GetField(int index)
        {
            var field = base.GetField(index);
            return field.TrimStart('=').Replace("\"", "");
        }
    }
}
