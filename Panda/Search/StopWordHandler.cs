using System;
using System.Collections.Generic;
using System.IO;

namespace Panda.Search
{
    public class StopWordHandler
    {
        private HashSet<string> word_set;

        public StopWordHandler() { }
        public StopWordHandler(string stopwords_filename)
        {
            var text = File.ReadAllText(stopwords_filename);
            Initialize(text);
        }

        public void Initialize(string text)
        {
            var words = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            word_set = new HashSet<string>(words);
        }

        public bool IsStopWord(string s)
        {
            return word_set.Contains(s);
        }
    }
}
