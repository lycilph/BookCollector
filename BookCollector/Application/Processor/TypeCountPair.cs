using System;

namespace BookCollector.Application.Processor
{
    public class TypeCountPair
    {
        public Type Type { get; set; }
        public int Count { get; set; }

        public TypeCountPair(Type type, int count)
        {
            Type = type;
            Count = count;
        }
    }
}
