using System;
using System.Collections;
using System.Collections.Generic;

namespace Tools.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            foreach (var element in source)
                target.Add(element);
        }

        public static void RemoveAll<T>(this ICollection<T> collection, T item)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (item == null) throw new ArgumentNullException(nameof(item));
            while (collection.Contains(item)) collection.Remove(item);
        }

        public static bool IsEmpty(this ICollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            return collection.Count == 0;
        }

        public static bool IsNullOrEmpty(this ICollection collection)
        {
            return collection == null || collection.Count == 0;
        }
    }
}