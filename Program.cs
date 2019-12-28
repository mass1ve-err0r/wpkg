using System;
using System.IO;
using System.Text;

namespace WindowsPackager
{

    public class Program
    {
        private static string LOCAL_DIR = Environment.CurrentDirectory;
        private static string[] ControlElements = {
            "Package: com.yourcompany.identifier",
            "Name: Name of the product",
            "Depends: ",
            "Architecture: iphoneos-arm",
            "Description: This is a sample short description",
            "Maintainer: Maintainer Name",
            "Author: Author Name",
            "Section: Section",
            "Version: 1.0"
        };
        private const string CREATE_DEBIAN_PACKAGE = "-b";
        private const string EXTRACT_DEBIAN_PACKAGE = "-x";
        private const string THEME_DEB = "--theme";
        private const string HELPTEXT = "-h";
        private const string ERRMSG_DIR_FAILURE = "E: Directory was not found! Aborting...";
        private const string ERRMSG_FILE_FAILURE = "E: Specified file does not exist! Aborting...";
        private const string ERRMSG_ARGC_FAILURE = "E: Mismatch in arguments! (perhaps missing one or one too much?) Aborting...";
        private const string ERRMSG_DEB_FAILURE = "E: File is not a Debian Binary! Aborting...";
        private const string ERRMSG_STRUCT_FAILURE = "E: Directory does NOT match a standard structure! (Perhaps missing control?) Aborting...";
        private const int EXIT_ARGS_MISMATCH = 100;
        private const int EXIT_DIR_ERROR = 200;
        private const int EXIT_DEBFILE_ERROR = 300;
        private const int EXIT_STRUCT_ERROR = 400;

        static void Main(string[] args) {
            // check because switch
            if (args.Length == 0) {
                InfoMessage();
                Environment.Exit(-1);
            }
            switch (args[0])
            {
                case CREATE_DEBIAN_PACKAGE:
                    if (args.Length == 2) {
                        if (Directory.Exists(args[1])) {
                            BuilderType(args[1], true);
                        }
                        else {
                            ExitWithMessage(ERRMSG_DIR_FAILURE, EXIT_DIR_ERROR);
                        }
                    } else {
                        BuilderType(null, false);
                    }
                    break;
                case EXTRACT_DEBIAN_PACKAGE:
                    if (args.Length == 3) {
                        // get properly formatted Path
                        string[] cmdargs = Environment.GetCommandLineArgs();
                        // check if file exists & create extraction stream
                        if (File.Exists(cmdargs[2]) && Directory.Exists(cmdargs[3])) {
                            ExtractorType(cmdargs[2], null, cmdargs[3]);
                        }
                        else {
                            ExitWithMessage(ERRMSG_ARGC_FAILURE, EXIT_ARGS_MISMATCH);
                        }
                    } else if (args.Length == 2) {
                        // check if we have a path or direct filename => file cannot contain the '\' char
                        if (args[1].Contains("\\")) {
                            if (File.Exists(args[1])) {
                                ExtractorType(args[1], null, Path.GetDirectoryName(args[1]));
                            } else {
                                ExitWithMessage(ERRMSG_ARGC_FAILURE, EXIT_ARGS_MISMATCH);
                            }
                        } else {
                            if (File.Exists(LOCAL_DIR + "\\" + args[1])) {
                                ExtractorType(LOCAL_DIR + "\\" + args[1], args[1], null);
                            }
                            else {
                                ExitWithMessage(ERRMSG_FILE_FAILURE, EXIT_DEBFILE_ERROR);
                            }
                        }
                    }
                    break;
                case THEME_DEB:
                    if (args.Length != 2) {
                        ExitWithMessage(ERRMSG_ARGC_FAILURE, EXIT_ARGS_MISMATCH);
                    }
                    // create base theme dir
                    string target = LOCAL_DIR + "\\Library\\Themes\\" + args[1] + ".theme";
                    Directory.CreateDirectory(target);
                    // create the necessary subdirs
                    Directory.CreateDirectory(target + "\\IconBundles");
                    Directory.CreateDirectory(target + "\\Bundles\\com.apple.springboard");
                    GenerateControlFile(LOCAL_DIR);
                    break;
                case HELPTEXT:
                    InfoMessage();
                    break;
                default:
                    InfoMessage();
                    break;
            }
        }

        private static void BuilderType(string WorkDir, bool IsSpecified) {
            string dir = (IsSpecified) ? WorkDir : LOCAL_DIR;
            VerifyStructure(dir);
            Builder.BuildControlTarball(dir);
            Builder.BuildDataTarball(dir);
            Builder.BuildPackage(dir);
        }

        private static void ExtractorType(string PassedFilePath, string FileName, string TargetDirectory) {
            VerifyFile(PassedFilePath);
            Extractor.DebName = Path.GetFileNameWithoutExtension(PassedFilePath);
            if (String.IsNullOrEmpty(TargetDirectory)) {
                Stream DebFileStream = Builder.CreateStream(FileName);
                Extractor.ExtractEverything(DebFileStream, LOCAL_DIR);
            } else {
                Stream DebFileStream = Builder.CreateStream(PassedFilePath, 3);
                Extractor.ExtractEverything(DebFileStream, TargetDirectory);
            }
        }

        private static void VerifyFile(string PathToFile) {
            if (Extractor.IsDebianBinary(PathToFile) == false) {
                ExitWithMessage(ERRMSG_DEB_FAILURE, EXIT_DEBFILE_ERROR);
            }
        }

        public static void VerifyStructure(string directory) {
            int passed = 0;
            // check if we AT LEAST have 1 dir
            DirectoryInfo[] subdirs = new DirectoryInfo(directory).GetDirectories();
            if (subdirs.Length > 0) {
                passed++;
            }
            // check if we have a control file
            if (File.Exists(directory + "\\control")) {
                passed++;
            }
            // check if our struct matches
            if (passed != 2) {
                ExitWithMessage(ERRMSG_STRUCT_FAILURE, EXIT_STRUCT_ERROR);
            }
        }

        private static void GenerateControlFile(string WorkingDir) {
            File.WriteAllLines(WorkingDir + "\\control", ControlElements, Encoding.ASCII);
        }

        public static void ExitWithMessage(string Message, int ExitCode) {
            Console.WriteLine(Message);
            Environment.Exit(ExitCode);
        }

        private static void InfoMessage() {
            Console.WriteLine("Windows Packager (wpkg) v1.0 Guide");
            ColorizedMessage("Building:\n" +
                "wpkg -b            - Build .deb inside the local directory\n" +
                "wpkg -b <Path>     - Build .deb in the given path\n",
                ConsoleColor.DarkCyan);
            ColorizedMessage("Extraction:\n" +
                "wpkg -x <PathToDeb> <DestFolder>   - Extract .deb to given path\n" +
                "wpkg -x <PathToDeb>                - Extract .deb inside the original folder\n" +
                "wpkg -x <DebfileName>              - Extract a .deb inside the folder you're in*\n" +
                " *: only works if you're in the same folder as the .deb!\n",
                ConsoleColor.DarkGreen);
            ColorizedMessage("Extras:\n" +
                "wpkg -h        - Show this helptext\n" +
                "wpkg --theme   - Create a base for an iOS Theme in the directory you are currently\n\n",
                ConsoleColor.DarkMagenta);
            ColorizedMessage("If you stumble upon an error, please send an email at\n" +
                "support@saadat.dev\n",
                ConsoleColor.DarkRed);
        }

        private static void ColorizedMessage(string Message, ConsoleColor cColor) {
            Console.ForegroundColor = cColor;
            Console.WriteLine(Message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        // <-- FIN -->
    }
}
