using System.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Filesystem.Ntfs;

[assembly : AssemblyTitle("LOCA")]
[assembly : AssemblyCopyright("rostok - https://github.com/rostok/")]
[assembly : AssemblyVersion("1.0.0.0")]
[assembly : AssemblyFileVersion("1.0.0.0")]

namespace NtfsReaderSample
{
    class Loca
    {
        static void Main(string[] args)
        {
            // Check if help is needed or no arguments are provided
            if (args.Length == 0 || Array.Exists(args, arg => arg.Equals("--help", StringComparison.OrdinalIgnoreCase)))
            {
                ShowHelp();
                DisplayIndexInfo();
                return;
            }

            // Check if indexing is requested
            if (Array.Exists(args, arg => arg.Equals("--index", StringComparison.OrdinalIgnoreCase)))
            {
                string drive = GetDriveFromArgs(args);
                IndexDrive(drive);
                return;
            }

            // Use arguments as filters
            FilterFiles(args);
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage: loca [drive] [options]");
            Console.WriteLine("Options:");
            Console.WriteLine("  --help                  Display this help message and show index information.");
            Console.WriteLine("  --index [drive]         Index all files and folders in the specified drive.");
            Console.WriteLine("                          If no drive is specified, the current drive is used.");
            Console.WriteLine("  [filters]               Use arguments as filters to display files containing all terms.");
            Console.WriteLine("                          Example: loca --index C | loca some important file");
            Console.WriteLine("this comes with MIT license from rostok - https://github.com/rostok/");
        }

        static void DisplayIndexInfo()
        {
            string locaDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".loca");
            
            if (!Directory.Exists(locaDirectory))
            {
                Console.WriteLine("No index information found.");
                return;
            }

            string[] indexFiles = Directory.GetFiles(locaDirectory, "allfiles-*.txt");

            Console.WriteLine("Indexed Drives:");
            foreach (string indexFile in indexFiles)
            {
                if (!indexFile.EndsWith(".txtbkp"))
                {
                    string fileName = Path.GetFileName(indexFile);
                    string driveLetter = fileName.Substring("allfiles-".Length, 1).ToUpperInvariant();
                    DateTime lastWriteTime = File.GetLastWriteTime(indexFile);

                    Console.WriteLine($"Drive: {driveLetter}, Last Indexed: {lastWriteTime}");
                }
            }
        }

        static string GetDriveFromArgs(string[] args)
        {
            // Default to current drive
            string drive = Directory.GetCurrentDirectory().Substring(0, 1);

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("--index", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length && args[i + 1].Length >= 1 && char.IsLetter(args[i + 1][0]))
                    {
                        drive = args[i + 1][0].ToString();
                    }
                    break;
                }
            }

            return drive.ToUpperInvariant();
        }

        static void IndexDrive(string drive)
        {
            Console.Error.WriteLine("Indexing drive " + drive);
            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".loca", $"allfiles-{drive}.txt");
            string backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".loca", $"allfiles-{drive}.txtbkp");

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            // Backup the existing index file
            if (File.Exists(outputPath))
            {
                // Delete any existing backup file before moving the current index file
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                File.Move(outputPath, backupPath);
                Console.WriteLine($"Existing index file moved to {backupPath}");
            }

            DriveInfo driveToAnalyze = new DriveInfo(drive);
            NtfsReader ntfsReader = new NtfsReader(driveToAnalyze, RetrieveMode.All);
            IEnumerable<INode> nodes = ntfsReader.GetNodes(driveToAnalyze.Name);

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                foreach (INode node in nodes)
                {
                    if (!node.FullName.StartsWith($"{drive}:\\$")) // Skip NTFS-specific folders
                    {
                        writer.WriteLine(node.FullName);
                    }
                }
            }

            Console.WriteLine($"Indexing completed. Results saved to {outputPath}");
        }

        static void FilterFiles(string[] filters)
        {
            string drive = Directory.GetCurrentDirectory().Substring(0, 1).ToUpperInvariant();
            string indexPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".loca", $"allfiles-{drive}.txt");

            if (!File.Exists(indexPath))
            {
                Console.Error.WriteLine("Index file not found. Please run the program with --index first.");
                return;
            }

            string[] filterTerms = Array.ConvertAll(filters, filter => filter.ToLowerInvariant());

            using (StreamReader reader = new StreamReader(indexPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string lowerLine = line.ToLowerInvariant();
                    bool matchesAllFilters = true;

                    foreach (string term in filterTerms)
                    {
                        if (!lowerLine.Contains(term))
                        {
                            matchesAllFilters = false;
                            break;
                        }
                    }

                    if (matchesAllFilters)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
        }
    }
}
