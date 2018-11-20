using System.Collections.Generic;

namespace Panda.Search
{
    public class Document<T>
    {
        public T Item { get; set; }
        public IDictionary<string, double> NormalizedTermFrequency { get; set; }
        public ICollection<string> Terms { get { return NormalizedTermFrequency.Keys; } }
        public ICollection<double> TF { get { return NormalizedTermFrequency.Values; } }

        public Document(IDictionary<string, double> term_frequencies) : this(term_frequencies, default) {}
        public Document(IDictionary<string, double> term_frequencies, T item)
        {
            NormalizedTermFrequency = term_frequencies;
            Item = item;
        }

        public Document<T> Add(T item)
        {
            Item = item;
            return this;
        }
    }
}
