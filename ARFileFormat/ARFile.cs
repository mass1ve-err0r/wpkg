using System;
using System.Text;
using System.IO;

namespace WindowsPackager.ARFileFormat
{

    public class ARFile : ArchiveFile
    {

        private const string Magic = "!<arch>\n";
        private bool magicRead;
        private ARHeader entryHeader;

        public ARFile(Stream stream, bool leaveOpen)
            : base(stream, leaveOpen) {
        }

        public override bool Read() {
            this.EnsureMagicRead();

            if (this.EntryStream != null) {
                this.EntryStream.Dispose();
            }

            this.Align(2);

            if (this.Stream.Position == this.Stream.Length) {
                return false;
            }

            this.entryHeader = this.Stream.ReadStruct<ARHeader>();
            this.FileHeader = this.entryHeader;
            this.FileName = this.entryHeader.FileName;

            if (this.entryHeader.EndChar != "`\n") {
                throw new InvalidDataException("The magic for the file entry is invalid");
            }

            this.EntryStream = new SubStream(this.Stream, this.Stream.Position, this.entryHeader.FileSize, leaveParentOpen: true);

            return true;
        }

        protected void EnsureMagicRead() {
            if (!this.magicRead) {
                byte[] buffer = new byte[Magic.Length];
                this.Stream.Read(buffer, 0, buffer.Length);
                var magic = Encoding.ASCII.GetString(buffer);

                if (!string.Equals(magic, Magic, StringComparison.Ordinal)) {
                    throw new InvalidDataException("The .ar file did not start with the expected magic");
                }

                this.magicRead = true;
            }
        }
    }
}
