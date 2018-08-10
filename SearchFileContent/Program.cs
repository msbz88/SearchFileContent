using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SearchFileContent {
    class Program {
        static string GetFileText(string name) {
            string fileContents = String.Empty;
            if (File.Exists(name)) {
                fileContents = File.ReadAllText(name);
            }
            return fileContents;
        }

        static bool VerifyDirectory(string searchFolder) {
            if (Directory.Exists(searchFolder)) {
                return true;
            } else {
                Console.WriteLine("Directory does not exists or input path is incorrect");
                return false;
            }
        }

        static bool VerifyFile(string fileSearchTerms) {
            if (File.Exists(fileSearchTerms)) {
                if (new FileInfo(fileSearchTerms).Length > 0) {
                    return true;
                } else {
                    Console.WriteLine("The input file is empty, nothing to find");
                    return false;
                }
            } else {
                Console.WriteLine("File does not exists or input path is incorrect");
                return false;
            }
        }

        static List<string> ReadFile(string fileSearchTerms) {
            List<string> res = new List<string>();
            while (true) {
                try {
                    res = File.ReadAllLines(fileSearchTerms).ToList();
                    if (res.Count == 0) {
                        Console.WriteLine("Input file is empty");
                    } else { break; }

                } catch (Exception) {
                    Console.WriteLine("Wrong input file");
                }
                Console.WriteLine("Correct the file and then press Enter to retry");
                Console.ReadLine();
            }
            return res;
        }

        static void Main(string[] args) {
            string searchFolder = "";
            while (true) {
                Console.WriteLine("Search directory:");
                searchFolder = Console.ReadLine();
                if (VerifyDirectory(searchFolder)) { break; }
            }
            DirectoryInfo dir = new DirectoryInfo(searchFolder);
            string fileSearchTerms = "";
            while (true) {
                Console.WriteLine("Path to file with items to search:");
                fileSearchTerms = Console.ReadLine();
                if (VerifyFile(fileSearchTerms)) { break; }
            }
            List<string> searchTerms = ReadFile(fileSearchTerms);
            IEnumerable<FileInfo> fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);
            if (fileList.Count() == 0) {
                Console.WriteLine("The search directory is empty, nothing found");
            } else {
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine("Found in:");
                int count = 0;
                var queryMatchingFiles =
                from file in fileList
                where file.Name != Path.GetFileName(fileSearchTerms)
                let fileText = GetFileText(file.FullName)
                where searchTerms.Any(s => fileText.Contains(s))
                select file.FullName;

                foreach (string filename in queryMatchingFiles) {
                    Directory.CreateDirectory(searchFolder + @"\Selected");
                    string newFileLoc = searchFolder + @"\Selected\" + Path.GetFileName(filename);
                    if (!File.Exists(newFileLoc)) {
                        File.Copy(filename, newFileLoc);
                        count++;
                    }
                    Console.WriteLine(filename);
                }
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine("Result: " + count + " unique file(s) found");
                Console.WriteLine("Files copied to:");
                Console.WriteLine(searchFolder + @"\Selected");
                Console.ReadKey();
            }
        }
    }
}
