using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;

namespace TextUtils
{
    public class BlockCollection : IList, ICollection,IEnumerable
    {
        private IList _listImplementation = new List<Block>();

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            return _listImplementation.GetEnumerator();
        }

        /// <inheritdoc />
        public void CopyTo(Array array, int index)
        {
            _listImplementation.CopyTo(array, index);
        }

        /// <inheritdoc />
        public int Count
        {
            get { return _listImplementation.Count; }
        }

        /// <inheritdoc />
        public bool IsSynchronized
        {
            get { return _listImplementation.IsSynchronized; }
        }

        /// <inheritdoc />
        public object SyncRoot
        {
            get { return _listImplementation.SyncRoot; }
        }

        /// <inheritdoc />
        public int Add(object? value)
        {
            return _listImplementation.Add(value);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _listImplementation.Clear();
        }

        /// <inheritdoc />
        public bool Contains(object? value)
        {
            return _listImplementation.Contains(value);
        }

        /// <inheritdoc />
        public int IndexOf(object? value)
        {
            return _listImplementation.IndexOf(value);
        }

        /// <inheritdoc />
        public void Insert(int index, object? value)
        {
            _listImplementation.Insert(index, value);
        }

        /// <inheritdoc />
        public void Remove(object? value)
        {
            _listImplementation.Remove(value);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            _listImplementation.RemoveAt(index);
        }

        /// <inheritdoc />
        public bool IsFixedSize
        {
            get { return _listImplementation.IsFixedSize; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return _listImplementation.IsReadOnly; }
        }

        /// <inheritdoc />
        public object? this[int index]
        {
            get { return _listImplementation[index]; }
            set { _listImplementation[index] = value; }
        }
    }
}