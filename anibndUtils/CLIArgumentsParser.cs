using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anibndUtils
{
    internal class CLIArgumentsParser
    {
        public static void DisplayHelp()
        {
            Console.WriteLine("Usage: anibndUtils [ compare | filter | index ]");
            Console.WriteLine();
            Console.WriteLine("anibndUtils compare <file1.anibnd.dcx> <file2.anibnd.dcx>");
            Console.WriteLine("anibndUtils filter <file.anibnd.dcx> [ regex_filter ]");
            Console.WriteLine("anibndUtils index <file.anibnd.dcx>");
            Console.WriteLine();
            Console.WriteLine("Examples");
            Console.WriteLine("- Compare `c0000.anibnd.dcx` and `mod\\c0000.anibnd.dcx` files:");
            Console.WriteLine("anibndUtils compare c0000.anibnd.dcx mod\\c0000.anibnd.dcx");
            Console.WriteLine("- Get all events with jump_table_id=12 (kill character) in c0000.anibnd.dcx:");
            Console.WriteLine("anibndUtils filter c0000.anibnd.dcx 12");
            Console.WriteLine("- Get all events with jump_table_id=12 or jump_table_id=20 in c0000.anibnd.dcx:");
            Console.WriteLine("anibndUtils filter c0000.anibnd.dcx \"12|20\"");

            Environment.Exit(0);
        }

        public static void Parse(string[] args)
        {
            if (args.Length == 0) { DisplayHelp(); }

            if (args[0] == "index")
            {
                if (args.Length != 2) { DisplayHelp(); }
                AnibndUtils.AnibndUtils.GetIndexes(args[1]);
            }

            if (args[0] == "compare")
            {
                if (args.Length != 3) { DisplayHelp(); }
                AnibndUtils.AnibndUtils.CompareANIBND(args[1], args[2]);
            }

            if (args[0] == "filter")
            {
                if (args.Length == 2) { AnibndUtils.AnibndUtils.Filter(args[1]); }
                else if (args.Length == 3) { AnibndUtils.AnibndUtils.Filter(args[1], args[2]); }
                else { DisplayHelp(); }
            }
        }
    }
}
