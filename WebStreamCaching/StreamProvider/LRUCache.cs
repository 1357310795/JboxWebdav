using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NutzCode.Libraries.Web.StreamProvider
{
    //Original code from stackoverflow user Martin
    //
    //http://stackoverflow.com/questions/754233/is-it-there-any-lru-implementation-of-idictionary
    //
    //Heavy modified to suit

    // ReSharper disable once InconsistentNaming
    public class LRUCache<T,TS> : IDisposable
    {
        internal readonly int _capacity;
        protected Dictionary<T, LinkedListNode<LRUCacheItem<T, TS>>> _cacheMap = new Dictionary<T, LinkedListNode<LRUCacheItem<T, TS>>>();
        protected readonly LinkedList<LRUCacheItem<T, TS>> _lruList = new LinkedList<LRUCacheItem<T, TS>>();

        public LRUCache(int capacity)
        {
            _capacity = capacity;
        }
        public TS this[T key]
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                LinkedListNode<LRUCacheItem<T, TS>> node;
                if (_cacheMap.TryGetValue(key, out node))
                {
                    TS value = node.Value.Value;
                    _lruList.Remove(node);
                    _lruList.AddLast(node);
                    return value;
                }
                return default(TS);
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                LinkedListNode<LRUCacheItem<T, TS>> node;
                if (_cacheMap.TryGetValue(key, out node))
                {
                    _lruList.Remove(node);
                    _cacheMap.Remove(node.Value.Key);
                }
                if (_cacheMap.Count >= _capacity)
                {
                     node = _lruList.First;
                    _lruList.RemoveFirst();
                    _cacheMap.Remove(node.Value.Key);
                }
                LRUCacheItem<T, TS> cacheItem = new LRUCacheItem<T, TS>(key, value);
                node = new LinkedListNode<LRUCacheItem<T, TS>>(cacheItem);
                _lruList.AddLast(node);
                _cacheMap.Add(key, node);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Remove(T key)
        {
            LinkedListNode<LRUCacheItem<T, TS>> node;
            if (_cacheMap.TryGetValue(key, out node))
            {
                _lruList.Remove(node);
                _cacheMap.Remove(node.Value.Key);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            _cacheMap.Clear();
            _lruList.Clear();
            GC.Collect();
        }
    }
}
