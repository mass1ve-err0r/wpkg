using System;
using System.IO;

namespace WindowsPackager.ARFileFormat
{

    public class SubStream : Stream
    {

        private Stream stream;
        private long subStreamOffset;
        private long subStreamLength;
        private bool leaveParentOpen;
        private bool readOnly;
        private long position;

        public SubStream(Stream stream, long offset, long length, bool leaveParentOpen = false, bool readOnly = false) {
            this.stream = stream;
            this.subStreamOffset = offset;
            this.subStreamLength = length;
            this.leaveParentOpen = leaveParentOpen;
            this.readOnly = readOnly;
            this.position = 0;

            if (this.stream.CanSeek) {
                this.Seek(0, SeekOrigin.Begin);
            }
        }

        public override bool CanRead
        {
            get
            {
                return this.stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return !this.readOnly && this.stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return this.subStreamLength;
            }
        }

        public override long Position
        {
            get
            {
                return this.position;
            }

            set
            {
                lock (this.stream) {
                    this.stream.Position = value + this.Offset;
                    this.position = value;
                }
            }
        }

        internal Stream Stream
        {
            get
            {
                return this.stream;
            }
        }

        internal long Offset
        {
            get
            {
                return this.subStreamOffset;
            }
        }

        public override void Flush() {
            lock (this.stream) {
                this.stream.Flush();
            }
        }

        public override int Read(byte[] buffer, int offset, int count) {
            lock (this.stream) {
                this.EnsurePosition();

                long bytesRemaining = this.Length - this.Position;
                long bytesToRead = Math.Min(count, bytesRemaining);

                if (bytesToRead < 0) {
                    bytesToRead = 0;
                }

                var read = this.stream.Read(buffer, offset, (int)bytesToRead);
                this.position += read;
                return read;
            }
        }

        public override void Write(byte[] buffer, int offset, int count) {
            if (this.readOnly) {
                throw new NotSupportedException();
            }

            lock (this.stream) {
                this.EnsurePosition();

                if (this.Position + offset + count > this.Length || this.Position < 0) {
                    throw new InvalidOperationException("This write operation would exceed the current length of the substream.");
                }

                this.stream.Write(buffer, offset, count);
                this.position += count;
            }
        }

        public override void WriteByte(byte value) {
            if (this.readOnly) {
                throw new NotSupportedException();
            }

            lock (this.stream) {
                this.EnsurePosition();

                if (this.Position > this.Length || this.Position < 0) {
                    throw new InvalidOperationException("This write operation would exceed the current length of the substream.");
                }

                this.stream.WriteByte(value);
                this.position++;
            }
        }

        public override long Seek(long offset, SeekOrigin origin) {
            lock (this.stream) {
                switch (origin) {
                    case SeekOrigin.Begin:
                        offset += this.subStreamOffset;
                        break;

                    case SeekOrigin.End:
                        long enddelta = this.subStreamOffset + this.subStreamLength - this.stream.Length;
                        offset += enddelta;
                        break;

                    case SeekOrigin.Current:
                        break;
                }

                if (origin == SeekOrigin.Current) {
                    this.EnsurePosition();
                }

                var parentPosition = this.stream.Seek(offset, origin);
                this.position = parentPosition - this.Offset;
                return this.position;
            }
        }

        public override void SetLength(long value) {
            if (this.readOnly) {
                throw new NotSupportedException();
            }

            this.subStreamLength = value;
        }

        public void UpdateWindow(long offset, long length) {
            this.subStreamOffset = offset;
            this.subStreamLength = length;
        }

        protected override void Dispose(bool disposing) {
            if (!this.leaveParentOpen) {
                this.stream.Dispose();
            }

            base.Dispose(disposing);
        }

        private void EnsurePosition() {
            if (this.stream.Position != this.position + this.Offset) {
                this.stream.Position = this.position + this.Offset;
            }
        }
    }
}