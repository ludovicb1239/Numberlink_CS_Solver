using System.Diagnostics;

namespace Numberlink
{
    class Program
    {
        static bool colorsFlag = false;
        static bool tubesFlag = true;
        static bool callsFlag = true;
        static bool profileFlag = true;

        static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith("--colors"))
                    colorsFlag = true;
                else if (arg.StartsWith("--tubes"))
                    tubesFlag = true;
                else if (arg.StartsWith("--calls"))
                    callsFlag = true;
                else if (arg.StartsWith("--profile"))
                    profileFlag = true;
            }

            // Normal run
            var reader = new StreamReader(Console.OpenStandardInput());
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                    break;

                line = line.Trim();
                if (string.IsNullOrEmpty(line) || line[0] == '#')
                    continue;

                var parts = line.Split(' ');
                bool bad = parts.Length != 2;
                int w = 0, h = 0;
                if (!bad)
                {
                    bad = !int.TryParse(parts[0], out w) || !int.TryParse(parts[1], out h);
                }
                if (bad)
                {
                    Console.Error.WriteLine($"Error: Expected 'width height' got '{line}'");
                    Environment.Exit(1);
                }

                // We use 0 0 as an end of puzzles mark
                if (w == 0 && h == 0)
                    break;

                var lines = new List<string>(w * h);
                for (int i = 0; i < h; i++)
                {
                    line = reader.ReadLine();
                    if (line == null)
                    {
                        Console.Error.WriteLine("Unexpected end of input");
                        Environment.Exit(1);
                    }
                    lines.Add(line.Trim());
                }

                // Done parsing stuff, time for the fun part
                Paper p = Parser.Parse(w, h, lines);

                Stopwatch profiler = new();
                if (profileFlag)
                {
                    profiler = new Stopwatch();
                    profiler.Start();
                }

                bool res = p.Solve();


                if (profileFlag)
                {
                    profiler.Stop();
                    double elapsedMicroseconds = (double)profiler.ElapsedTicks / Stopwatch.Frequency * 1_000_000;
                    Console.WriteLine($"Solve Time: {elapsedMicroseconds:F3} µs");
                }

                if (res)
                {
                    if (tubesFlag)
                        Print.PrintTubes(p, colorsFlag);
                    else
                        Print.PrintSimple(p, colorsFlag);
                }
                else
                {
                    Console.WriteLine("IMPOSSIBLE");
                }

                if (callsFlag)
                {
                    Console.WriteLine($"Called {p.Calls} times");
                }
                Console.WriteLine();
            }
        }
    }
}