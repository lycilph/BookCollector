using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandbox.Model
{
    public class GoodreadsBook
    {
        public string Id { get; set; }
        public string Title { get; set; }
        //public string Title { get; set; }
        public string image_url { get; set; }
        public string Description { get; set; }
        public GoodreadsWork Work { get; set; }
        public List<GoodreadsWork> SeriesWorks { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Book:");
            sb.AppendLine($"Id: {Id}");
            sb.AppendLine($"Title: {Title}");
            sb.AppendLine($"Image: {image_url}");
            sb.AppendLine($"Description: {Description}");
            if (Work != null)
                sb.Append(Work.ToString());
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
