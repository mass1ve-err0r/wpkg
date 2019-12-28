using System;
using System.IO;
using System.Text;
using WindowsPackager.ARFileFormat;
using ICSharpCode.SharpZipLib.Tar;

namespace WindowsPackager
{

    class Extractor
    {
        public static string DebName;
        private static string OutPath;
        private static string LOCAL_PATH = Environment.CurrentDirectory;

        public static void ExtractEverything(Stream DebianPackageStream, string WorkDir) {
            OutPath = (String.IsNullOrEmpty(WorkDir)) ? LOCAL_PATH : WorkDir;

            ARFile DebianPackage = new ARFile(DebianPackageStream, leaveOpen: true);
            while (DebianPackage.Read()) {
                if (DebianPackage.FileName == "debian-binary") {
                    ExtractContent(DebianPackage, 1);
                }
                else if (DebianPackage.FileName == "control.tar") {
                    ExtractContent(DebianPackage, 2);
                }
                else if (DebianPackage.FileName == "data.tar") {
                    ExtractContent(DebianPackage, 3);
                }
            }
        }

        public static bool IsDebianBinary(string FilePath) {
            //(String.IsNullOrEmpty(FilePath))
            string MagicMatch = "213C617263683E0A"; // !<arch>\n
            var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            BinaryReader bs = new BinaryReader(fs, new ASCIIEncoding());
            byte[] buffer = bs.ReadBytes(8);
            string MagicString = BitConverter.ToString(buffer).Replace("-", "");
            bs.Close();
            fs.Close();
            return (MagicString.Equals(MagicMatch)) ? true : false;
        }

        private static void ExtractContent(ARFile DebFile, int variant) {
            string fn = "";
            string fdir = "";
            if (variant == 1) /*debian-binary*/ {
                fn = "debian-binary";
            } else if (variant == 2) /*control.tar*/ {
                fn = "control.tar";
                fdir = OutPath + "\\" + DebName + "_control.tar_CONTENT";
            } else if (variant == 3) /*data.tar*/ {
                fn = "data.tar";
                fdir = OutPath + "\\" + DebName + "_data.tar_CONTENT";
            }

            // extractor
            Stream Lstream = DebFile.Open();
            StreamReader reader = new StreamReader(Lstream);
            var content = reader.ReadToEnd();
            StreamWriter sw = new StreamWriter(Path.Combine(OutPath, fn));
            sw.Write(content);
            sw.Close();
            reader.Close();

            // deeper extraction for the tarballs
            if(variant == 2 ||variant == 3) {
                Stream inStream = File.OpenRead(Path.Combine(OutPath, fn));
                TarArchive InnerTarball = TarArchive.CreateInputTarArchive(inStream);
                Directory.CreateDirectory(fdir);
                InnerTarball.ExtractContents(fdir);
                InnerTarball.Close();
                inStream.Close();

                File.Delete(OutPath + "\\control.tar");
                File.Delete(OutPath + "\\data.tar");
                File.Delete(OutPath + "\\debian-binary");
            }
        }
    }
}
