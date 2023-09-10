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
            else
            {
                var binFile = new Bin(args[0]);
                File.WriteAllLines(args[0] + ".txt", binFile.GetStrings());
            }


        }




    }
}
