using System.Collections.Generic;

namespace ModShardLauncher
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(int, T)> Enumerate<T>(
            this IEnumerable<T> ienumerable
        ) {
            int ind = 0;
            foreach(T element in ienumerable) {
                yield return (ind++, element);
            }
        }
    }
}