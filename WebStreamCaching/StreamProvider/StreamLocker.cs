using Nito.AsyncEx;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NutzCode.Libraries.Web.StreamProvider
{
    public class StreamLocker 
    {
        private readonly ActiveStreamCache _activeStreams;
        private readonly InactiveStreamsCache _inactiveStreams;
        //private readonly AsyncReaderWriterLock _rw = new AsyncReaderWriterLock();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);
        public StreamLocker(int maxInactiveStreams)
        {
            _inactiveStreams=new InactiveStreamsCache(maxInactiveStreams);
            _activeStreams= new ActiveStreamCache();
        }

        public void Dispose()
        {
            using (_lock.Lock())
            {
                _activeStreams.Dispose();
                _inactiveStreams.Dispose();
            }
        }


        public void RemoveFile(string file)
        {
            using (_lock.Lock())
            {
                _activeStreams.RemoveAndDisposeKey(file);
                _inactiveStreams.RemoveAndDisposeKey(file);
            }
        }


        
        public async Task<StreamInfo> GetOrCreateActiveStream(string key, long blockposition, int maxBlockDistance, Func<CancellationToken, Task<WebStream>> create, CancellationToken token)
        {
            StreamInfo info=null;
            try
            {
                using (await _lock.LockAsync(token))
                {
                    if (_activeStreams.Keys.Any(a => a.Item1 == key && a.Item2 >= blockposition && a.Item2 <= blockposition + maxBlockDistance))
                        return new StreamInfo(); //Empty One

                    Tuple<WebStream, long> n = _inactiveStreams.CheckAndRemove(key, blockposition, maxBlockDistance);
                    if (n != null)
                    {
                        _activeStreams[new Tuple<string, long>(key, n.Item2)] = n.Item1;
                        info = new StreamInfo(this, key, n.Item1, n.Item2);
                    }
                    else
                    {
                        _activeStreams[new Tuple<string, long>(key, blockposition)] = null; //Lock the file/position 
                    }

                    if (info == null)
                    {
                        WebStream s = await create(token);
                        if ((s.StatusCode != HttpStatusCode.OK) && (s.StatusCode != HttpStatusCode.PartialContent))
                        {
                            _activeStreams.Remove(key, blockposition);
                            throw new IOException("Http Status (" + s.StatusCode + ")");
                        }
                        _activeStreams[Tuple.Create(key, blockposition)] = s;
                        info = new StreamInfo(this, key, s, blockposition);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            return info;
        }

        internal void ReturnStreamAndMakeItInactive(StreamInfo info)
        {
            using (_lock.Lock())
            {
                _activeStreams.Remove(info.File, info.OriginalBlock);
                if (info.Stream.ContentLength != info.Stream.Position && !info.Faulted)
                    _inactiveStreams[Tuple.Create(info.File, info.CurrentBlock)] = info.Stream;
                else
                    info.Stream.Dispose();
            }

        }
        /*
        public WebStream RemoveActive(string file, long block)
        {
            using (_rw.WriterLock())
            {
                return _activeStreams.Remove(file, block);
            }
        }

        public bool IsActive(string file, long blockposition, int maxBlockDistance)
        {
            lock (_lock)
            {
                return _activeStreams.Keys.FirstOrDefault(a => a.Item1 == file && a.Item2 >= blockposition && a.Item2 <= blockposition + maxBlockDistance) != null;
            }
        }


        public StreamInfo GetStreamAndMakeItActive(string file, long blockposition, int maxBlockDistance)
        {
            lock (_lock)
            {
                Tuple<WebStream, long> n = _inactiveStreams.CheckAndRemove(file, blockposition, maxBlockDistance);
                if (n != null)
                {
                    _activeStreams[new Tuple<string, long>(file, n.Item2)] = n.Item1;
                    return new StreamInfo(file, n.Item1, n.Item2);
                }
                _activeStreams[new Tuple<string, long>(file, blockposition)] = null; //Lock the file/position 
                return null;
            }
        }



        public void ReturnStreamAndMakeItInactive(long oldblock, StreamInfo info)
        {
            lock (_lock)
            {
                _activeStreams.Remove(info.File, oldblock);
                if (info.Stream.ContentLength != info.Stream.Position)
                    _inactiveStreams[Tuple.Create(info.File, info.CurrentBlock)] = info.Stream;
                else
                    info.Stream.Dispose();
            }

        }

        public StreamInfo AddNewStreamAndMakeItActive(string file, long block, WebStream w)
        {
            lock (_lock)
            {
                _activeStreams[Tuple.Create(file, block)] = w;
                return new StreamInfo(file, w, block);
            }
        }
                */

    }
}
