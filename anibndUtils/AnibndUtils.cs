using ObjectsComparer;
using SoulsFormats;
using System.Text.RegularExpressions;
using static SoulsFormats.DRB;

namespace AnibndUtils
{
    internal partial class AnibndUtils
    {
        const int EVENT_JUMP_TABLE = 0;

        public static void GetIndexes(string file)
        {
            BND4 bnd = BND4.Read(file);

            Console.WriteLine("# index anim_id anim_file_name");

            foreach (var bndFile in bnd.Files)
            {
                TAE tae;
                try
                {
                    tae = TAE.Read(bndFile.Bytes);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("[ERROR] Unable to read " + bndFile.Name);
                    continue;
                }

                Console.WriteLine("# Animations in " + bndFile.Name);
                for (int j = 0; j < tae.Animations.Count; j++)
                {
                    Console.WriteLine($"{j} {tae.Animations[j].ID} {tae.Animations[j].AnimFileName}");
                }
            }
        }

        public static void CompareANIBND(string file1, string file2)
        {
            BND4 bnd1 = BND4.Read(file1);
            BND4 bnd2 = BND4.Read(file2);

            Console.WriteLine("Files count in:");
            Console.WriteLine($"- {file1}: {bnd1.Files.Count}");
            Console.WriteLine($"- {file2}: {bnd2.Files.Count}");

            int max;
            if (bnd1.Files.Count > bnd2.Files.Count)
            {
                max = bnd2.Files.Count;
            }
            else
            {
                max = bnd1.Files.Count;
            }

            for (int i = 0; i < max; i++)
            {
                TAE tae1;
                TAE tae2;

                try
                {
                    tae1 = TAE.Read(bnd1.Files[i].Bytes);
                    tae2 = TAE.Read(bnd2.Files[i].Bytes);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("[ERROR] Unable to read " + bnd1.Files[i].Name);
                    continue;
                }

                var comparer = new ObjectsComparer.Comparer<TAE>();

                // compare objects
                var isEqual = comparer.Compare(tae1, tae2, out IEnumerable<Difference> differences);

                // print results
                if (isEqual)
                {
                    Console.Write("[NO DIFF] ");
                    Console.WriteLine($"ID={bnd1.Files[i].ID} NAME={bnd1.Files[i].Name}");
                }
                else
                {
                    Console.Write("[DIFF FOUND] ");
                    Console.WriteLine($"ID={bnd1.Files[i].ID} NAME={bnd1.Files[i].Name}");

                    var modifiedEvents = new Dictionary<long, string>();

                    foreach (Difference difference in differences)
                    {
                        Match m = RegexGetAnimationID().Match(difference.MemberPath);
                        if (m.Success)
                        {
                            int animIndex = int.Parse(m.Value);
                            modifiedEvents.TryAdd(tae1.Animations[animIndex].ID, tae1.Animations[animIndex].AnimFileName);
                            Console.WriteLine(
                                $"anim_id=\"{tae1.Animations[animIndex].ID}\" " +
                                $"anim_name=\"{tae1.Animations[animIndex].AnimFileName}\" " +
                                $"member_path=\"{difference.MemberPath}\" " +
                                $"value1=\"{difference.Value1}\" " +
                                $"value2=\"{difference.Value2}\"");
                        }
                        else
                        {
                            Console.WriteLine(
                                $"member_path=\"{difference.MemberPath}\" " +
                                $"value1=\"{difference.Value1}\" " +
                                $"value2=\"{difference.Value2}\"");
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine($"Number of modified events in " +
                        $"ID={bnd1.Files[i].ID} " +
                        $"NAME={bnd1.Files[i].Name}: " +
                        $"{modifiedEvents.Count}");

                    Console.WriteLine("Modified events:");
                    foreach (var e in modifiedEvents)
                    {
                        Console.WriteLine($"ID={e.Key} NAME={e.Value}");
                    }
                    Console.WriteLine();
                }
            }
        }

        public static void Filter(string file, string pattern = "")
        {
            BND4 bnd = BND4.Read(file);

            foreach (var bndFile in bnd.Files)
            {
                TAE tae;
                try
                {
                    tae = TAE.Read(bndFile.Bytes);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("[ERROR] Unable to read " + bndFile.Name);
                    continue;
                }

                string msg = "";
                string result = "";
                string beginMsg = $"# {bndFile.Name}";
                foreach (var anim in tae.Animations)
                {
                    foreach (var e in anim.Events)
                    {
                        if (e.Type == EVENT_JUMP_TABLE)
                        {
                            var args = e.GetParameterBytes(false);
                            int jumpTableID = BitConverter.ToInt32(args, 0);
                            // filter the results with the regex pattern
                            if (pattern == "" || Regex.IsMatch(jumpTableID.ToString(), "^(" + pattern + ")$"))
                            {
                                result +=
                                    $"anim_id=\"{anim.ID}\" " +
                                    $"anim_file_name=\"{anim.AnimFileName}\" " +
                                    $"jump_table_id=\"{jumpTableID}\" " +
                                    $"start_time=\"{e.StartTime}\" " +
                                    $"end_time=\"{e.EndTime}\"" + Environment.NewLine;
                            }
                        }
                    }
                    if (result != "")
                    {
                        msg = beginMsg + Environment.NewLine + result;
                    }
                }
                if (msg != "")
                {
                    Console.WriteLine(msg);
                }
            }
        }

        [GeneratedRegex("(?<=^Animations\\[)(\\d+)(?=\\])")]
        private static partial Regex RegexGetAnimationID();
    }
}