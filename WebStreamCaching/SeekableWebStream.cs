using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NutzCode.Libraries.Web.StreamProvider;

namespace NutzCode.Libraries.Web
{
    public class SeekableWebStream : Stream
    {

        private long _position;
        private long _length;
        private string _key;
        private Func<long, SeekableWebParameters> _resolver;
        private WebDataProvider _provider;


        public override void Close()
        {
        }


        public SeekableWebStream(string key, long maxsize, WebDataProvider provider, Func<long, SeekableWebParameters> webParameterResolver)
        {
            _length = maxsize;
            _resolver = webParameterResolver;
            _provider = provider;
            _key = key;
        }


        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _position = offset;
                    break;
                case SeekOrigin.Current:
                    _position += offset;
                    break;
                case SeekOrigin.End:
                    _position = _length + offset;
                    break;
            }
            if (_position < 0)
                _position = 0;
            if (_position > _length)
                _position = _length;
            return _position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Task.Run(() => ReadAsync(buffer, offset, count, new CancellationToken())).Result;
        }
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (count == 0)
                return 0;
            int cnt = await _provider.Read(_key, _resolver, _length, _position, buffer, offset, count, cancellationToken);
            _position += cnt;
            return cnt;
        }
   

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;

        public override long Length => _length;
        public override long Position
        {
            get
            {
                return _position;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }
    }
}
