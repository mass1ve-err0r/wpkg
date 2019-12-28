using System;
using System.Text;
using System.IO;

namespace WindowsPackager.ARFileFormat
{
    public static class ARFileCreator
    {
        public static void WriteMagic(Stream output) {
            var wr = new StreamWriter(output);
            wr.Write("!<arch>\n");
            wr.Flush();
        }

        public static void WriteEntry(Stream output, string name, LFileMode mode, string data)
            => WriteEntry(output, name, mode, new MemoryStream(Encoding.UTF8.GetBytes(data)));

        public static void WriteEntry(Stream output, string name, LFileMode mode, Stream data) {
            var hdr = new ARHeader {
                EndChar = "`\n",
                FileMode = mode,
                FileName = name,
                FileSize = (uint)data.Length,
                GroupId = 0,
                OwnerId = 0,
                LastModified = DateTimeOffset.UtcNow
            };
            WriteEntry(output, hdr, data);
        }

        public static void WriteEntry(Stream output, ARHeader header, Stream data) {
            output.WriteStruct(header);
            data.CopyTo(output);
            if (output.Position % 2 != 0) {
                output.WriteByte(0);
            }
        }
    }
}
