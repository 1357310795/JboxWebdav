using System;
using System.Collections.Generic;
using System.Linq;

namespace NutzCode.Libraries.Web.StreamProvider
{

    //Original LRU Cache code from stackoverflow user Martin
    //
    //http://stackoverflow.com/questions/754233/is-it-there-any-lru-implementation-of-idictionary
    //
    //Heavy modified to suit

    public class InactiveStreamsCache : IDisposable
    {
        private readonly int _capacity;
        private readonly Dictionary<Tuple<string, long>, LinkedListNode<LRUCacheItem<Tuple<string,long>, WebStream>>> _cacheMap = new Dictionary<Tuple<string, long>, LinkedListNode<LRUCacheItem<Tuple<string, long>, WebStream>>>();
        private readonly LinkedList<LRUCacheItem<Tuple<string, long>, WebStream>> _lruList = new LinkedList<LRUCacheItem<Tuple<string, long>, WebStream>>();

        public InactiveStreamsCache(int capacity)
        {
            _capacity = capacity;
        }

        public WebStream this[Tuple<string, long> key] //Tuple<url of the file, position of the offset>
        {
            get
            {
                LinkedListNode<LRUCacheItem<Tuple<string, long>, WebStream>> node;
                if (_cacheMap.TryGetValue(key, out node))
                {
                    WebStream value = node.Value.Value;
                    _lruList.Remove(node);
                    _lruList.AddLast(node);
                    return value;
                }
                return null;
            }
            set
            {
                if (_cacheMap.ContainsKey(key))
                    return;
                LinkedListNode<LRUCacheItem<Tuple<string, long>, WebStream>> node;
                if (_cacheMap.Count >= _capacity)
                {
                    node = _lruList.First;
                    _lruList.RemoveFirst();
                    _cacheMap.Remove(node.Value.Key);
                    node.Value.Value.Dispose();
                }
                LRUCacheItem<Tuple<string, long>, WebStream> cacheItem = new LRUCacheItem<Tuple<string, long>, WebStream>(key, value);
                node = new LinkedListNode<LRUCacheItem<Tuple<string, long>, WebStream>>(cacheItem);
                _lruList.AddLast(node);
                _cacheMap.Add(key, node);
            }
        }


        public void RemoveAndDisposeKey(string key)
        {
            _cacheMap.Where(a => a.Key.Item1 == key).ToList().ForEach(a =>
            {
                LinkedListNode<LRUCacheItem<Tuple<string, long>, WebStream>> node;
                if (_cacheMap.TryGetValue(a.Key, out node))
                {
                    _lruList.Remove(node);
                    _cacheMap.Remove(node.Value.Key);
                    node.Value.Value.Dispose();
                }
            });
        }

        public Tuple<WebStream, long> CheckAndRemove(string key, long block, int maxBlockDistance)
        {
            LinkedListNode<LRUCacheItem<Tuple<string, long>, WebStream>> node;
            Tuple<string,long> k=_cacheMap.FirstOrDefault(a => a.Key.Item1 == key && a.Key.Item2 >= block && a.Key.Item2 <= block + maxBlockDistance).Key;
            if (k!=null && _cacheMap.TryGetValue(k, out node))
            {
                _lruList.Remove(node);
                _cacheMap.Remove(node.Value.Key);
                return new Tuple<WebStream, long>(node.Value.Value,node.Value.Key.Item2);
            }
            return null;
        }


        public void Dispose()
        {
            _cacheMap.Keys.ToList().ForEach(a =>
            {
                LinkedListNode<LRUCacheItem<Tuple<string, long>, WebStream>> node;
                if (_cacheMap.TryGetValue(a, out node))
                {
                    _lruList.Remove(node);
                    _cacheMap.Remove(node.Value.Key);
                    node.Value.Value.Dispose();
                }
            });
        }
    }

}
