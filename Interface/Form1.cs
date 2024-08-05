using Numberlink;
using System.Numerics;

namespace Interface
{
    public partial class Form1 : Form
    {
        bool play = false;
        int sidesX = 9;
        int sidesY = 9;
        public Form1()
        {
            InitializeComponent();

            Thread thread = new(new ThreadStart(ContinuousScan));
            thread.Start();
        }

        private void SolveButton_Click(object sender, EventArgs e)
        {
            play = true;
        }
        void ContinuousScan()
        {
            while (true)
            {
                Thread.Sleep(100);
                ScanAndPlay();
            }
        }

        void ScanAndPlay()
        {
            Reader paper_reader = new();
            Player player = new();
            Vector2 topLeft = new(1080, 166);
            Vector2 boardSize = new(715, 715);

            paper_reader.topLeftCorner = topLeft;
            paper_reader.bottomRightCorner = topLeft + boardSize;
            paper_reader.sizeX = sidesX;
            paper_reader.sizeY = sidesY;
            player.topLeftCorner = topLeft;
            player.bottomRightCorner = topLeft + boardSize;
            player.sizeX = sidesX;
            player.sizeY = sidesY;


            // Move the cursor out of the way
            // player.MoveCursor((int)(topLeft.X + boardSize.X), (int)(topLeft.Y + boardSize.Y));

            Paper paper = paper_reader.ScanGameboard();

            Bitmap notSolvedImage = Logger.GenerateGameBoardBitmap(paper);
            InputImageBox.Invoke(new Action(() =>
            {
                InputImageBox.Image = notSolvedImage;
            }));

            // gameBoardBitmap = Logger.GenerateGameBoardBitmap(board);
            // pictureBox.Image = gameBoardBitmap;
            // gameBoardBitmap.Save("board.png");

            bool solved = Paper.Solve(paper);

            if (solved)
            {
                Print.PrintSimple(paper, false);

                Bitmap solvedImage = Logger.DrawSolution(paper);
                OutputImageBox.Invoke(new Action(() =>
                {
                    OutputImageBox.Image = solvedImage;
                }));

                if (play)
                {
                    player.Play(paper);
                    play = false;
                }
                //Thread.Sleep(2000);
                //player.ClickNextLevel();
            }
            else
            {
                OutputImageBox.Image = null;
            }
        }

        private void XCountInput_ValueChanged(object sender, EventArgs e)
        {
            sidesX = (int)XCountInput.Value;
        }

        private void YCountInput_ValueChanged(object sender, EventArgs e)
        {
            sidesY = (int)YCountInput.Value;
        }
    }
}
