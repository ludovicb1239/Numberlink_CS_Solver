using System.Drawing;

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
        static readonly public int[] MIR = { 0, S, W, 0, N, 0, 0, 0, E, 0, 0, 0, 0, 0, 0, 0 };
        static readonly public bool[] DIAG = new bool[16];

        public int Width { get; set; }
        public int Height { get; set; }

        public int[] Vctr = new int[16];
        public int[] Crnr = new int[16];

        public char[] Table;
        public int[] Connections;

        public bool[] isSource;
        public int[] end;
        public bool[] canSE;
        public bool[] canSW;

        public int[] next;

        public Dictionary<char, Color> colorLookup;

        public Paper()
        {
            DIAG[N | E] = true;
            DIAG[N | W] = true;
            DIAG[S | E] = true;
            DIAG[S | W] = true;
        }

        public static Paper NewPaper(int width, int height, char[] table)
        {
            Paper p = new();

            // Pad the given table with #, to make boundary checks easier
            int w = width + 2, h = height + 2;
            p.Width = w;
            p.Height = h;

            List<char> list = new();

            for (int i = 0; i < w; i++)
            {
                list.Add(GRASS);
            }
            for (int y = 1; y < h - 1; y++)
            {
                list.Add(GRASS);
                for (int x = 1; x < w - 1; x++)
                {
                    list.Add(table[(y - 1) * (w - 2) + (x - 1)]);
                }
                list.Add(GRASS);
            }
            for (int i = 0; i < w; i++)
            {
                list.Add(GRASS);
            }
            p.Table = list.ToArray();
            InitTables(p);

            return p;
        }

        public static bool Solve(Paper paper)
        {
            return ChooseConnection(paper, paper.Crnr[N | W]);
        }

        public static int Calls = 0;
        static bool ChooseConnection(Paper p, int pos)
        {
            Calls++;

            /*{ 
                // Debug Purposes, will save every step of process
            
                 Bitmap gameBoardBitmap = Logger.DrawSolution(paper);
                 gameBoardBitmap.Save($"board{Calls}.png");
            }*/

            // Final
            if (pos == 0)
                return Validate(p);

            int w = p.Width;
            if (p.isSource[pos])
            {
                switch (p.Connections[pos])
                {
                    // If the source is not yet connected
                    case 0:
                        // We can't connect E if we have a NE corner
                        if (p.Connections[pos - w + 1] != (S | W))
                        {
                            if (TryConnection(p, pos, E))
                                return true;
                        }
                        // South connections can create a forced SE position
                        if (CheckImplicitSE(p, pos))
                        {
                            if (TryConnection(p, pos, S))
                                return true;
                        }
                        break;
                    // If the source is already connected
                    case N:
                    case W:
                        return ChooseConnection(p, p.next[pos]);
                }
            }
            else
            {
                switch (p.Connections[pos])
                {
                    // SE
                    case 0:
                        // Should we check for implied N|W?
                        if (p.canSE[pos])
                        {
                            return TryConnection(p, pos, E | S);
                        }
                        break;
                    // SW or WE
                    case W:
                        // Check there is a free line down to the source we are turning around
                        if (p.canSW[pos] && CheckSWLane(p, pos) && CheckImplicitSE(p, pos))
                        {
                            if (TryConnection(p, pos, S))
                                return true;
                        }
                        // Ensure we don't block off any diagonals (NE and NW don't seem very important)
                        if (p.Connections[pos - w + 1] != (S | W) && p.Connections[pos - w - 1] != (S | E))
                        {
                            return TryConnection(p, pos, E);
                        }
                        break;
                    // NW
                    case (N | W):
                        // Check if the 'by others implied' turn is actually allowed
                        // We don't need to check the source connection here like in N|E
                        if (p.Connections[pos - w - 1] == (N | W) || p.isSource[pos - w - 1])
                        {
                            return ChooseConnection(p, p.next[pos]);
                        }
                        break;
                    // NE or NS
                    case N:
                        // Check that we are either extending a corner or starting at a non-occupied source
                        if (p.Connections[pos - w + 1] == (N | E) || p.isSource[pos - w + 1] && (p.Connections[pos - w + 1] & (N | E)) != 0)
                        {
                            if (TryConnection(p, pos, E))
                                return true;
                        }
                        // Ensure we don't block off any diagonals
                        if (p.Connections[pos - w + 1] != (S | W) && p.Connections[pos - w - 1] != (S | E) && CheckImplicitSE(p, pos))
                        {
                            if (TryConnection(p, pos, S))
                                return true;
                        }
                        break;
                }
            }
            return false;
        }

        // Check that a SW line of corners, starting at pos, will not intersect a SE or NW line
        public static bool CheckSWLane(Paper p, int pos)
        {
            while (!p.isSource[pos])
            {
                // Con = 0 means we are crossing a SE line, N|W means a NW
                if (p.Connections[pos] != W)
                    return false;
                pos += p.Width - 1;
            }
            return true;
        }

        // Check that a south connection at pos won't create a forced, illegal SE corner at pos+1
        // Something like: │└
        //                  │   <-- Forced SE corner
        public static bool CheckImplicitSE(Paper p, int pos)
        {
            return !(p.Connections[pos + 1] == 0) || p.canSE[pos + 1] || p.Table[pos + 1] != EMPTY;
        }

        public static bool TryConnection(Paper p, int pos1, int dirs)
        {
            // Extract the (last) bit which we will process in this call
            int dir = dirs & -dirs;
            int pos2 = pos1 + p.Vctr[dir];
            int end1 = p.end[pos1], end2 = p.end[pos2];

            // Cannot connect out of the paper
            if (p.Table[pos2] == GRASS)
            {
                return false;
            }
            // Check different sources aren't connected
            if (p.Table[end1] != EMPTY && p.Table[end2] != EMPTY &&
                p.Table[end1] != p.Table[end2])
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
            if (p.Connections[pos1] != 0)
            {
                dir2 = p.Connections[pos1 + p.Vctr[p.Connections[pos1]]];
                int dir3 = p.Connections[pos1] | dir;
                if (DIAG[dir2] && DIAG[dir3] && (dir2 & dir3) != 0)
                {
                    return false;
                }
            }

            // Add the connection and a backwards connection from pos2
            int old1 = p.Connections[pos1], old2 = p.Connections[pos2];
            p.Connections[pos1] |= dir;
            p.Connections[pos2] |= MIR[dir];
            // Change states of ends to connect pos1 and pos2
            int old3 = p.end[end1], old4 = p.end[end2];
            p.end[end1] = end2;
            p.end[end2] = end1;

            // Remove the done bit and recurse if necessary
            dir2 = dirs & ~dir;
            bool res;
            if (dir2 == 0)
                res = ChooseConnection(p, p.next[pos1]);
            else
                res = TryConnection(p, pos1, dir2);

            // Recreate the state, but not if a solution was found,
            // since we'll let it bubble all the way to the caller
            if (!res)
            {
                p.Connections[pos1] = old1;
                p.Connections[pos2] = old2;
                p.end[end1] = old3;
                p.end[end2] = old4;
            }

            return res;
        }

        // As it turns out, though our algorithm avoids must self-touching flows, it
        // can be tricked to allow some. Hence we need this validation to filter out
        // the false positives
        public static bool Validate(Paper p)
        {
            int w = p.Width, h = p.Height;
            char[] vtable = new char[w * h];
            for (int pos = 0; pos < w * h; pos++)
            {
                if (p.isSource[pos])
                {
                    // Run through the flow
                    char alpha = p.Table[pos];
                    int pos1 = pos, old = pos, next = pos;
                    while (true)
                    {
                        // Mark our path as we go
                        vtable[pos1] = alpha;
                        foreach (var dir in DIRS)
                        {
                            int cand = pos1 + p.Vctr[dir];
                            if ((p.Connections[pos1] & dir) != 0)
                            {
                                if (cand != old)
                                    next = cand;
                            }
                            else if (vtable[cand] == alpha)
                            {
                                // We aren't connected, but it has our color,
                                // this is exactly what we don't want.
                                return false;
                            }
                        }
                        // We have reached the end
                        if (old != pos1 && p.isSource[pos1])
                        {
                            break;
                        }
                        old = pos1;
                        pos1 = next;
                    }
                }
            }
            return true;
        }

        public static void InitTables(Paper p)
        {
            int w = p.Width, h = p.Height;

            // Direction vector table
            for (int dir = 0; dir < 16; dir++)
            {
                if ((dir & N) != 0)
                {
                    p.Vctr[dir] += -w;
                }
                if ((dir & E) != 0)
                {
                    p.Vctr[dir] += 1;
                }
                if ((dir & S) != 0)
                {
                    p.Vctr[dir] += w;
                }
                if ((dir & W) != 0)
                {
                    p.Vctr[dir] -= 1;
                }
            }

            // Positions of the four corners inside the grass
            p.Crnr[N | W] = w + 1;
            p.Crnr[N | E] = 2 * w - 2;
            p.Crnr[S | E] = h * w - w - 2;
            p.Crnr[S | W] = h * w - 2 * w + 1;

            // Table to easily check if a position is a source
            p.isSource = new bool[w * h];
            for (int pos = 0; pos < w * h; pos++)
            {
                p.isSource[pos] = p.Table[pos] != EMPTY && p.Table[pos] != GRASS;
            }

            // Pivot tables
            p.canSE = new bool[w * h];
            p.canSW = new bool[w * h];
            for (int pos = 0; pos < p.Table.Length; pos++)
            {
                if (p.isSource[pos])
                {
                    int d = p.Vctr[N | W];
                    for (int pos2 = pos + d; p.Table[pos2] == EMPTY; pos2 += d)
                    {
                        p.canSE[pos2] = true;
                    }
                    d = p.Vctr[N | E];
                    for (int pos2 = pos + d; p.Table[pos2] == EMPTY; pos2 += d)
                    {
                        p.canSW[pos2] = true;
                    }
                }
            }

            // Diagonal 'next' table
            p.next = new int[w * h];
            int last = 0, pos1;
            foreach (var pos in Concatenate(
                XRange(p.Crnr[N | W], p.Crnr[N | E], 1),
                XRange(p.Crnr[N | E], p.Crnr[S | E] + 1, w)))
            {
                pos1 = pos;
                while (p.Table[pos1] != GRASS)
                {
                    p.next[last] = (pos1);
                    last = pos1;
                    pos1 = pos1 + w - 1;
                }
            }

            // 'Where is the other end' table
            p.end = new int[w * h];
            for (int pos = 0; pos < w * h; pos++)
            {
                p.end[pos] = pos;
            }

            // Connection table
            p.Connections = new int[w * h];
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