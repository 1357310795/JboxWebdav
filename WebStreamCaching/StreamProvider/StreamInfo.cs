using System;

namespace NutzCode.Libraries.Web.StreamProvider
{

    public class StreamInfo : IDisposable
    {
        private readonly StreamLocker _locker;
        public long OriginalBlock { get;  }
        public bool Faulted { get; set; } = false;
        public bool IsEmpty { get; }
        public string File { get; }
        public WebStream Stream { get; }
        public long CurrentBlock { get; set; }


        public void Dispose()
        {
            if (!IsEmpty)
               _locker.ReturnStreamAndMakeItInactive(this);

        }

        public StreamInfo(StreamLocker locker, string file, WebStream stream, long block) 
        {
            _locker = locker;
            File = file;
            Stream = stream;
            CurrentBlock = block;
            OriginalBlock = block;
            IsEmpty = false;
        }

        public StreamInfo() 
        {
            IsEmpty = true;
        }
    }
}
