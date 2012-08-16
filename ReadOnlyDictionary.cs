// Copyright (c) 2007-2012 Andrej Repin aka Gremlin2
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;

namespace FB2Fix
{
    internal class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> innerDictionary;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> innerDictionary)
        {
            if (innerDictionary == null)
            {
                throw new ArgumentNullException("innerDictionary");
            }

            this.innerDictionary = innerDictionary;
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException("The dictionary is readonly and can not be modified.");
        }

        public bool ContainsKey(TKey key)
        {
            return this.innerDictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return this.innerDictionary.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            throw new NotSupportedException("The dictionary is readonly and can not be modified.");
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.innerDictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get
            {
                return this.innerDictionary.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return this.innerDictionary[key];
            }
            set
            {
                throw new NotSupportedException("The dictionary is readonly and can not be modified.");
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("The dictionary is readonly and can not be modified.");
        }

        public void Clear()
        {
            throw new NotSupportedException("The dictionary is readonly and can not be modified.");
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.innerDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.innerDictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return this.innerDictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("The dictionary is readonly and can not be modified.");
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.innerDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this.innerDictionary).GetEnumerator();
        }
    }
}