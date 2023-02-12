using CalcudokuSolver.Extensions;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace CalcudokuSolver
{
    internal class Puzzle
    {
        private readonly Cell[,] grid;
        private readonly Clue[] clues;
        private readonly Clue[] clueLookup;
        private readonly int size;

        public int Size => size;

        public Puzzle(int size, string[] board, IDictionary<string, string> clues, int[] answer)
        {
            this.size = size;
            if (board.Length != size * size)
                throw new InvalidOperationException("Invalid board size");
            var cl = clues.ToDictionary(it => it.Key, it => new Clue { Formula = it.Value });
            var cm = cl.Values.ToDictionary(it => it, it => new List<Cell>());
            grid = new Cell[size, size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    grid[x, y] = new Cell
                    {
                        X = x,
                        Y = y,
                        Clue = cl[board[y * size + x].Trim()],
                        Answer = answer[y * size + x]
                    };
                    cm[grid[x, y].Clue].Add(grid[x, y]);
                }
            this.clues = cm.Keys.ToArray();
            for (int index = 0; index < this.clues.Length; index++)
            {
                var clue = this.clues[index];
                clue.Index = index;
                clue.Cells = cm[clue].ToArray();
                var (calc, isCommutative, expected) = CompileFormula(clue);
                clue.Calc = calc;
                clue.IsCommutative = isCommutative;
                clue.Expected = expected;
                clue.Uniques = clue.Cells.Select((it, i) => (it, i))
                       .GroupBy(it => it.it.X, it => it.i)
                       .Select(it => it.ToArray()).Where(it => it.Length > 1)
                       .Concat(clue.Cells.Select((it, i) => (it, i))
                       .GroupBy(it => it.it.Y, it => it.i)
                       .Select(it => it.ToArray()).Where(it => it.Length > 1)).ToArray();
            }
            this.clueLookup = this.clues
                .SelectMany(it => it.Cells, (clue, cell) => (key: cell.Y * Size + cell.X, clue))
                .OrderBy(it => it.key)
                .Select(it => it.clue)
                .ToArray();
        }

        private static Regex splitter = new(@"(\d+)\s*(\S+)");

        private (Func<int[], int>[], bool, int) CompileFormula(Clue clue)
        {
            var match = splitter.Match(clue.Formula);
            if (!match.Success)
                throw new InvalidOperationException("Formula is not in the form {value}{operator}");
            var expected = Int32.Parse(match.Groups[1].Value);
            var par = Expression.Parameter(typeof(int[]), "value");
            var (exprType, isCommutative) = match.Groups[2].Value switch
            {
                "x" => (ExpressionType.Multiply, true),
                "+" => (ExpressionType.Add, true),
                "-" => (ExpressionType.Subtract, false),
                "/" => (ExpressionType.Divide, false),
                "^" => (ExpressionType.Power, false),
                "or" => (ExpressionType.Or, true),
                "and" => (ExpressionType.And, true),
                "mod" => (ExpressionType.Modulo, false),
                _ => throw new InvalidOperationException($"Operator {match.Groups[2]} not supported")
            };

            // For commutative operations we don't need to check all permutations
            // e.g. 1x2x3 = 3x1x2 but for non commutative we do, e.g. 2/3 != 3/2
            var perm1 = isCommutative
                ? new[] { Enumerable.Range(0, clue.Cells.Length) }
                : Enumerable.Range(0, clue.Cells.Length).Permute();
            var calcList = new List<Func<int[], int>>();
            foreach (var perm in perm1)
            {
                Expression expr = null;
                foreach (var parm2 in perm)
                {
                    Expression expr1 = Expression.ArrayAccess(par, Expression.Constant(parm2));
                    if (exprType == ExpressionType.Power)
                        expr1 = Expression.Convert(expr1, typeof(double));
                    expr = expr == null ? expr1 : Expression.MakeBinary(exprType, expr, expr1);
                }
                if (expr.Type == typeof(double))
                    expr = Expression.Convert(expr, typeof(int));
                var lambda = Expression.Lambda<Func<int[], int>>(expr, par);
                calcList.Add(lambda.Compile());
            }
            return (calcList.ToArray(), isCommutative, expected);
        }

        // Eliminate bits in cell masks where possible
        // e.g. when three cells contain exactly the same three values we know
        // the other cells cannot contain these values
        // Returns true when there were bits eliminated
        private bool CheckColumn(PuzzleState state, int x, ref bool faulted)
        {
            bool dirty = false;
            var grouped = Enumerable.Range(0, Size).Select(y => state.GridState[x, y]).GroupBy(it => it).ToList();
            foreach (var item in grouped.Where(it => it.Key.GetBitCount() == it.Count()))
            {
                for (int y = 0; y < Size; y++)
                {
                    if (state.GridState[x, y] == 0) faulted = true;
                    if (state.GridState[x, y] != item.Key)   // Cell not part of the group?
                    {
                        int newVal = state.GridState[x, y] & ~item.Key;  // Clear the group's bits in that cell
                        if (newVal != state.GridState[x, y])
                        {
                            state.ClueState[clueLookup[y * Size + x].Index].Dirty = true;
                            dirty = true;
                        }
                    }
                }
            }
            return dirty;
        }

        // Eliminate bits in cell masks where possible
        // e.g. when three cells contain exactly the same three values we know
        // the other cells cannot contain these values
        // Returns true when there were bits eliminated
        private bool CheckRow(PuzzleState state, int y, ref bool faulted)
        {
            bool dirty = false;
            var grouped = Enumerable.Range(0, Size).Select(x => state.GridState[x, y]).GroupBy(it => it).ToList();
            foreach (var item in grouped.Where(it => it.Key.GetBitCount() == it.Count()))
            {
                for (int x = 0; x < Size; x++)
                {
                    if (state.GridState[x, y] == 0) faulted = true;
                    if (state.GridState[x, y] != item.Key)   // Cell not part of the group?
                    {
                        int newVal = state.GridState[x, y] & ~item.Key;  // Clear the group's bits in that cell
                        if (newVal != state.GridState[x, y])
                        {
                            state.ClueState[clueLookup[y * Size + x].Index].Dirty = true;
                            dirty = true;
                        }
                    }
                }
            }
            return dirty;
        }

        private bool Solve(int level, PuzzleState state, IProgress<PuzzleState> progress = null, CancellationToken cancellation = default)
        {
            bool puzzleDirty;
            bool puzzleSolved;
            bool faulted = false;
            int iter = 0;
            do
            {
                Trace.WriteLine($"Level: {level} Iteration: {iter++}");

                puzzleDirty = false;
                puzzleSolved = true;
                for (int clueIndex = 0; clueIndex < clues.Length; clueIndex++)
                {
                    if (state.ClueState[clueIndex].Dirty)
                    {
                        var clue = clues[clueIndex];
                        var testData = new int[clue.Cells.Length];
                        var counters = new IEnumerator<int>[clue.Cells.Length];
                        var masks = new int[clue.Cells.Length];
                        var resultMasks = new int[clue.Cells.Length];
                        for (int n = 0; n < testData.Length; n++)
                        {
                            var cell = clue.Cells[n];
                            masks[n] = state.GridState[cell.X, cell.Y];
                            counters[n] = masks[n].GetBitsSet().GetEnumerator();
                            // Counters[n] should have at least one value
                            if (!counters[n].MoveNext())
                            {
                                return false;
                            }
                        }

                        do /* Counter loop */
                        // -> test all permutations of the clue's cells depending on their bitmasks
                        {
                            // Fill the testdata for the cells 
                            for (int n = 0; n < testData.Length; n++)
                            {
                                testData[n] = counters[n].Current;
                            }

                            // Check if the testdata has duplicate values 
                            // in the same X- or Y direction (which is not allowed)
                            bool hasDups = false;
                            for (int n = 0; n < clue.Uniques.Length; n++)
                            {
                                // clue.Uniques contains groups of cell-indices that have to be unique
                                int umask = 0;
                                var uniq = clue.Uniques[n];
                                // The trick to test uniqueness is to set a bit in umask for every cell
                                // in the group. 
                                for (int o = 0; o < uniq.Length; o++)
                                {
                                    umask |= (1 << (testData[uniq[o]] - 1));
                                }
                                // If the number of bits is not equal to the size of the group,
                                // we know the testData values for that group are not unique.
                                if (umask.GetBitCount() != uniq.Length)
                                {
                                    hasDups = true;
                                    break;
                                }
                            }

                            // If no dups found, calculate the result by applying testdata to the formula
                            if (!hasDups)
                            {
                                try
                                {
                                    for (int q = 0; q < clue.Calc.Length; q++)
                                    {
                                        int value = clue.Calc[q](testData);
                                        if (value == clue.Expected)
                                        {
                                            for (int n = 0; n < testData.Length; n++)
                                            {
                                                resultMasks[n] |= (1 << (testData[n] - 1));
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    // Ignore exception, calculation not valid apparently
                                }
                            }

                            // Increment the counters (without recursion!)
                            int m = 0;
                            do
                            {
                                // Can increment Counter m?
                                if (!counters[m].MoveNext())
                                {
                                    // Reset Counter m
                                    counters[m] = masks[m].GetBitsSet().GetEnumerator();
                                    counters[m].MoveNext();
                                }
                                else
                                {
                                    break;
                                }
                            } while (++m < testData.Length);

                            // All combinations tested: Apply the result masks,
                            // eliminating bits (=possible values) in the clue's cell values 
                            if (m == testData.Length)
                            {
                                bool clueChanged = false;
                                bool clueSolved = true;

                                for (int n = 0; n < resultMasks.Length; n++)
                                {
                                    var resultMask = resultMasks[n];
                                    var cell = clue.Cells[n];
                                    clueChanged |= (state.GridState[cell.X, cell.Y] != resultMask);
                                    faulted |= (state.GridState[cell.X, cell.Y] == 0);
                                    state.GridState[cell.X, cell.Y] &= resultMask;
                                    clueSolved &= (resultMask.GetBitCount() == 1);
                                }

                                if (faulted) return false;

                                if (clueChanged)
                                {
                                    // Apply the changes to the rows and columns of the puzzle,
                                    // possibly making other clues 'dirty' (require recalculation)
                                    foreach (var x in clue.Cells.Select(it => it.X).Distinct())
                                        puzzleDirty |= CheckColumn(state, x, ref faulted);
                                    foreach (var y in clue.Cells.Select(it => it.Y).Distinct())
                                        puzzleDirty |= CheckRow(state, y, ref faulted);
                                }

                                if (faulted) return false;

                                state.ClueState[clueIndex] = new ClueState { Dirty = false, Solved = clueSolved };
                                puzzleSolved &= clueSolved;
                                break;
                            }

                        }
                        while (true);

                    }
                }
                progress?.Report(state);
            }
            while (puzzleDirty);

            if (!puzzleSolved)
            {
                var guesses = clues.SelectMany(it => it.Cells, (a, b) => (
                        clue: a,
                        cell: b,
                        clueState: state.ClueState[a.Index],
                        count: state.GridState[b.X, b.Y].GetBitCount())
                    )
                    .Where(it => !it.clueState.Solved && it.count > 1)
                    .OrderBy(it => it.count)
                    .ToList();

                foreach (var guess in guesses)
                {
                    var possibleValues = state.GridState[guess.cell.X, guess.cell.Y].GetBitsSet();
                    foreach (var valueToClear in possibleValues)
                    {
                        faulted = false;
                        var newState = new PuzzleState(state);
                        newState.GridState[guess.cell.X, guess.cell.Y] = 1 << (valueToClear - 1);
                        puzzleDirty |= CheckColumn(newState, guess.cell.X, ref faulted);
                        puzzleDirty |= CheckRow(newState, guess.cell.Y, ref faulted);
                        if (faulted)
                        {
                            // Should never happen, something is wrong in the puzzle logic
                            return false;
                        }
                        if (Solve(iter + 1, newState, progress, cancellation))
                        {
                            return true;
                        }
                    }
                }
            }
            return true;
        }

        public Task<bool> Solve(IProgress<PuzzleState> progress = null, CancellationToken cancellation = default)
        {
            var state = new PuzzleState(size, clues.Length);
            var correct = Solve(0, state, progress, cancellation);
            return Task.FromResult(correct);
        }


        public void Draw(Graphics g, PuzzleState state = null)
        {
            float sc = 1.5f;
            g.ScaleTransform(sc, sc);
            g.TranslateTransform(10f, 10f);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            using var gridPen = new Pen(Color.DarkGreen, 1f / sc);
            using var thickPen = new Pen(Color.Black, 2f);
            thickPen.StartCap = LineCap.Round;
            thickPen.EndCap = LineCap.Round;
            using var clueFont = new Font(FontFamily.GenericSansSerif, 7f);
            using var smallerClueFont = new Font(FontFamily.GenericSansSerif, 6f);
            using var answerFont = new Font(FontFamily.GenericSansSerif, 10f);
            using var smallerAnswerFont = new Font(FontFamily.GenericSansSerif, 5f);
            for (int n = 0; n <= Size; n++)
            {
                g.DrawLine(gridPen, n * 32, 0, n * 32, Size * 32);
                g.DrawLine(gridPen, 0, n * 32, Size * 32, n * 32);
            }
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    var cell = grid[x, y];
                    if (x == 0 || cell.Clue != grid[x - 1, y].Clue)
                        g.DrawLine(thickPen, x * 32, y * 32, x * 32, (y + 1) * 32);
                    if (x == Size - 1 || cell.Clue != grid[x + 1, y].Clue)
                        g.DrawLine(thickPen, (x + 1) * 32, y * 32, (x + 1) * 32, (y + 1) * 32);
                    if (y == 0 || cell.Clue != grid[x, y - 1].Clue)
                        g.DrawLine(thickPen, x * 32, y * 32, (x + 1) * 32, y * 32);
                    if (y == Size - 1 || cell.Clue != grid[x, y + 1].Clue)
                        g.DrawLine(thickPen, x * 32, (y + 1) * 32, (x + 1) * 32, (y + 1) * 32);
                    if (cell == cell.Clue.Cells[0])
                    {
                        var size = g.MeasureString(cell.Clue.Formula, clueFont);
                        g.DrawString(cell.Clue.Formula, size.Width < 30f ? clueFont : smallerClueFont, Brushes.Black, x * 32 + 1f, y * 32 + 1f);
                    }
                }
            if (state != null)
            {
                for (int x = 0; x < Size; x++)
                    for (int y = 0; y < Size; y++)
                    {
                        var values = state.GridState[x, y].GetBitsSet().ToArray();
                        string valueTxt = values.Length < 5 ? String.Join("|", values) : "...";
                        var font = values.Length == 1 ? answerFont : smallerAnswerFont;
                        var size = g.MeasureString(valueTxt, font);
                        g.DrawString(valueTxt, font, Brushes.Black,
                            x * 32 + 16 - size.Width / 2,
                            y * 32 + 16 - size.Height / 2
                        );
                    }
            }
        }
    }
}
