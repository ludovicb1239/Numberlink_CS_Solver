using System.Drawing;
using System.Numerics;

namespace Numberlink
{
    public class Reader
    {
        public int sizeX;
        public int sizeY;

        public bool ScanGameboard(Bitmap input, out Paper p)
        {
            int width = input.Width;
            int height = input.Height;

            var table = new List<char>(width * height);

            float cellSizeX = width / sizeX;
            float cellSizeY = height / sizeY;
            List<Color> colors = new();
            Dictionary<Color, int> colorAndCount = new();
            for (short idy = 0; idy < sizeY; idy += 1)
            {
                for (short idx = 0; idx < sizeX; idx += 1)
                {
                    int x = (int)(idx * cellSizeX + cellSizeX / 2f);
                    int y = (int)(idy * cellSizeY + cellSizeY / 2f);

                    Color pixelColor = input.GetPixel(x, y);

                    // Output the color of the pixel
                    //Console.WriteLine($"The color of the pixel at ({x}, {y}) is: {pixelColor}");

                    char val = Paper.EMPTY;

                    if (pixelColor.GetBrightness() > 0.1f)
                    {
                        if (colors.Contains(pixelColor))
                        {
                            val = (char)(colors.IndexOf(pixelColor) + 'A');
                            colorAndCount[pixelColor] += 1;
                        }
                        else
                        {
                            val = (char)(colors.Count() + 'A');
                            colors.Add(pixelColor);
                            colorAndCount.Add(pixelColor, 1);
                        }
                    }
                    table.Add(val);
                }
            }

            // Validate the board
            foreach(int count in colorAndCount.Values)
            {
                if (count != 2)
                {
                    p = null;
                    return false;
                }
            }

            p = Paper.NewPaper(sizeX, sizeY, table.ToArray());

            p.colorLookup = new()
            {
                { Paper.EMPTY, Color.White },
                { Paper.GRASS, Color.Red }
            };
            for (int i =  0; i < colors.Count; i++)
                p.colorLookup.Add((char)(i+'A'), colors[i]);

            return true;
        }
        public bool validateGameboard(Bitmap input)
        {
            int width = input.Width;
            int height = input.Height;
            float cellSizeX = width / sizeX;
            float cellSizeY = height / sizeY;

            for (short idy = 0; idy < sizeY; idy += 1)
            {
                for (short idx = 0; idx < sizeX; idx += 1)
                {
                    int x = (int)(idx * cellSizeX + cellSizeX / 2f);
                    int y = (int)(idy * cellSizeY + cellSizeY / 2f);

                    Color pixelColor = input.GetPixel(x, y);
                    // Checks if every box is colored
                    if (pixelColor.GetBrightness() < 0.1f)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
