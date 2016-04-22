using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace StrayTech
{
    /// <summary>
    /// Houses extension methods relating to .net collections. 
    /// </summary>
    public static class CollectionExtension
    {
        /// <summary>
        /// Adds/Updates the given Key and Value in this dictionary regardless of whether or not it already contains the Key.
        /// </summary>
        public static void AddOrSet<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, TValue value)
        {
            if (self == null)
            {
                Debug.LogException(new ArgumentNullException("self"));
                return;
            }

            if (!self.ContainsKey(key))
            {
                self.Add(key, value);
            }
            else
            {
                self[key] = value;
            }
        }

        /// <summary>
        /// Wraps the given index around the count of the collection. 
        /// For example given a list count of 11, and an index value of 12, will return index 1
        /// </summary>
        public static int WrapIndex(this ICollection self, int index)
        {
            if (self == null)
            {
                Debug.LogException(new ArgumentNullException("self"));
                return default(int);
            }

            //Wrap the index around the count. 
            return WrapIndex(index, self.Count);
        }

        /// <summary>
        /// Wraps the given index around the count of the collection. 
        /// For example given a list count of 11, and an index value of 12, will return index 1
        /// </summary>
        public static int WrapIndex(int index, int collectionCount)
        {
            if (collectionCount == 0)
                return 0;

            //Ensure positive. 
            collectionCount = Mathf.Abs(collectionCount);

            //Wrap that index. 
            return (collectionCount + index) % collectionCount;
        }

        /// <summary>
        /// Clamp the index from 0 to (collection count - 1)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="collectionCount"></param>
        /// <returns></returns>
        public static int ClampIndex(this ICollection self, int index)
        {
            if (self == null)
            {
                Debug.LogException(new ArgumentNullException("self"));
                return default(int);
            }

            if (self.Count <= 0)
                return 0;

            return Mathf.Clamp(index, 0, self.Count - 1);
        }

        /// <summary>
        /// Return a random element in the list. 
        /// </summary>
        public static T GetRandomElement<T>(this List<T> self)
        {
            if (self == null)
            {
                Debug.LogException(new ArgumentNullException("self"));
                return default(T);
            }

            if (self.Count == 0)
                return default(T);
            if (self.Count == 1)
                return self[0];

            return self[UnityEngine.Random.Range(0, self.Count)];
        }

        /// <summary>
        /// Return a random element in the provided hashset. 
        /// </summary>
        public static T GetRandomElement<T>(this HashSet<T> self)
        {
            if (self == null)
            {
                Debug.LogException(new ArgumentNullException("self"));
                return default(T);
            }

            if (self.Count == 0)
                return default(T);
            if (self.Count == 1)
                return self.ElementAt(0);

            var randomIndex = UnityEngine.Random.Range(0, self.Count);

            return self.ElementAt(randomIndex);
        }

        /// <summary>
        /// Return a random element in the provided IEnumerable. 
        /// </summary>
        public static T GetRandomElement<T>(this IEnumerable<T> self)
        {
            if (self == null)
            {
                return default(T);
            }

            var count = self.Count();

            if (count == 0)
            {
                return default(T);
            }
            if (count == 1)
            {
                return self.ElementAt(0);
            }

            return self.ElementAt(UnityEngine.Random.Range(0, count));
        }

        /// <summary>
        /// Generate a list with n size of T value. 
        /// </summary>
        public static List<T> GenerateListWithValues<T>(T value, int count)
        {
            count = Mathf.Max(0, count);

            var toReturn = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                toReturn.Add(value);
            }

            return toReturn;
        }

        /// <summary>
        /// Shuffle the elements contained in the array. 
        /// </summary>
        public static void Shuffle<T>(this T[] self)
        {
            if (self == null)
            {
                Debug.LogException(new ArgumentNullException("self"));
                return;
            }

            for (int i = self.Length; i > 1; i--)
            {
                int j = UnityEngine.Random.Range(0, i);
                T tmp = self[j];
                self[j] = self[i - 1];
                self[i - 1] = tmp;
            }
        }

        /// <summary>
        /// Scrape all of the items in the collection which are not null and have passed validation. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<T> ScrapeValidItems<T>(this IEnumerable<T> self)
            where T : IValidates
        {
            if (self == null)
            {
                Debug.LogException(new ArgumentNullException("self"));
                yield break;
            }

            foreach (var item in self)
            {
                if (item == null || item.IsValid() == false)
                    continue;

                yield return item;
            }
        }

        /// <summary>
        /// Scrape all of the items in the collection which are not null and have passed validation. 
        /// </summary>
        public static HashSet<T> ScrapeValidAndUniqueItems<T>(this IEnumerable<T> self)
            where T : IValidates
        {
            HashSet<T> toReturn = new HashSet<T>();

            if (self == null)
            {
                Debug.LogException(new ArgumentNullException("source"));
                return toReturn;
            }

            foreach (var item in self)
            {
                if (item.IsNullOrInvalid() == true)
                    continue;

                if (toReturn.Contains(item) == true)
                    continue;

                toReturn.Add(item);
            }

            return toReturn;
        }

        /// <summary>
        /// Scrape all of the items in the collection which are non null and unique. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<T> ScrapeNonNullAndUniqueItems<T>(this IEnumerable<T> self)
        {
            if (self == null)
            {
                yield break;
            }

            HashSet<T> toReturn = new HashSet<T>();

            foreach (var item in self)
            {
                if (item == null)
                    continue;

                if (toReturn.Contains(item) == true)
                    continue;

                toReturn.Add(item);
            }

            foreach (var item in toReturn)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Scrape all of the items in the collection which are not null.
        /// </summary>
        public static IEnumerable<T> ScrapeNonNullItems<T>(this IEnumerable<T> self)
            where T : class
        {
            if (self == null)
            {
                Debug.LogException(new ArgumentNullException("source"));
                yield break;
            }

            foreach (var item in self)
            {
                if (item == null)
                    continue;

                yield return item;
            }
        }

        public static Dictionary<TKey, TValue> ToDictionary<TSelf, TKey, TValue>(this TSelf self)
            where TSelf : List<IndexedItem<TKey, TValue>>
        {
            var toReturn = new Dictionary<TKey, TValue>();

            if (self == null)
                return toReturn;

            foreach (var item in self)
            {
                if (item.IsNullOrInvalid() == true)
                    continue;

                if (toReturn.ContainsKey(item.ID))
                    continue;

                toReturn.Add(item.ID, item.Value);
            }

            return toReturn;
        }

        /// <summary>
        /// Returns the element from the collection at the provided index. 
        /// Will not throw an exception for invalid index, just log and return default(T)
        /// </summary>
        public static T ElementAtSafe<T>(this List<T> self, int index)
        {
            if (self == null)
                return default(T);

            if (index < 0 || index >= self.Count)
            {
                Debug.LogError(string.Format("Requested index {0} is out of range of collection!", index));
                return default(T);
            }

            return self[index];
        }

        /// <summary>
        /// Is the provided collection null or empty? 
        /// </summary>
        public static bool IsNullOrEmpty(this ICollection self)
        {
            return self == null || self.Count == 0;
        }

        /// <summary>
        /// Helper method which turns a single item into an enumerable. 
        /// </summary>
        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            if (item == null)
                yield break;

            yield return item;
        }

        /// <summary>
        /// Returns a collection of elements extracted from the elements of this collection.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the original collection.</typeparam>
        /// <typeparam name="TField">The type of field extracted from the original collection's elements.</typeparam>
        /// <param name="source">This collection.</param>
        /// <param name="fieldExtractor">A method that transforms an element of TSource into an element of TField. null elements in the source collection are NEVER passed into this method.</param>
        /// <returns>An enumerable of TField, generated from the source collection.</returns>
        public static IEnumerable<TField> Field<TSource, TField>(this IEnumerable<TSource> source, Func<TSource, TField> fieldExtractor)
        {
            if (fieldExtractor == null)
            {
                Debug.LogException(new ArgumentException("fieldExtractor cannot be null!"));
                yield break;
            }

            foreach (var item in source)
            {
                // skip null entries
                if (item == null)
                {
                    continue;
                }

                yield return fieldExtractor(item);
            }
        }

        public static void Remove<T>(this T[] self, T toRemove)
            where T : class
        {
            if (self == null)
            {
                return;
            }

            for (int i = 0; i < self.Length; i++)
            {
                if (self[i] == toRemove)
                {
                    self[i] = null;
                }
            }
        }

        /// <summary>
        /// Its like IEnumerable FirstOrDefault, but without the Garbage allocation
        /// because it operates on a List. Usefult for tight, time sensitive loops. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T FirstOrDefaultQuick<T>(this List<T> self)
        {
            if (self == null)
            {
                return default(T);
            }

            if (self.Count == 0)
            {
                return default(T);
            }

            return self[0];
        }

        /// <summary>
        /// Slice a subset of the provided array. 
        /// </summary>
        public static T[] Subset<T>(this T[] source, int length)
        {
            //Garbage in, garbage out. 
            if (source == null)
            {
                return null;
            }

            //Ensure bounds. 
            length = Mathf.Clamp(length, 0, source.Length);

            //Generate our new array. 
            T[] toReturn = new T[length];
            //Copy our source into our new array. 
            Array.Copy(source, 0, toReturn, 0, length);

            return toReturn;
        }

        /// <summary>
        /// We cannot create a HashSet with a default capacity! To prevent
        /// resizing during runtime we will add elements to the hashset then clear
        /// it so it will have enough space internally to do it's operations without 
        /// the need to re-size. 
        /// </summary>
        public static void PreallocateCapacity(this HashSet<int> toPopulate, int capacity)
        {
            if (toPopulate == null)
            {
                return;
            }

            capacity = Mathf.Max(0, capacity);

            for (int i = 0; i < capacity; i++)
            {
                toPopulate.Add(i);
            }

            toPopulate.Clear();
        }

        /// <summary>
        /// Add the subest of self defined from start index for count on to the end of self. 
        /// </summary>
        public static void AddSubsetToSelf<T>(this List<T> self, int startIndex, int count)
        {
            //If null or empty nothing to do. 
            if (self == null || self.Count == 0)
            {
                return;
            }

            //If length is 0 or less, nothing to do. 
            if (count <= 0)
            {
                return;
            }

            //Bail if given invalid start index. 
            if (startIndex < 0 || startIndex >= self.Count)
            {
                return;
            }

            //Get the end index by adding the count to the start index. 
            var endIndex = Mathf.Clamp(startIndex + count, 0, self.Count);

            //Add the specified subest. 
            for (int i = startIndex; i < endIndex; i++)
            {
                //Add the item at the current index to the end of the list. 
                self.Add(self[i]);
            }
        }

        /// <summary>
        /// Add the subset of the specified list defined from start index and extending for count onto the end our ourself. 
        /// </summary>
        public static void AddSubsetFromOther<T>(this List<T> self, List<T> other, int startIndex, int count)
        {
            //If null nothing to do. 
            if (self == null)
            {
                return;
            }

            if (other == null || other.Count == 0)
            {
                return;
            }

            //If length is 0 or less, nothing to do. 
            if (count <= 0)
            {
                return;
            }

            //Bail if given invalid start index. 
            if (startIndex < 0 || startIndex >= other.Count)
            {
                return;
            }

            //Get the end index by adding the count to the start index. 
            var endIndex = Mathf.Clamp(startIndex + count, 0, other.Count);

            //Add the specified subest. 
            for (int i = startIndex; i < endIndex; i++)
            {
                //Add the item at the current index to the end of the list. 
                self.Add(other[i]);
            }
        }
    }
}