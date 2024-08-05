using System;
using System.Collections.Generic;

namespace Numberlink
{
    public class ParseError : Exception
    {
        public int Line { get; }
        public string Problem { get; }

        public ParseError(int line, string problem)
        {
            Line = line;
            Problem = problem;
        }

        public override string ToString()
        {
            return $"ParseError: '{Problem}' at line {Line}";
        }
    }
    public class Parser
    {
        public static Paper Parse(int width, int height, List<string> lines)
        {
            if (width * height == 0)
            {
                throw new ParseError(0, "width and height cannot be 0");
            }
            if (width != lines[0].Length || height != lines.Count)
            {
                throw new ParseError(1, "width and height must match puzzle size");
            }

            var table = new List<char>(width * height);
            foreach (var line in lines)
            {
                foreach (var c in line)
                {
                    table.Add(c);
                }
            }

            return Paper.NewPaper(width, height, table.ToArray());
        }
    }
}


