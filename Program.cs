using Bayonetta3_bin_tool.Core;
using System;
using System.IO;

namespace Bayonetta3_bin_tool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Author: Amr Shaheen");

            Console.WriteLine("\nBayonetta 3 bin tool");
            Console.WriteLine($"\nUsage:");
            Console.WriteLine($"\tExport: {AppDomain.CurrentDomain.FriendlyName} <bin file path>");
            Console.WriteLine($"\tImport: {AppDomain.CurrentDomain.FriendlyName} <txt file path>");

            if (args.Length == 0)
            {
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            if (args[0].EndsWith("txt"))
            {
                var Filepath = Path.ChangeExtension(args[0], null);
                var binFile = new Bin(Filepath);
                binFile.UpdateStrings(File.ReadAllLines(args[0]));
                binFile.Save(Filepath.Insert(Filepath.LastIndexOf('.'), "_new"));
                return;
            }
            else if(File.Exists(args[0]))
            {
                var binFile = new Bin(args[0]);
                File.WriteAllLines(args[0] + ".txt", binFile.GetStrings());
            }
            else if (Directory.Exists(args[0]))
            {
                Console.WriteLine("Extracting: "+ args[0]);
                var path =Path.GetFullPath(args[0]);
                Directory.SetCurrentDirectory(path);
                if (File.Exists("AllStrings.txt"))
                {
                    File.Delete("AllStrings.txt");
                }

                foreach (var FilePath in Directory.GetFiles(path,"*.bin",SearchOption.AllDirectories))
                {
                    Console.WriteLine("\tExtracting: " + FilePath);

                    var binFile = new Bin(FilePath);

                    File.AppendAllText("AllStrings.txt", "[PATH]"+ FilePath.Substring(path.Length)+ "[PATH]\r\n");
                    File.AppendAllLines("AllStrings.txt", binFile.GetStrings());

                }

            }
        }




    }
}
