
using System.Drawing;

namespace Numberlink
{
    public class Logger
    {
        const int cellSize = 50;
        public static Bitmap GenerateGameBoardBitmap(Paper paper)
        {
            int width = (paper.Width - 2) * cellSize;
            int height = (paper.Height - 2) * cellSize;
            Bitmap bitmap = new Bitmap(width + 2, height + 2);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Black);

                // Draw grid lines
                Pen pen = new Pen(Color.Gray, 2);
                for (int x = 1; x <= paper.Width - 1; x++)
                {
                    g.DrawLine(pen, x * cellSize, 0, x * cellSize, height);
                }
                for (int y = 1; y <= paper.Height - 1; y++)
                {
                    g.DrawLine(pen, 0, y * cellSize, width, y * cellSize);
                }

                var table = Print.FillTable(paper);
                for (int y = 1; y < paper.Height - 1; y++)
                {
                    for (int x = 1; x < paper.Width - 1; x++)
                    {
                        int pos = y * paper.Width + x;
                        if (paper.isSource[pos] && table[pos] != Paper.EMPTY && table[pos] >= 'A')
                        {
                            DrawPoint(g, x - 1, y - 1, paper.colorLookup[table[pos]], cellSize, 30);
                        }
                    }
                }
            }
            return bitmap;
        }
        public static Bitmap DrawSolution(Paper paper)
        {
            Bitmap original = GenerateGameBoardBitmap(paper);

            using (Graphics g = Graphics.FromImage(original))
            {
                int w = paper.Width, h = paper.Height;
                var table = Print.FillTable(paper);
                for (int pos = 0; pos < w * h; pos++)
                {
                    foreach (int dir in Paper.DIRS)
                    {
                        int next = pos + paper.Vctr[dir];
                        if ((paper.Connections[pos] & dir) != 0 && table[next] == table[pos])
                        {
                            DrawLine(g, PosToLocal(paper, pos), PosToLocal(paper, next), paper.colorLookup[table[pos]], 10);
                        }
                    }
                }
            }
            return original;
        }
        private static (int, int) PosToLocal(Paper paper, int val)
        {
            (int, int) pos = (val % paper.Width - 1, val / paper.Height - 1);
            return pos;
        }
        private static void DrawLine(Graphics g, (int, int) pos1, (int, int) pos2, Color color, int size)
        {
            Pen pen = new Pen(color, size);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            int x1 = pos1.Item1 * cellSize + cellSize / 2;
            int y1 = pos1.Item2 * cellSize + cellSize / 2;
            int x2 = pos2.Item1 * cellSize + cellSize / 2;
            int y2 = pos2.Item2 * cellSize + cellSize / 2;
            g.DrawLine(pen, x1, y1, x2, y2);
        }
        private static void DrawPoint(Graphics g, int xPos, int yPos, Color color, int cellSize, int size)
        {
            int x = xPos * cellSize + cellSize / 2;
            int y = yPos * cellSize + cellSize / 2;
            int pointSize = size; // Adjust as needed
            g.FillEllipse(new SolidBrush(color), x - pointSize / 2, y - pointSize / 2, pointSize, pointSize);
        }
    }
}
