using System.ComponentModel;
using System.Drawing;

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

            var colors = new List<Color> { Color.Red, Color.Green, Color.Yellow, Color.Blue, Color.Olive, Color.Lime, Color.Cyan, Color.DarkBlue, Color.Turquoise, Color.Pink, Color.Orange, Color.Purple };

            Dictionary<char, Color> colorsLook = new();
            colorsLook.Add(Paper.EMPTY, Color.White);
            colorsLook.Add(Paper.GRASS, Color.Magenta);
            foreach (var line in lines)
            {
                foreach (var c in line)
                {
                    table.Add(c);
                    if (!colorsLook.ContainsKey(c))
                    {
                        Color newColor = colors[c % colors.Count];
                        colorsLook.Add(c, newColor);
                    }
                }
            }
            Paper paper = Paper.NewPaper(width, height, table.ToArray());
            paper.colorLookup = colorsLook;
            return paper;
        }
    }
}


