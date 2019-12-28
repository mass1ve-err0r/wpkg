using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPackager.ARFileFormat
{

    public class ArchiveEntry
    {

        public string TargetPath
        { get; set; }

        public string Owner
        { get; set; }

        public string Group
        { get; set; }

        public LFileMode Mode
        { get; set; }

        public string SourceFilename
        { get; set; }

        public uint FileSize
        { get; set; }

        public DateTimeOffset Modified
        { get; set; }

        public byte[] Sha256
        { get; set; }

        public byte[] Md5Hash
        { get; set; }

        public ArchiveEntryType Type
        { get; set; }

        public string LinkTo
        { get; set; }

        public uint Inode
        { get; set; }

        public bool IsAscii
        { get; set; }

        public bool RemoveOnUninstall
        { get; set; }

        public string TargetPathWithFinalSlash
        {
            get
            {
                if (this.Mode.HasFlag(LFileMode.S_IFDIR) && !this.TargetPath.EndsWith("/")) {
                    return this.TargetPath + "/";
                }
                else {
                    return this.TargetPath;
                }
            }
        }

        public string TargetPathWithoutFinalSlash
        {
            get
            {
                return this.TargetPath?.TrimEnd('/');
            }
        }

        public override string ToString() {
            return this.TargetPath;
        }
    }
}