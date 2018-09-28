namespace Panda.Search
{
    public class SearchResult<T>
    {
        public T Item { get; private set; }
        public double Score { get; private set; }

        public SearchResult(T item, double score)
        {
            Item = item;
            Score = score;
        }
    }
}
