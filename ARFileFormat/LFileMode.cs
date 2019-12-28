using System;

namespace WindowsPackager.ARFileFormat
{
    // Unix-Style File perms, should be similar to sys/stat.h
    [Flags]
    public enum LFileMode : ushort
    {
        // none
        None = 0,
        // set userid on exec
        S_ISUID = 0x0800,
        // set gid
        S_ISGID = 0x0400,
        // sticky
        S_ISVTX = 0x0200,
        //                  <-- PERMS -->
        // user read
        S_IRUSR = 0x0100,
        // user write
        S_IWUSR = 0x0080,
        // user exec
        S_IXUSR = 0x0040,
        // group read
        S_IRGRP = 0x0020,
        // group write
        S_IWGRP = 0x0010,
        // group exec
        S_IXGRP = 0x0008,
        // other read
        S_IROTH = 0x0004,
        // other write
        S_IWOTH = 0x0002,
        // other exec
        S_IXOTH = 0x0001,
        //                  <-- AR_FILEMODE -->
        // FIFO
        S_IFIFO = 0x1000,
        // special char
        S_IFCHR = 0x2000,
        // is directory
        S_IFDIR = 0x4000,
        // block device
        S_IFBLK = 0x6000,
        // reg. file
        S_IFREG = 0x8000,
        // smbolic link
        S_IFLNK = 0xA000,
        // unix socket
        S_IFSOCK = 0xC000,
        //                  <-- MASKS -->
        // get all perms flag
        PermissionsMask = 0x0FFF,
        // get filetype flag
        FileTypeMask = 0xF000,
    }
}
