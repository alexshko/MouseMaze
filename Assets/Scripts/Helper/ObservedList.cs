using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace alexshko.colamazle.helpers
{
    public class ObservedList<T> : List<T>
    {
        public Action OnListChanged;

        public new void Add(T item)
        {
            base.Add(item);
            if (OnListChanged != null)
            {
                OnListChanged();
            }
        }

        public new bool Remove(T item)
        {
            bool result = base.Remove(item);
            if (result && OnListChanged != null)
            {
                OnListChanged();
            }
            return result;
        }
    }
}
