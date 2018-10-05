using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandbox.Model
{
    public class GoodreadsSeries
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string SeriesWorksCount { get; set; }
        public string PrimaryWorkCount { get; set; }
        public List<GoodreadsWork> SeriesWorks { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Series:");
            sb.AppendLine($"Id: {Id}");
            sb.AppendLine($"Title: {Title}");
            sb.AppendLine($"Series work count: {SeriesWorksCount}");
            sb.AppendLine($"Primary work count: {PrimaryWorkCount}");
            if (SeriesWorks != null && SeriesWorks.Any())
            {
                sb.AppendLine("Series works:");
                foreach (var work in SeriesWorks)
                    sb.Append(work.ToString());
            }

            return sb.ToString();
        }
    }
}