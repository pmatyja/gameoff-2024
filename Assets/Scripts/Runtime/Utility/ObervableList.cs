using System;
using System.Collections.Generic;

namespace Utility.Generics
{
    [Serializable]
    public class ObservableList<T> : List<T>
    {
        public event Action<T> ElementAdded;
        public event Action<int, T> ElementInserted;
        public event Action<int, T> ElementRemovedAt;
        public event Action<T> ElementRemoved;
        public event Action Cleared;
        public event Action Sorted;

        public new void Add(T item)
        {
            base.Add(item);
            ElementAdded?.Invoke(item);
        }
        
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            ElementInserted?.Invoke(index, item);
        }
        
        public new bool Remove(T item)
        {
            var result = base.Remove(item);
            if (result)
            {
                ElementRemoved?.Invoke(item);
            }
            return result;
        }
        
        public new void RemoveAt(int index)
        {
            var item = this[index];
            base.RemoveAt(index);
            ElementRemovedAt?.Invoke(index, item);
        }

        public new void Clear()
        {
            Cleared?.Invoke();
            base.Clear();
        }
        
        public new void Sort()
        {
            base.Sort();
            Sorted?.Invoke();
        }
        
        public void RemoveDuplicates()
        {
            var hashSet = new HashSet<T>();
            for (var i = Count - 1; i >= 0; i--)
            {
                if (!hashSet.Add(this[i]))
                {
                    RemoveAt(i);
                }
            }
        }
        
        public bool IsReadOnly => ((ICollection<T>)this).IsReadOnly;
    }
}