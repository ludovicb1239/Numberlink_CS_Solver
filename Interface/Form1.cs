using Numberlink;
using System.Numerics;

namespace Interface
{
    public partial class Form1 : Form
    {
        bool play = false;
        public Form1()
        {
            InitializeComponent();

            Thread thread = new(new ThreadStart(ContinuousScan));
            thread.Start();
        }

        private void SolveButton_Click(object sender, EventArgs e)
        {
            play = !play;
            SolveButton.Text = play ? "Stop" : "Play";
        }
        void ContinuousScan()
        {
            while (true)
            {

                Bitmap bitmap = TakeScreenshotRegion();
                bool didFind = false;
                for(int sides = 5; sides < 16;  sides++)
                {
                    if (ScanAndPlay(sides, sides, bitmap))
                    {
                        didFind = true;
                        break;
                    }
                }
                if (!didFind)
                {
                    OutputImageBox.Invoke(new Action(() =>
                    {
                        InputImageBox.Image = bitmap;
                        OutputImageBox.Image = null;
                        outLabel.Text = "Could not find puzzle in frame";
                    }));
                }
                Thread.Sleep(100);
            }
        }

        bool ScanAndPlay(int x, int y, Bitmap bitmap)
        {
            Reader paper_reader = new();
            Player player = new();
            Vector2 topLeft = new(1080, 166);
            Vector2 boardSize = new(715, 715);

            paper_reader.sizeX = x;
            paper_reader.sizeY = y;
            player.topLeftCorner = topLeft;
            player.bottomRightCorner = topLeft + boardSize;
            player.sizeX = x;
            player.sizeY = y;

            if (!paper_reader.ScanGameboard(bitmap, out Paper paper))
                return false;

            Bitmap notSolvedImage = Logger.GenerateGameBoardBitmap(paper);

            bool solved = paper.Solve();

            if (solved)
            {

                Print.PrintSimple(paper, false);

                Bitmap solvedImage = Logger.DrawSolution(paper);
                OutputImageBox.Invoke(new Action(() =>
                {
                    InputImageBox.Image = notSolvedImage;
                    OutputImageBox.Image = solvedImage;
                    outLabel.Text = "Solved puzzle";
                }));

                if (play)
                {
                    player.Play(paper, false);
                    //play = true;
                    Bitmap result = TakeScreenshotRegion();
                    if (paper_reader.validateGameboard(result))
                    {
                        player.ClickCursor(); //Release the last color because the player didnt do it
                        Thread.Sleep(120);
                        player.ClickNextLevel();
                        Thread.Sleep(120);
                    }
                    else
                    {
                        OutputImageBox.Invoke(new Action(() =>
                        {
                            outLabel.Text = "Failed to play puzzle";
                            play = false;
                            SolveButton.Text = play ? "Stop" : "Play";
                        }));
                        player.ClickCursor();
                        Thread.Sleep(2000);
                    }
                }
            }
            return solved;
        }

        Bitmap TakeScreenshotRegion()
        {
            Vector2 topLeft = new(1080, 166);
            Vector2 boardSize = new(715, 715);
            int width = (int)((topLeft + boardSize).X - topLeft.X);
            int height = (int)((topLeft + boardSize).Y - topLeft.Y);
            Bitmap bitmap = new Bitmap(width, height);
            // Use the Graphics object to copy the pixel from the screen
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen((int)topLeft.X, (int)topLeft.Y, 0, 0, new Size(width, height));
            }
            return bitmap;
        }
    }
}
