namespace CalcudokuSolver
{
    // Static Cell data (immutable/calculated once)
    public class Cell
    {
        public int Answer { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Clue Clue { get; set; }
    }
}
