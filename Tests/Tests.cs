using Numberlink;

namespace Tests
{

    [TestFixture]
    public class Tests
    {
        private static readonly (int width, int height, int n, int expected)[] paperTests =
        {
            (1, 4, 2, 2),
            (1, 5, 2, 4),
            (2, 1, 2, 0),
            (2, 2, 2, 4),
            (2, 3, 2, 14),
            (2, 4, 2, 18),
            (2, 5, 2, 18),
            (3, 3, 2, 24),
            (3, 4, 2, 32),
            (3, 5, 2, 36),
            (4, 4, 2, 24),
            (4, 5, 2, 44),
            (4, 6, 2, 44),
            (5, 5, 2, 48),
            // (6, 6, 2, 72),
            // (7, 7, 2, 96),
            // (8, 8, 2, 96),
            // (9, 9, 2, 144),
            // (10, 10, 2, 240),
        };

        private static int Calls = 0; // Track the number of calls for validation

        [Test]
        public void TestSolve()
        {
            foreach (var (width, height, n, expected) in paperTests)
            {
                if (n != 2)
                {
                    Assert.Fail("Currently only n=2 is supported");
                    continue;
                }

                int al = Choose2(width * height);
                int bl = Choose2(width * height - 2);
                int count = 0;
                var asArray = new int[] { 0, 1 };
                for (int i = 0; i < al; i++)
                {
                    var bsArray = new int[] { 0, 1 };
                    for (int j = 0; j < bl; j++)
                    {
                        int a1 = asArray[0], a2 = asArray[1];
                        int b1 = bsArray[0], b2 = bsArray[1];

                        if (a1 <= b1) b1++;
                        if (a2 <= b1) b1++;
                        if (a1 <= b2) b2++;
                        if (a2 <= b2) b2++;

                        Paper p = Create(new[] { new[] { a1, a2 }, new[] { b1, b2 } }, width, height);
                        bool result = p.Solve();
                        if (result)
                        {
                            Print.PrintSimple(p, false);
                            Console.WriteLine(Calls);
                            count++;
                        }
                        Calls = 0;

                        if (j + 1 < bl)
                        {
                            NextCombination(bsArray, width * height - 2);
                        }
                    }
                    if (i + 1 < al)
                    {
                        NextCombination(asArray, width * height);
                    }
                }
                Assert.AreEqual(expected, count, $"Expected {expected}, got {count} for width={width}, height={height}");
            }
        }

        // Helper methods
        public static int Choose2(int n) => n * (n - 1) / 2;
        public static int Imax(int a, int b) => Math.Max(a, b);

        public static void NextCombination(int[] array, int n)
        {
            int r = array.Length;
            int i = r - 1;
            while (array[i] == n - r + i)
            {
                i--;
            }
            array[i]++;
            for (int j = i + 1; j < r; j++)
            {
                array[j] = array[j - 1] + 1;
            }
        }

        public static Paper Create(int[][] sources, int w, int h)
        {
            var table = new char[w * h];
            for (int i = 0; i < w * h; i++)
            {
                table[i] = '.';
            }
            for (int i = 0; i < sources.Length; i++)
            {
                var pair = sources[i];
                table[pair[0]] = (char)('A' + i); // Replace with actual SIGMA[i] if needed
                table[pair[1]] = (char)('A' + i); // Replace with actual SIGMA[i] if needed
            }
            return new Paper(w, h, table);
        }
    }
}