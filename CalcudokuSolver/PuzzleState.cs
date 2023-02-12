namespace CalcudokuSolver
{
    public class PuzzleState
    {
        private readonly int[,] gridState;
        private readonly ClueState[] clueState;

        public int[,] GridState => gridState;
        public ClueState[] ClueState => clueState;

        public PuzzleState(int size, int clueCount)
        {
            this.gridState = new int[size, size];
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    this.gridState[x, y] = (1 << size) - 1; // All bits set: every value possible
            this.clueState = new ClueState[clueCount];
            for (int n = 0; n < clueCount; n++)
                this.clueState[n] = new ClueState { Dirty = true, Solved = false };
        }

        public PuzzleState(PuzzleState other)
        {
            var size = other.gridState.GetLength(0);
            this.gridState = new int[size, size];
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    this.gridState[x, y] = other.gridState[x, y];
            this.clueState = new ClueState[other.clueState.Length];
            for (int n = 0; n < other.clueState.Length; n++)
                this.clueState[n] = other.clueState[n];
        }
    }
}
