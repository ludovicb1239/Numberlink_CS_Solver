namespace Numberlink
{
    public class Print
    {
        const string RESET = "\x1b[0m";
        const string BOLD = "\x1b[1m";

        const string BLACK = "\x1b[30m";
        const string RED = "\x1b[31m";
        const string GREEN = "\x1b[32m";
        const string YELLOW = "\x1b[33m";
        const string BLUE = "\x1b[34m";
        const string MAGENTA = "\x1b[35m";
        const string CYAN = "\x1b[36m";
        const string WHITE = "\x1b[37m";

        static readonly char[] TUBE = { ' ', '╵', '╶', '└', '╷', '│', '┌', '├', '╴', '┘', '─', '┴', '┐', '┤', '┬', '┼' };

        public static void PrintSimple(Paper paper, bool color)
        {
            var colors = MakeColorTable(paper, !color);
            var table = FillTable(paper);
            Console.WriteLine($"{paper.Width - 2} {paper.Height - 2}");
            for (int y = 1; y < paper.Height - 1; y++)
            {
                for (int x = 1; x < paper.Width - 1; x++)
                {
                    int pos = y * paper.Width + x;
                    if (string.IsNullOrEmpty(colors[pos]))
                    {
                        Console.Write(table[pos]);
                    }
                    else
                    {
                        Console.Write($"{colors[pos]}{table[pos]}{RESET}");
                    }
                }
                Console.WriteLine();
            }
        }

        public static void PrintTubes(Paper paper, bool color)
        {
            var colors = MakeColorTable(paper, !color);
            for (int y = 1; y < paper.Height - 1; y++)
            {
                for (int x = 1; x < paper.Width - 1; x++)
                {
                    int pos = y * paper.Width + x;
                    char val = paper.Table[pos];
                    char c = val == Paper.EMPTY ? TUBE[paper.Connections[pos]] : val;
                    if (string.IsNullOrEmpty(colors[pos]))
                    {
                        Console.Write(c);
                    }
                    else
                    {
                        Console.Write($"{colors[pos]}{c}{RESET}");
                    }
                }
                Console.WriteLine();
            }
        }

        public static string[] MakeColorTable(Paper paper, bool empty)
        {
            var color = new string[paper.Width * paper.Height];
            if (!empty)
            {
                var table = FillTable(paper);

                int next = 0;
                var available = new List<string> { RED, GREEN, YELLOW, BLUE, MAGENTA, CYAN, BLACK, WHITE };
                available.AddRange(new List<string> { BOLD + RED, BOLD + GREEN, BOLD + YELLOW, BOLD + BLUE, BOLD + MAGENTA, BOLD + CYAN, BOLD + BLACK, BOLD + WHITE });
                var mapping = new Dictionary<char, string>();

                for (int y = 1; y < paper.Height - 1; y++)
                {
                    for (int x = 1; x < paper.Width - 1; x++)
                    {
                        char c = table[y * paper.Width + x];
                        if (!mapping.ContainsKey(c))
                        {
                            if (available.Count >= 1)
                            {
                                mapping[c] = available[next];
                                next = (next + 1) % available.Count;
                            }
                            else
                            {
                                mapping[c] = BLACK;
                            }
                        }
                        color[y * paper.Width + x] = mapping[c];
                    }
                }
            }
            return color;
        }

        public static char[] FillTable(Paper paper)
        {
            int w = paper.Width, h = paper.Height;
            char[] table = new char[w * h];
            paper.Table.CopyTo(table);
            for (int pos1 = 0; pos1 < w * h; pos1++)
            {
                if (paper.isSource[pos1])
                {
                    Queue<int> queue = new();
                    queue.Enqueue(pos1);
                    while (queue.Count != 0)
                    {
                        int pos = queue.Dequeue();
                        char paint = table[pos];
                        foreach (int dir in Paper.DIRS)
                        {
                            int next = pos + paper.Vctr[dir];
                            if ((paper.Connections[pos] & dir) != 0 && table[next] == Paper.EMPTY)
                            {
                                table[next] = paint;
                                queue.Enqueue(next);
                            }
                        }
                    }
                }
            }
            return table;
        }
    }
}