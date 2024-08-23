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
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
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

            string folderPath = @"puzzles"; // Replace with your folder path
            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                using (var reader = new StreamReader(file))
                {
                    Console.WriteLine($"Working on {file}");
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
                            Console.Error.WriteLine($"Error: Expected 'width height' got '{line}' in file {file}");
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
                                Console.Error.WriteLine($"Unexpected end of input in file {file}");
                                Environment.Exit(1);
                            }
                            lines.Add(line.Trim());
                        }
                        double totalus = 0;

                        for(int i = 0; i < 100; i++)
                        {
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
                                totalus += elapsedMicroseconds;
                            }

                            if (res)
                            {//
                             //if (tubesFlag)
                             //    Print.PrintTubes(p, colorsFlag);
                             //else
                             //    Print.PrintSimple(p, colorsFlag);
                            }
                            else
                            {
                                Console.WriteLine("IMPOSSIBLE");
                            }
                        }
                        Console.WriteLine($"Average Solve Time for 100 solves: {totalus/100f:F3} µs\n");


                        //if (callsFlag)
                        //{
                        //    Console.WriteLine($"Called {p.Calls} times");
                        //}
                        //Console.WriteLine();
                    }
                }
            }
            Thread.Sleep(1000000000);
        }
    }
}