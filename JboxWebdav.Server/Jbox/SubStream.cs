using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JboxWebdav.Server.Jbox
{
    internal class SubStream : Stream
    {
        private readonly long _startInSuperStream;
        private long _positionInSuperStream;
        private readonly long _endInSuperStream;
        private readonly Stream _superStream;
        private bool _canRead;
        private bool _isDisposed;

        public SubStream(Stream superStream, long startPosition, long endPosition)
        {
            this._startInSuperStream = startPosition;
            this._positionInSuperStream = startPosition;
            this._endInSuperStream = endPosition + 1;
            this._superStream = superStream;
            this._canRead = true;
            this._isDisposed = false;
        }

        public override long Length
        {
            get
            {
                ThrowIfDisposed();

                return _endInSuperStream - _startInSuperStream;
            }
        }

        public override long Position
        {
            get
            {
                ThrowIfDisposed();

                return _positionInSuperStream - _startInSuperStream;
            }
            set
            {
                ThrowIfDisposed();

                throw new NotSupportedException("seek not support");
            }
        }

        public override bool CanRead => _superStream.CanRead && _canRead;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().ToString(), nameof(this._superStream));
        }

        private void ThrowIfCantRead()
        {
            if (!CanRead)
                throw new NotSupportedException("read not support");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // parameter validation sent to _superStream.Read
            int origCount = count;

            ThrowIfDisposed();
            ThrowIfCantRead();

            if (_superStream.Position != _positionInSuperStream)
                _superStream.Seek(_positionInSuperStream, SeekOrigin.Begin);
            if (_positionInSuperStream + count > _endInSuperStream)
                count = (int)(_endInSuperStream - _positionInSuperStream);

            int ret = _superStream.Read(buffer, offset, count);

            _positionInSuperStream += ret;
            return ret;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfDisposed();
            throw new NotSupportedException("seek not support");
        }

        public override void SetLength(long value)
        {
            ThrowIfDisposed();
            throw new NotSupportedException("seek and write not support");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();
            throw new NotSupportedException("write not support");
        }

        public override void Flush()
        {
            ThrowIfDisposed();
            throw new NotSupportedException("write not support");
        }

        // Close the stream for reading.  Note that this does NOT close the superStream (since
        // the subStream is just 'a chunk' of the super-stream
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                _canRead = false;
                _isDisposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
