using System.Numerics;
using WindowsInput; //InputSimulator

namespace Numberlink
{
    public class Player
    {
        public Vector2 topLeftCorner;
        public Vector2 bottomRightCorner;
        public int sizeX;
        public int sizeY;
        InputSimulator sim;
        public Player()
        {
            sim = new();
        }

        public static double MapToAbsolute(double value, double screenSize)
        {
            return value * 65535.0 / screenSize;
        }
        public void Play(Paper paper)
        {
            int delayms = 30;
            int w = paper.Width, h = paper.Height;
            char[] table = new char[w * h];
            paper.Table.CopyTo(table);
            List<char> doneChar = new();
            for (int pos1 = 0; pos1 < w * h; pos1++)
            {
                if (paper.source[pos1] && !doneChar.Contains(table[pos1]))
                {

                    MoveCursorPaper(paper, pos1);
                    Thread.Sleep(delayms);
                    ClickCursor();

                    Queue<int> queue = new();
                    queue.Enqueue(pos1);

                    char paint = table[pos1];
                    doneChar.Add(paint);

                    int prevDir = -1;
                    while (queue.Count != 0)
                    {
                        int pos = queue.Dequeue();

                        foreach (int dir in Paper.DIRS)
                        {
                            int next = pos + paper.Vctr[dir];
                            if ((paper.Con[pos] & dir) != 0 && table[next] == Paper.EMPTY)
                            {
                                table[next] = paint;
                                queue.Enqueue(next);


                                if (dir != prevDir)
                                {
                                    Thread.Sleep(delayms);
                                    MoveCursorPaper(paper, pos);
                                    prevDir = dir;
                                }
                            }
                            else if (table[next] == paint && next != pos1 && paper.source[next])
                            {
                                if (dir != prevDir)
                                {
                                    Thread.Sleep(delayms);
                                    MoveCursorPaper(paper, pos);
                                }
                                Thread.Sleep(delayms);
                                MoveCursorPaper(paper,next);
                                Thread.Sleep(delayms);
                                ClickCursor();
                                Thread.Sleep(delayms);
                            }
                        }
                    }

                }
            }
        }
        public void ClickNextLevel()
        {
            MoveCursor(1440, 505);
            Thread.Sleep(100);
            ClickCursor();
        }
        void MoveCursorPaper(Paper paper, int pos)
        {
            int width = (int)(bottomRightCorner.X - topLeftCorner.X);
            int height = (int)(bottomRightCorner.Y - topLeftCorner.Y);

            float cellSizeX = width / sizeX;
            float cellSizeY = height / sizeY;

            int posX = pos % (sizeX + 2) - 1; //To compensate for the grass
            int posY = pos / (sizeY + 2) - 1;

            int x = (int)(posX * cellSizeX + cellSizeX / 2 + topLeftCorner.X);
            int y = (int)(posY * cellSizeY + cellSizeY / 2 + topLeftCorner.Y);

            MoveCursor(x, y);
        }
        public void MoveCursor(int x, int y)
        {
            // Screen resolution
            double screenWidth = 1920;
            double screenHeight = 1080;

            // Convert to absolute mouse units
            double absoluteX = MapToAbsolute(x, screenWidth);
            double absoluteY = MapToAbsolute(y, screenHeight);

            // Move the mouse to the specified coordinates
            sim.Mouse.MoveMouseTo(absoluteX, absoluteY);
        }
        void ClickCursor()
        {
            sim.Mouse.LeftButtonClick();
        }
    }
}
