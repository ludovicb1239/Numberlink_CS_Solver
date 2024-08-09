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

        public Paper(int width, int height, char[] table)
        {
            DIAG[N | E] = true;
            DIAG[N | W] = true;
            DIAG[S | E] = true;
            DIAG[S | W] = true;

            // Pad the given table with #, to make boundary checks easier
            int w = width + 2, h = height + 2;
            Width = w;
            Height = h;

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
            Table = list.ToArray();
            InitTables();
        }

        public bool Solve()
        {
            return ChooseConnection(Crnr[N | W]);
        }

        public int Calls = 0;
        bool ChooseConnection(int pos)
        {
            Calls++;

            /*{ 
                // Debug Purposes, will save every step of process
            
                 Bitmap gameBoardBitmap = Logger.DrawSolution(paper);
                 gameBoardBitmap.Save($"board{Calls}.png");
            }*/

            // Final
            if (pos == 0)
                return Validate();

            int w = Width;
            if (isSource[pos])
            {
                switch (Connections[pos])
                {
                    // If the source is not yet connected
                    case 0:
                        // We can't connect E if we have a NE corner
                        if (Connections[pos - w + 1] != (S | W))
                        {
                            if (TryConnection(pos, E))
                                return true;
                        }
                        // South connections can create a forced SE position
                        if (CheckImplicitSE(pos))
                        {
                            if (TryConnection(pos, S))
                                return true;
                        }
                        break;
                    // If the source is already connected
                    case N:
                    case W:
                        return ChooseConnection(next[pos]);
                }
            }
            else
            {
                switch (Connections[pos])
                {
                    // SE
                    case 0:
                        // Should we check for implied N|W?
                        if (canSE[pos])
                        {
                            return TryConnection(pos, E | S);
                        }
                        break;
                    // SW or WE
                    case W:
                        // Check there is a free line down to the source we are turning around
                        if (canSW[pos] && CheckSWLane(pos) && CheckImplicitSE(pos))
                        {
                            if (TryConnection(pos, S))
                                return true;
                        }
                        // Ensure we don't block off any diagonals (NE and NW don't seem very important)
                        if (Connections[pos - w + 1] != (S | W) && Connections[pos - w - 1] != (S | E))
                        {
                            return TryConnection(pos, E);
                        }
                        break;
                    // NW
                    case (N | W):
                        // Check if the 'by others implied' turn is actually allowed
                        // We don't need to check the source connection here like in N|E
                        if (Connections[pos - w - 1] == (N | W) || isSource[pos - w - 1])
                        {
                            return ChooseConnection(next[pos]);
                        }
                        break;
                    // NE or NS
                    case N:
                        // Check that we are either extending a corner or starting at a non-occupied source
                        if (Connections[pos - w + 1] == (N | E) || isSource[pos - w + 1] && (Connections[pos - w + 1] & (N | E)) != 0)
                        {
                            if (TryConnection(pos, E))
                                return true;
                        }
                        // Ensure we don't block off any diagonals
                        if (Connections[pos - w + 1] != (S | W) && Connections[pos - w - 1] != (S | E) && CheckImplicitSE(pos))
                        {
                            if (TryConnection(pos, S))
                                return true;
                        }
                        break;
                }
            }
            return false;
        }

        // Check that a SW line of corners, starting at pos, will not intersect a SE or NW line
        public bool CheckSWLane(int pos)
        {
            while (!isSource[pos])
            {
                // Con = 0 means we are crossing a SE line, N|W means a NW
                if (Connections[pos] != W)
                    return false;
                pos += Width - 1;
            }
            return true;
        }

        // Check that a south connection at pos won't create a forced, illegal SE corner at pos+1
        // Something like: │└
        //                  │   <-- Forced SE corner
        public bool CheckImplicitSE(int pos)
        {
            return !(Connections[pos + 1] == 0) || canSE[pos + 1] || Table[pos + 1] != EMPTY;
        }

        public bool TryConnection(int pos1, int dirs)
        {
            // Extract the (last) bit which we will process in this call
            int dir = dirs & -dirs;
            int pos2 = pos1 + Vctr[dir];
            int end1 = end[pos1], end2 = end[pos2];

            // Cannot connect out of the paper
            if (Table[pos2] == GRASS)
            {
                return false;
            }
            // Check different sources aren't connected
            if (Table[end1] != EMPTY && Table[end2] != EMPTY &&
                Table[end1] != Table[end2])
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
            if (Connections[pos1] != 0)
            {
                dir2 = Connections[pos1 + Vctr[Connections[pos1]]];
                int dir3 = Connections[pos1] | dir;
                if (DIAG[dir2] && DIAG[dir3] && (dir2 & dir3) != 0)
                {
                    return false;
                }
            }

            // Add the connection and a backwards connection from pos2
            int old1 = Connections[pos1], old2 = Connections[pos2];
            Connections[pos1] |= dir;
            Connections[pos2] |= MIR[dir];
            // Change states of ends to connect pos1 and pos2
            int old3 = end[end1], old4 = end[end2];
            end[end1] = end2;
            end[end2] = end1;

            // Remove the done bit and recurse if necessary
            dir2 = dirs & ~dir;
            bool res;
            if (dir2 == 0)
                res = ChooseConnection(next[pos1]);
            else
                res = TryConnection(pos1, dir2);

            // Recreate the state, but not if a solution was found,
            // since we'll let it bubble all the way to the caller
            if (!res)
            {
                Connections[pos1] = old1;
                Connections[pos2] = old2;
                end[end1] = old3;
                end[end2] = old4;
            }

            return res;
        }

        // As it turns out, though our algorithm avoids must self-touching flows, it
        // can be tricked to allow some. Hence we need this validation to filter out
        // the false positives
        public bool Validate()
        {
            int w = Width, h = Height;
            char[] vtable = new char[w * h];
            for (int pos = 0; pos < w * h; pos++)
            {
                if (isSource[pos])
                {
                    // Run through the flow
                    char alpha = Table[pos];
                    int pos1 = pos, old = pos, next = pos;
                    while (true)
                    {
                        // Mark our path as we go
                        vtable[pos1] = alpha;
                        foreach (var dir in DIRS)
                        {
                            int cand = pos1 + Vctr[dir];
                            if ((Connections[pos1] & dir) != 0)
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
                        if (old != pos1 && isSource[pos1])
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

        void InitTables()
        {
            int w = Width, h = Height;

            // Direction vector table
            for (int dir = 0; dir < 16; dir++)
            {
                if ((dir & N) != 0)
                {
                    Vctr[dir] += -w;
                }
                if ((dir & E) != 0)
                {
                    Vctr[dir] += 1;
                }
                if ((dir & S) != 0)
                {
                    Vctr[dir] += w;
                }
                if ((dir & W) != 0)
                {
                    Vctr[dir] -= 1;
                }
            }

            // Positions of the four corners inside the grass
            Crnr[N | W] = w + 1;
            Crnr[N | E] = 2 * w - 2;
            Crnr[S | E] = h * w - w - 2;
            Crnr[S | W] = h * w - 2 * w + 1;

            // Table to easily check if a position is a source
            isSource = new bool[w * h];
            for (int pos = 0; pos < w * h; pos++)
            {
                isSource[pos] = Table[pos] != EMPTY && Table[pos] != GRASS;
            }

            // Pivot tables
            canSE = new bool[w * h];
            canSW = new bool[w * h];
            for (int pos = 0; pos < Table.Length; pos++)
            {
                if (isSource[pos])
                {
                    int d = Vctr[N | W];
                    for (int pos2 = pos + d; Table[pos2] == EMPTY; pos2 += d)
                    {
                        canSE[pos2] = true;
                    }
                    d = Vctr[N | E];
                    for (int pos2 = pos + d; Table[pos2] == EMPTY; pos2 += d)
                    {
                        canSW[pos2] = true;
                    }
                }
            }

            // Diagonal 'next' table
            next = new int[w * h];
            int last = 0, pos1;
            foreach (var pos in Concatenate(
                XRange(Crnr[N | W], Crnr[N | E], 1),
                XRange(Crnr[N | E], Crnr[S | E] + 1, w)))
            {
                pos1 = pos;
                while (Table[pos1] != GRASS)
                {
                    next[last] = (pos1);
                    last = pos1;
                    pos1 = pos1 + w - 1;
                }
            }

            // 'Where is the other end' table
            end = new int[w * h];
            for (int pos = 0; pos < w * h; pos++)
            {
                end[pos] = pos;
            }

            // Connection table
            Connections = new int[w * h];
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