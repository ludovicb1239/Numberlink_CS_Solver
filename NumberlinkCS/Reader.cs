using System.Drawing;
using System.Numerics;

namespace Numberlink
{
    public class Reader
    {
        public Vector2 topLeftCorner;
        public Vector2 bottomRightCorner;
        public int sizeX;
        public int sizeY;

        public Paper ScanGameboard()
        {
            Paper paper = new();
            paper.Width = sizeX;
            paper.Height = sizeY;

            int width = (int)(bottomRightCorner.X - topLeftCorner.X);
            int height = (int)(bottomRightCorner.Y - topLeftCorner.Y);

            var table = new List<char>(width * height);

            using (Bitmap bitmap = new Bitmap(width, height))
            {
                // Use the Graphics object to copy the pixel from the screen
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen((int)topLeftCorner.X, (int)topLeftCorner.Y, 0, 0, new Size(width, height));
                }


                float cellSizeX = width / sizeX;
                float cellSizeY = height / sizeY;
                List<Color> colors = new();
                for (short idy = 0; idy < sizeY; idy += 1)
                {
                    for (short idx = 0; idx < sizeX; idx += 1)
                    {
                        int x = (int)(idx * cellSizeX + cellSizeX / 2f);
                        int y = (int)(idy * cellSizeY + cellSizeY / 2f);

                        Color pixelColor = bitmap.GetPixel(x, y);

                        // Output the color of the pixel
                        //Console.WriteLine($"The color of the pixel at ({x}, {y}) is: {pixelColor}");

                        char val = Paper.EMPTY;

                        if (pixelColor.GetBrightness() > 0.1f)
                        {
                            if (colors.Contains(pixelColor))
                            {
                                val = (char)(colors.IndexOf(pixelColor) + 'A');
                            }
                            else
                            {
                                val = (char)(colors.Count() + 'A');
                                colors.Add(pixelColor);
                            }
                        }
                        table.Add(val);
                        //else
                        //Console.WriteLine($"Color is dark");
                    }
                }
                Paper p = Paper.NewPaper(sizeX, sizeY, table.ToArray());
                p.colorLookup = colors.ToArray();

                return p;
            }
        }
    }
}
