using System;
using System.Collections.Generic;

namespace Panda.Search
{
    public interface ISearchEngine<T>
    {
        void Index(List<T> items, Func<T, string> mapping);
        List<SearchResult<T>> Search(string query);
    }
}
