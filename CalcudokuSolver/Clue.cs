namespace CalcudokuSolver
{
    // Static Clue data (immutable/calculated once)
    public class Clue
    {
        public int Index { get;  set; }
        // array of groups of Cell[]-indices that have to have unique values
        public int[][] Uniques { get; set; }
        public Cell[] Cells { get; set; }
        public string Formula { get; set; }
        // list of calculations (values in, result out)
        // If IsCommutative = true, there is just one calculation
        public Func<int[], int>[] Calc { get; set; }
        public bool IsCommutative { get; set; }
        public int Expected { get; set; }
    }
}
