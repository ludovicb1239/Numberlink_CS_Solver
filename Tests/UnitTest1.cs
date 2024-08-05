
using Numberlink;

namespace Tests
{
    [TestFixture]
    public class PaperTests
    {
        private static readonly (int width, int height, int n, int outValue)[] paperTests =
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

                int al = Tests.Choose2(width * height);
                int bl = Tests.Choose2(width * height - 2);
                int count = 0;
                var asArray = new int[] { 0, 1 };
                for (int i = 0; i < al; i++)
                {
                    var bsArray = new int[] { 0, 1 };
                    for (int j = 0; j < bl; j++)
                    {
                        var (a1, a2) = (asArray[0], asArray[1]);
                        var (b1, b2) = (bsArray[0], bsArray[1]);

                        if (a1 <= b1) b1++;
                        if (a2 <= b1) b1++;
                        if (a1 <= b2) b2++;
                        if (a2 <= b2) b2++;

                        Paper p = Tests.Create(new[] { new[] { a1, a2 }, new[] { b1, b2 } }, width, height);
                        bool result = Paper.Solve(p);
                        if (result) count++;

                        if (j + 1 < bl)
                        {
                            Tests.NextCombination(bsArray, width * height - 2);
                        }
                    }
                    if (i + 1 < al)
                    {
                        Tests.NextCombination(asArray, width * height);
                    }
                }

                Assert.AreEqual(expected, count, $"Expected {expected}, got {count} for width={width}, height={height}");
            }
        }
    }
}
