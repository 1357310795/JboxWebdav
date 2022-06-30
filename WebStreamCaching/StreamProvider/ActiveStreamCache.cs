using System;
using System.Collections.Generic;
using System.Linq;

namespace NutzCode.Libraries.Web.StreamProvider
{
    internal class ActiveStreamCache : Dictionary<Tuple<string, long>, WebStream>, IDisposable
    {
        public void Dispose()
        {
            Keys.ToList().ForEach(a =>
            {
                this[a]?.Dispose();
                Remove(a);
            });
        }
        public void RemoveAndDisposeKey(string key)
        {

            Keys.Where(a => a.Item1 == key).ToList().ForEach(a =>
            {
                this[a]?.Dispose();
                Remove(a);
            });
        }
        public WebStream Remove(string key, long block)
        {
            Tuple<string, long> k = Keys.FirstOrDefault(a => a.Item1 == key && a.Item2 == block);
            if (k == null)
                return null;
            WebStream w = this[k];
            Remove(k);
            return w;       
        }

    }
}
