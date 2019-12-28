using System;
using System.IO;

namespace WindowsPackager.ARFileFormat
{

    public abstract class ArchiveFile : IDisposable
    {

        private readonly Stream stream;
        private readonly bool leaveOpen;
        private bool disposed;

        public ArchiveFile(Stream stream, bool leaveOpen) {
            if (stream == null) {
                throw new ArgumentNullException(nameof(stream));
            }

            this.stream = stream;
            this.leaveOpen = leaveOpen;
        }

        public SubStream EntryStream
        {
            get;
            protected set;
        }

        public string FileName
        {
            get;
            protected set;
        }

        public IArchiveHeader FileHeader
        {
            get;
            protected set;
        }

        protected Stream Stream
        {
            get
            {
                this.EnsureNotDisposed();
                return this.stream;
            }
        }

        public static int PaddingSize(int multiple, int value) {
            if (value % multiple == 0) {
                return 0;
            }
            else {
                return multiple - value % multiple;
            }
        }

        public Stream Open() {
            return this.EntryStream;
        }

        public void Dispose() {
            if (!this.leaveOpen) {
                this.Stream.Dispose();
            }

            this.disposed = true;
        }

        public abstract bool Read();

        public void Skip() {
            byte[] buffer = new byte[60 * 1024];

            while (this.EntryStream.Read(buffer, 0, buffer.Length) > 0) {
                // Keep reading until we're at the end of the stream.
            }
        }

        protected void EnsureNotDisposed() {
            if (this.disposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        protected void Align(int alignmentBase) {
            var currentIndex =
                (int)(this.EntryStream != null ? (this.EntryStream.Offset + this.EntryStream.Length) : this.Stream.Position);

            if (this.Stream.CanSeek) {
                this.Stream.Seek(currentIndex + PaddingSize(alignmentBase, currentIndex), SeekOrigin.Begin);
            }
            else {
                byte[] buffer = new byte[PaddingSize(alignmentBase, currentIndex)];
                this.Stream.Read(buffer, 0, buffer.Length);
            }
        }
    }
}