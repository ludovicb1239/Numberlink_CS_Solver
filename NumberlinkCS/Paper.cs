using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Intrinsics.X86;

namespace Numberlink
{
    public class Paper
    {
        static readonly public char GRASS = '#';
        static readonly public char EMPTY = '.';

        const int N = 0b0001;
        const int E = 0b0010;
        const int S = 0b0100;
        const int W = 0b1000;

        static readonly public int[] DIRS = { N, E, S, W };
        static readonly public int[] MIR = { 0, S, W, 0, N, 0, 0, 0, E, 0, 0, 0, 0, 0, 0, 0};
        static readonly public bool[] DIAG = new bool[16];

        public int Width { get; set; }
        public int Height { get; set; }

        public int[] Vctr = new int[16];
        public int[] Crnr = new int[16];

        public List<char> Table = new List<char>();
        public List<int> Con = new List<int>();

        public bool[] source;
        public int[] end;
        public bool[] canSE;
        public bool[] canSW;

        public int[] next;

        public Color[] colorLookup;

        public Paper()
        {
            DIAG[N | E] = true;
            DIAG[N | W] = true;
            DIAG[S | E] = true;
            DIAG[S | W] = true;
        }

        public static Paper NewPaper(int width, int height, char[] table)
        {
            var paper = new Paper();

            // Pad the given table with #, to make boundary checks easier
            int w = width + 2, h = height + 2;
            paper.Width = w;
            paper.Height = h;

            for (int i = 0; i < w; i++)
            {
                paper.Table.Add(GRASS);
            }
            for (int y = 1; y < h - 1; y++)
            {
                paper.Table.Add(GRASS);
                for (int x = 1; x < w - 1; x++)
                {
                    paper.Table.Add(table[(y - 1) * (w - 2) + (x - 1)]);
                }
                paper.Table.Add(GRASS);
            }
            for (int i = 0; i < w; i++)
            {
                paper.Table.Add(GRASS);
            }

            InitTables(paper);

            return paper;
        }

        public static bool Solve(Paper paper)
        {
            return ChooseConnection(paper, paper.Crnr[N | W]);
        }

        public static int Calls = 0;
        static bool ChooseConnection(Paper paper, int pos)
        {
            Calls++;

            // Final
            if (pos == 0)
            {
                return Validate(paper);
            }

            int w = paper.Width;
            if (paper.source[pos])
            {
                switch (paper.Con[pos])
                {
                    // If the source is not yet connected
                    case 0:
                        // We can't connect E if we have a NE corner
                        if (paper.Con[pos - w + 1] != (S | W))
                        {
                            if (TryConnection(paper, pos, E))
                            {
                                return true;
                            }
                        }
                        // South connections can create a forced SE position
                        if (CheckImplicitSE(paper, pos))
                        {
                            if (TryConnection(paper, pos, S))
                            {
                                return true;
                            }
                        }
                        break;
                    // If the source is already connected
                    case N:
                    case W:
                        return ChooseConnection(paper, paper.next[pos]);
                }
            }
            else
            {
                switch (paper.Con[pos])
                {
                    // SE
                    case 0:
                        // Should we check for implied N|W?
                        if (paper.canSE[pos])
                        {
                            return TryConnection(paper, pos, E | S);
                        }
                        break;
                    // SW or WE
                    case W:
                        // Check there is a free line down to the source we are turning around
                        if (paper.canSW[pos] && CheckSWLane(paper, pos) && CheckImplicitSE(paper, pos))
                        {
                            if (TryConnection(paper, pos, S))
                            {
                                return true;
                            }
                        }
                        // Ensure we don't block off any diagonals (NE and NW don't seem very important)
                        if (paper.Con[pos - w + 1] != (S | W) && paper.Con[pos - w - 1] != (S | E))
                        {
                            return TryConnection(paper, pos, E);
                        }
                        break;
                    // NW
                    case (N | W):
                        // Check if the 'by others implied' turn is actually allowed
                        // We don't need to check the source connection here like in N|E
                        if (paper.Con[pos - w - 1] == (N | W) || paper.source[pos - w - 1])
                        {
                            return ChooseConnection(paper, paper.next[pos]);
                        }
                        break;
                    // NE or NS
                    case N:
                        // Check that we are either extending a corner or starting at a non-occupied source
                        if (paper.Con[pos - w + 1] == (N | E) || paper.source[pos - w + 1] && (paper.Con[pos - w + 1] & (N | E)) != 0)
                        {
                            if (TryConnection(paper, pos, E))
                            {
                                return true;
                            }
                        }
                        // Ensure we don't block off any diagonals
                        if (paper.Con[pos - w + 1] != (S | W) && paper.Con[pos - w - 1] != (S | E) && CheckImplicitSE(paper, pos))
                        {
                            if (TryConnection(paper, pos, S))
                            {
                                return true;
                            }
                        }
                        break;
                }
            }
            return false;
        }

        // Check that a SW line of corners, starting at pos, will not intersect a SE or NW line
        public static bool CheckSWLane(Paper paper, int pos)
        {
            while (!paper.source[pos])
            {
                // Con = 0 means we are crossing a SE line, N|W means a NW
                if (paper.Con[pos] != W)
                {
                    return false;
                }
                pos += paper.Width - 1;
            }
            return true;
        }

        // Check that a south connection at pos won't create a forced, illegal SE corner at pos+1
        // Something like: │└
        //                  │   <-- Forced SE corner
        public static bool CheckImplicitSE(Paper paper, int pos)
        {
            return !(paper.Con[pos + 1] == 0) || paper.canSE[pos + 1] || paper.Table[pos + 1] != EMPTY;
        }

        public static bool TryConnection(Paper paper, int pos1, int dirs)
        {
            // Extract the (last) bit which we will process in this call
            int dir = dirs & -dirs;
            int pos2 = pos1 + paper.Vctr[dir];
            int end1 = paper.end[pos1], end2 = paper.end[pos2];

            // Cannot connect out of the paper
            if (paper.Table[pos2] == GRASS)
            {
                return false;
            }
            // Check different sources aren't connected
            if (paper.Table[end1] != EMPTY && paper.Table[end2] != EMPTY &&
                paper.Table[end1] != paper.Table[end2])
            {
                return false;
            }
            // No loops
            if (end1 == pos2 && end2 == pos1)
            {
                return false;
            }
            int dir2 = 0;
            // No tight corners (Just an optimization)
            if (paper.Con[pos1] != 0)
            {
                dir2 = paper.Con[pos1 + paper.Vctr[paper.Con[pos1]]];
                int dir3 = paper.Con[pos1] | dir;
                if (DIAG[dir2] && DIAG[dir3] && (dir2 & dir3) != 0)
                {
                    return false;
                }
            }

            // Add the connection and a backwards connection from pos2
            int old1 = paper.Con[pos1], old2 = paper.Con[pos2];
            paper.Con[pos1] |= dir;
            paper.Con[pos2] |= MIR[dir];
            // Change states of ends to connect pos1 and pos2
            int old3 = paper.end[end1], old4 = paper.end[end2];
            paper.end[end1] = end2;
            paper.end[end2] = end1;

            // Remove the done bit and recurse if necessary
            dir2 = dirs & ~dir;
            bool res;
            if (dir2 == 0)
            {
                res = ChooseConnection(paper, paper.next[pos1]);
            }
            else
            {
                res = TryConnection(paper, pos1, dir2);
            }

            // Recreate the state, but not if a solution was found,
            // since we'll let it bubble all the way to the caller
            if (!res)
            {
                paper.Con[pos1] = old1;
                paper.Con[pos2] = old2;
                paper.end[end1] = old3;
                paper.end[end2] = old4;
            }

            return res;
        }

        // As it turns out, though our algorithm avoids must self-touching flows, it
        // can be tricked to allow some. Hence we need this validation to filter out
        // the false positives
        public static bool Validate(Paper paper)
        {
            int w = paper.Width, h = paper.Height;
            char[] vtable = new char[w * h];
            for (int pos = 0; pos < w * h; pos++)
            {
                if (paper.source[pos])
                {
                    // Run through the flow
                    char alpha = paper.Table[pos];
                    int p = pos, old = pos, next = pos;
                    while (true)
                    {
                        // Mark our path as we go
                        vtable[p] = alpha;
                        foreach (var dir in DIRS)
                        {
                            int cand = p + paper.Vctr[dir];
                            if ((paper.Con[p] & dir) != 0)
                            {
                                if (cand != old)
                                {
                                    next = cand;
                                }
                            }
                            else if (vtable[cand] == alpha)
                            {
                                // We aren't connected, but it has our color,
                                // this is exactly what we don't want.
                                return false;
                            }
                        }
                        // We have reached the end
                        if (old != p && paper.source[p])
                        {
                            break;
                        }
                        old = p;
                        p = next;
                    }
                }
            }
            return true;
        }

        public static void InitTables(Paper paper)
        {
            int w = paper.Width, h = paper.Height;

            // Direction vector table
            for (int dir = 0; dir < 16; dir++)
            {
                if ((dir & N) != 0)
                {
                    paper.Vctr[dir] += -w;
                }
                if ((dir & E) != 0)
                {
                    paper.Vctr[dir] += 1;
                }
                if ((dir & S) != 0)
                {
                    paper.Vctr[dir] += w;
                }
                if ((dir & W) != 0)
                {
                    paper.Vctr[dir] -= 1;
                }
            }

            // Positions of the four corners inside the grass
            paper.Crnr[N | W] = w + 1;
            paper.Crnr[N | E] = 2 * w - 2;
            paper.Crnr[S | E] = h * w - w - 2;
            paper.Crnr[S | W] = h * w - 2 * w + 1;

            // Table to easily check if a position is a source
            paper.source = new bool[w * h];
            for (int pos = 0; pos < w * h; pos++)
            {
                paper.source[pos] = paper.Table[pos] != EMPTY && paper.Table[pos] != GRASS;
            }

            // Pivot tables
            paper.canSE = new bool[w * h];
            paper.canSW = new bool[w * h];
            for (int pos = 0; pos < paper.Table.Count; pos++)
            {
                if (paper.source[pos])
                {
                    int d = paper.Vctr[N | W];
                    for (int p = pos + d; paper.Table[p] == EMPTY; p += d)
                    {
                        paper.canSE[p] = true;
                    }
                    d = paper.Vctr[N | E];
                    for (int p = pos + d; paper.Table[p] == EMPTY; p += d)
                    {
                        paper.canSW[p] = true;
                    }
                }
            }

            // Diagonal 'next' table
            paper.next = new int[w * h];
            int last = 0;
            int pos1 = 0;
            foreach (var pos in Concatenate(
                XRange(paper.Crnr[N | W], paper.Crnr[N | E], 1),
                XRange(paper.Crnr[N | E], paper.Crnr[S | E] + 1, w)))
            {
                pos1 = pos;
                while (paper.Table[pos1] != GRASS)
                {
                    paper.next[last] = (pos1);
                    last = pos1;
                    pos1 = pos1 + w - 1;
                }
            }

            // 'Where is the other end' table
            paper.end = new int[w * h];
            for (int pos = 0; pos < w * h; pos++)
            {
                paper.end[pos] = pos;
            }

            // Connection table
            paper.Con = new List<int>(new int[w * h]);
        }

        // Makes a list of the interval [i, i+step, i+2step, ..., j)
        static List<int> XRange(int i, int j, int step)
        {
            List<int> result = new List<int>();
            while (i < j)
            {
                result.Add(i);
                i += step;
            }
            return result;
        }

        // Concatenates multiple lists into a single list
        static List<int> Concatenate(params List<int>[] lists)
        {
            List<int> result = new List<int>();
            foreach (var list in lists)
            {
                result.AddRange(list);
            }
            return result;
        }
    }
}