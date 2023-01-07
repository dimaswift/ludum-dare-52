using System.Collections.Generic;

namespace ECF.Views
{
    public static class Extensions
    {
        public static T Random<T>(this IList<T> source)
        {
            return source[UnityEngine.Random.Range(0, source.Count)];
        }

        public static T Random<T>(this T[] source)
        {
            return source[UnityEngine.Random.Range(0, source.Length)];
        }
    }
}