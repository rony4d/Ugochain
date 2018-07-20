using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UgoChain
{
    public static class Extensions
    {
        public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
        {
            foreach (var item in enumerable)
            {
                await action(item);
            }
        }
    }
}
