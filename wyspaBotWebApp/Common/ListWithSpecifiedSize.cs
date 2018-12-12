using System;
using System.Collections;
using System.Collections.Generic;

namespace wyspaBotWebApp.Common {
    public class ListWithSpecifiedSize<T> : IList<T> {
        private readonly IList<T> list;

        private readonly int size;

        public int Count => this.list.Count;

        public bool IsReadOnly => this.list.IsReadOnly;

        public ListWithSpecifiedSize(int size) {
            this.size = size;
            this.list = new List<T>();
        }

        public IEnumerator<T> GetEnumerator() {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void Add(T item) {
            this.list.Add(item);
            if (this.list.Count > this.size) {
                this.list.RemoveAt(0);
            }
        }

        public void Clear() {
            this.list.Clear();
        }

        public bool Contains(T item) {
            return this.list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            this.list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            return this.list.Remove(item);
        }

        public int IndexOf(T item) {
            return this.list.IndexOf(item);
        }

        public void Insert(int index, T item) {
            if (index > this.size) {
                throw new InvalidOperationException("Index can not be bigger than defined size!");
            }

            this.list.Insert(index, item);
        }

        public void RemoveAt(int index) {
            if (index > this.size) {
                throw new InvalidOperationException("Index can not be bigger than defined size!");
            }

            this.list.RemoveAt(index);
        }

        public T this[int index] {
            get {
                if (index < 0 || index >= this.Count || index > this.size) {
                    throw new InvalidOperationException("Index must be in range from 0 to current count or specified size!");
                }
                return this.list[index];
            }
            set => this.list.Insert(index, value);
        }
    }
}