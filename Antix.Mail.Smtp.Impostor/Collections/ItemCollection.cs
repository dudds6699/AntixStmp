//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Antix.Mail.Smtp.Impostor.Collections {
    /// <summary>
    ///   <para>A keyed (string) collection which can only be edited internally</para>
    /// </summary>
    /// <typeparam name = "TItem">Type of Item</typeparam>
    public abstract class ItemCollection<TItem> : IEnumerable<TItem> {
        /// <summary>
        ///   <para>Create and add a number of items</para>
        /// </summary>
        /// <param name = "items"></param>
        protected ItemCollection(IEnumerable<TItem> items) {
            _values = new ConcurrentDictionary<string, TItem>(
                items.Select(i => new KeyValuePair<string, TItem>(GetKey(i), i)));
        }

        /// <summary>
        ///   <para>Create object (internal)</para>
        /// </summary>
        internal ItemCollection() {
            _values = new ConcurrentDictionary<string, TItem>();
        }

        #region storage, private or internal

        private readonly ConcurrentDictionary<string, TItem> _values;

        /// <summary>
        ///   <para>Adds a item to the collection</para>
        /// </summary>
        /// <param name = "item">Item to add</param>
        internal void Add(TItem item) {
            var key = GetKey(item);

            if (Contains(key)) throw new ItemExistsException(key);

            _values.TryAdd(key, item);
        }

        /// <summary>
        ///   <para>Remove the given item</para>
        /// </summary>
        /// <param name = "item">Item</param>
        internal void Remove(TItem item) {
            Remove(GetKey(item));
        }

        /// <summary>
        ///   <para>Remove given the item Key</para>
        /// </summary>
        /// <param name = "key">Key</param>
        internal void Remove(string key) {
            if (!Contains(key)) throw new ItemNotFoundException(key);

            TItem item;
            _values.TryRemove(key, out item);
        }

        #endregion

        #region public properties and methods

        /// <summary>
        ///   <para>Gets an item by key</para>
        /// </summary>
        /// <param name = "key">Session Key (IP:Port)</param>
        /// <returns>Session if found or null</returns>
        public TItem this[string key] {
            get { return _values[key]; }
        }

        /// <summary>
        ///   <para>Gets the number of items in the collection</para>
        /// </summary>
        public int Count {
            get { return _values.Count; }
        }

        /// <summary>
        ///   <para>Gets the enumerator</para>
        /// </summary>
        public IEnumerator<TItem> GetEnumerator() {
            return _values.Values.GetEnumerator();
        }

        /// <summary>
        ///   <para>Gets the enumerator</para>
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        ///   <para>Gets the key given the item</para>
        /// </summary>
        /// <param name = "item">Item</param>
        /// <returns>Key value</returns>
        public abstract string GetKey(TItem item);

        /// <summary>
        ///   <para>Check for key in the collection</para>
        /// </summary>
        /// <param name = "key">key</param>
        /// <returns>True if found</returns>
        public bool Contains(string key) {
            return _values.ContainsKey(key);
        }

        /// <summary>
        ///   <para>Clear all items</para>
        /// </summary>
        public void Clear() {
            _values.Clear();
        }

        #endregion
    }
}