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

namespace FB2Fix.Sgml
{
    public class Set<TItem> : ICollection<TItem>
    {
        private readonly bool isReadOnly;
        private readonly Dictionary<TItem, object> dictionary;

        public Set() : this(10)
        {
        }

        public Set(int capacity)
        {
            dictionary = new Dictionary<TItem, object>(capacity);
            isReadOnly = false;
        }

        private Set(Set<TItem> innerSet)
        {
            dictionary = innerSet.dictionary;
            isReadOnly = true;
        }

        public void Add(TItem item)
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException();
            }

            dictionary[item] = String.Empty;
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException();
            }

            foreach (TItem item in items)
            {
                dictionary[item] = String.Empty;
            }
        }


        public Set<TItem> AsReadOnly()
        {
            return new Set<TItem>(this);
        }

        public void Clear()
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException();
            }

            dictionary.Clear();
        }

        public bool Contains(TItem item)
        {
            return dictionary.ContainsKey(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            dictionary.Keys.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
            }
        }

        public bool Remove(TItem item)
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException();
            }

            return dictionary.Remove(item);
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return dictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}