using System;
using System.Collections.Generic;

namespace Panda.Search
{
    public interface ISearchEngine<T>
    {
        void Initialize(string stopwords);
        void InitializeFromFile(string stopwords_filename);
        void Index(IEnumerable<T> items, Func<T, string> mapping);
        List<SearchResult<T>> Search(string query);
    }
}
