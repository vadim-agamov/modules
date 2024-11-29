using System.Collections.Generic;
using System.Linq;

namespace Modules.Extensions
{
    public static class RandomExtensions
    {
        public static T Random<T>(this T[] list)
        {
            var index = UnityEngine.Random.Range(0, list.Length);
            return list[index];
        }
        
        public static T Random<T>(this List<T> list)
        {
            var index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }
        
        public static T Random<T>(this T[] list, float[] weights)
        {
            var sum = weights.Sum();
            var randomValue = UnityEngine.Random.Range(0f, sum);
            var currentSum = 0f;
            for (var i = 0; i < weights.Length; i++)
            {
                currentSum += weights[i];
                if (randomValue <= currentSum)
                {
                    return list[i];
                }
            }

            return list[^1];
        }
        
        public static IList<T> Shuffle<T>(this IList<T> self)
        {
            var count = self.Count;

            for (var i = 0; i < count; ++i)
            {
                var j = UnityEngine.Random.Range(0, count);
                (self[i], self[j]) = (self[j], self[i]);
            }

            return self;
        }
    }
}