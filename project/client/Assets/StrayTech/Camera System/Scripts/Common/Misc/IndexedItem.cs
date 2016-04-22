using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace StrayTech
{
    /// <summary>
    /// Represents a Unity-Serializable KeyValuePair.
    /// </summary>
    [Serializable]
    public abstract class IndexedItem<TKey, TValue> : IValidates
    {
        #region inspector members
            /// <summary>
            /// This IndexedItem's Key.
            /// </summary>
            [SerializeField]
            private TKey _ID;

            /// <summary>
            /// This IndexedItem's Value.
            /// </summary>
            [SerializeField]
            private TValue _Value;
        #endregion inspector members

        #region properties
            /// <summary>
            /// This IndexedItem's Key.
            /// </summary>
            public TKey ID
            {
                get
                {
                    return this._ID;
                }
                protected set
                {
                    this._ID = value;
                }
            }

            /// <summary>
            /// This IndexedItem's Value.
            /// </summary>
            public TValue Value
            {
                get
                {
                    return this._Value;
                }
                protected set
                {
                    this._Value = value;
                }
            }
        #endregion properties

        #region constructors
            /// <summary>
            /// Creates a new IndexedItem&lt;TKey, TValue&lt;.
            /// </summary>
            public IndexedItem()
                : this(default(TKey), default(TValue)) { }

            /// <summary>
            /// Creates a new IndexedItem&lt;TKey, TValue&lt; with the given values.
            /// </summary>
            public IndexedItem(TKey ID, TValue value)
            {
                this._ID = ID;
                this._Value = value;
            }
        #endregion constructors

        #region methods
            /// <summary>
            /// Checks whether or not this IndexedItem&lt;TKey,TValue&gt; is valid.
            /// </summary>
            public abstract bool IsValid();
        #endregion methods
    }

    public static class IndexedItemExtensions
    {
        /// <summary>
        /// Inflates a List of IndexedItems into a Dicionary of them if there are no duplicate keys.
        /// </summary>
        /// <typeparam name="TSource">A type derived from IndexedItem&lt;TKey, TValue&gt; that is the type of elements in this list.</typeparam>
        /// <typeparam name="TKey">The Key type of the return dictionary.</typeparam>
        /// <typeparam name="TValue">The Value type of the return dictionary.</typeparam>
        /// <param name="toInflate">This collection.</param>
        /// <returns>A Dictionary&lt;TKey, TValue&gt; continaing the IndexedItems in the given list, expanded into KeyValuePairs. </returns>
        public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(this List<TSource> toInflate)
            where TSource : IndexedItem<TKey, TValue>
        {
            Dictionary<TKey, TValue> output = new Dictionary<TKey, TValue>();

            foreach (TSource toInsert in toInflate)
            {
                if (toInsert == null)
                {
                    Debug.LogException(new ArgumentNullException("An element in toInflate was null."));
                    continue;
                }

                if (toInsert.IsValid() == false)
                {
                    Debug.LogException(new FailedValidationException("An element in toInflate was not valid!"));
                    continue;
                }

                if (output.ContainsKey(toInsert.ID))
                {
                    Debug.LogException(new ArgumentException("Found a duplicate key when trying to inflate a List of IndexedItems!"));
                    return new Dictionary<TKey, TValue>();
                }

                output.Add(toInsert.ID, toInsert.Value);
            }

            return output;
        }

        /// <summary>
        /// Deflates a Dictionary into an List of IndexedItems that can be serialized by Unity.
        /// </summary>
        /// <typeparam name="TSource">A type that derives from IndexedItem&lt;TKey, TValue&gt;, which will be the type of elements in the list.</typeparam>
        /// <typeparam name="TKey">The type of the Keys in the source Dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the Values in the source Dictionary.</typeparam>
        /// <param name="toDeflate">This Dictionary.</param>
        /// <returns>A list of TSource which represent the KeyValuePairs in the source Dictionary.</returns>
        //public static List<TSource> ToIndexedList<TSource, TKey, TValue>(this Dictionary<TKey, TValue> toDeflate)
        //    where TSource : IndexedItem<TKey, TValue>, new()
        //{
        //    List<TSource> output = new List<TSource>();

        //    foreach (KeyValuePair<TKey, TValue> kvp in toDeflate)
        //    {
        //        TSource newElement = new TSource();
        //        newElement.ID = kvp.Key;
        //        newElement.Value = kvp.Value;

        //        output.Add(newElement);
        //    }

        //    return output;
        //}
    }
}