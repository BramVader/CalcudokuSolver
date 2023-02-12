using static CalcudokuSolver.Puzzle;

namespace CalcudokuSolver
{
    public partial class Form1 : Form
    {
        private readonly string[] board = new[] {
            " A", " A", " B", " B", " B", " C", " D", " D", " D", " E", " E", " F", " F", " F", " G", " H", " I", " I", " J", " J",
            " A", " K", " K", " B", " C", " C", " C", " D", " 3", " 1", " 1", " 0", " F", " z", " G", " H", " t", " t", " t", " s",
            " L", " K", " M", " N", " N", " 5", " 5", " 4", " 3", " 2", " 2", " 0", " z", " z", " y", " x", " u", " u", " u", " s",
            " L", " L", " M", " 6", " 6", " 6", " 5", " 4", " 4", "AA", "AA", " 0", "AP", " z", " y", " x", " v", " v", " v", " r",
            " O", " O", " M", " 7", " 7", " 5", " 5", " 4", " 8", "AB", "AA", "AP", "AP", "AP", "AQ", "AR", "AR", " w", " w", " r",
            " P", " P", "AE", "AD", "AC", " 9", " 9", " 8", " 8", "AB", "AB", "AO", "AO", "AQ", "AQ", "AQ", "AS", "AS", " q", " q",
            " Q", " P", "AE", "AD", "AC", " 9", " 9", "AH", " 8", "AJ", "AJ", "AN", "AN", "AW", "AV", "AU", "AT", "AT", "AT", " p",
            " Q", " R", "AF", "AF", "AF", "AG", "AH", "AH", "AI", "AK", "AJ", "AM", "AN", "AW", "AV", "AU", "AU", "AX", " p", " p",
            " R", " R", " R", "AF", "AG", "AG", "AG", "AI", "AI", "AK", "AL", "AM", "AY", "AY", "AV", "AU", "AX", "AX", "AX", " p",
            " S", "Ak", "Ak", "Aj", "Ai", "Ah", "Ag", "Af", "Ae", "Ae", "AL", "Ac", "Ac", "Ab", "Aa", "Aa", "AZ", " o", " n", " m",
            " S", "Ak", "Ak", "Aj", "Ai", "Ah", "Ag", "Af", "Af", "Aq", "Ad", "Ad", "Ac", "Ab", "Aa", "AZ", "AZ", " o", " n", " m",
            " T", "Al", "Al", "Al", "Am", "An", "Ao", "Ao", "Ap", "Aq", "Ar", "As", "As", "At", "At", "At", "Au", " l", " l", " l",
            " T", " T", "Al", "Am", "Am", "An", "BE", "BD", "Ap", "BC", "Ar", "As", "A4", "A4", "At", "Au", "Au", "Au", " l", " k",
            " T", "BF", "BF", "BF", "Am", "An", "BE", "BD", "BD", "BC", "BC", "A5", "A4", "A3", "A3", "A2", "A1", "Av", " j", " k",
            " U", " U", "BG", "BG", "BH", "BH", "BI", "BI", "BI", "BB", "BB", "A5", "A5", "A3", "A3", "A2", "A1", "Av", " j", " j",
            " V", "BP", "BP", "BO", "BO", "BH", "BJ", "BJ", "BJ", "BA", "BB", "A5", "A6", "A7", "A7", "A0", "A0", "Aw", " i", " i",
            " V", "BQ", "BQ", "BQ", "BN", "BM", "BL", "BJ", "BK", "BA", "BA", "A6", "A6", "A7", "Az", "Az", "Az", "Aw", " h", " h",
            " W", "BR", "BR", "BR", "BN", "BM", "BL", "BL", "BK", "A9", "A9", "A8", "A6", "A7", "A7", "Ay", "Ay", "Aw", "Ax", " h",
            " W", "BS", "BS", "BS", " Z", " a", "BL", " b", "BK", "A9", "A9", "A8", " d", " e", " e", " e", " f", "Ax", "Ax", " g",
            " X", " X", " Y", " Y", " Z", " a", " b", " b", " b", " c", " c", " d", " d", " d", " e", " f", " f", " f", " g", " g",
        };

        private readonly Dictionary<string, string> clues = new()
        {
            { "A", "50 +" },
            { "B", "693 x" },
            { "C", "44 +" },
            { "D", "5434 x" },
            { "E", "13 +" },
            { "F", "1275 x" },
            { "G", "20 +" },
            { "H", "0 mod" },
            { "I", "2 -" },
            { "J", "204 x" },
            { "K", "45 +" },
            { "L", "3510 x" },
            { "M", "130 x" },
            { "N", "9 /" },
            { "O", "6 or" },
            { "P", "7 -" },
            { "Q", "14 ^" },
            { "R", "48 +" },
            { "S", "19 +" },
            { "T", "4 -" },
            { "U", "30 +" },
            { "V", "32 +" },
            { "W", "12 or" },
            { "X", "15 x" },
            { "Y", "28 +" },
            { "Z", "15 +" },
            { "a", "21 or" },
            { "b", "32 +" },
            { "c", "23 +" },
            { "d", "10800 x" },
            { "e", "90972 x" },
            { "f", "25740 x" },
            { "g", "14 -" },
            { "h", "0 -" },
            { "i", "2 /" },
            { "j", "36 +" },
            { "k", "1 -" },
            { "l", "5712 x" },
            { "m", "13 +" },
            { "n", "3 +" },
            { "o", "31 or" },
            { "p", "49 +" },
            { "q", "23 or" },
            { "r", "15 +" },
            { "s", "9 mod" },
            { "t", "42 +" },
            { "u", "20 +" },
            { "v", "2187 ^" },
            { "w", "6 -" },
            { "x", "13 or" },
            { "y", "21 +" },
            { "z", "2730 x" },
            { "0", "3952 x" },
            { "1", "16 ^" },
            { "2", "49 ^" },
            { "3", "6 -" },
            { "4", "20230 x" },
            { "5", "46816 x" },
            { "6", "1188 x" },
            { "7", "153 x" },
            { "8", "15 or" },
            { "9", "14280 x" },
            { "AA", "33 +" },
            { "AB", "2640 x" },
            { "AC", "14 +" },
            { "AD", "216 x" },
            { "AE", "14 or" },
            { "AF", "3 -" },
            { "AG", "26000 x" },
            { "AH", "0 -" },
            { "AI", "51 +" },
            { "AJ", "33 +" },
            { "AK", "9 +" },
            { "AL", "14 x" },
            { "AM", "3 -" },
            { "AN", "27 +" },
            { "AO", "96 x" },
            { "AP", "1540 x" },
            { "AQ", "680 x" },
            { "AR", "30 +" },
            { "AS", "195 x" },
            { "AT", "819 x" },
            { "AU", "616 x" },
            { "AV", "1320 x" },
            { "AW", "25 +" },
            { "AX", "2700 x" },
            { "AY", "6 /" },
            { "AZ", "5814 x" },
            { "Aa", "27 +" },
            { "Ab", "625 ^" },
            { "Ac", "40 +" },
            { "Ad", "23 +" },
            { "Ae", "31 +" },
            { "Af", "28 +" },
            { "Ag", "5 +" },
            { "Ah", "7776 ^" },
            { "Ai", "156 x" },
            { "Aj", "21 +" },
            { "Ak", "48 +" },
            { "Al", "2916 x" },
            { "Am", "38 +" },
            { "An", "2394 x" },
            { "Ao", "14 +" },
            { "Ap", "17 +" },
            { "Aq", "38 +" },
            { "Ar", "21 +" },
            { "As", "31 +" },
            { "At", "23 or" },
            { "Au", "49 +" },
            { "Av", "81 ^" },
            { "Aw", "38 +" },
            { "Ax", "46 +" },
            { "Ay", "288 x" },
            { "Az", "819 x" },
            { "A0", "4 +" },
            { "A1", "44 x" },
            { "A2", "1 mod" },
            { "A3", "33660 x" },
            { "A4", "952 x" },
            { "A5", "952 x" },
            { "A6", "18720 x" },
            { "A7", "34 +" },
            { "A8", "59049 ^" },
            { "A9", "50 +" },
            { "BA", "13 +" },
            { "BB", "42 +" },
            { "BC", "30 +" },
            { "BD", "25 ^" },
            { "BE", "31 +" },
            { "BF", "140 x" },
            { "BG", "7 or" },
            { "BH", "2052 x" },
            { "BI", "2430 x" },
            { "BJ", "51 +" },
            { "BK", "15 or" },
            { "BL", "1428 x" },
            { "BM", "50625 ^" },
            { "BN", "13 +" },
            { "BO", "0 mod" },
            { "BP", "31 or" },
            { "BQ", "1 -"},
            { "BR", "1710 x"},
            { "BS", "2 -"}
        };

        private readonly int[] answer = new int[] {
            20, 18, 7, 11, 3, 16, 13, 19, 2, 9, 4, 1, 5, 15, 14, 10, 8, 6, 12, 17,
            12, 10, 19, 3, 15, 8, 5, 11, 14, 1, 16, 13, 17, 7, 6, 2, 20, 18, 4, 9,
            18, 16, 5, 9, 1, 14, 11, 17, 20, 7, 2, 19, 3, 13, 12, 8, 6, 4, 10, 15,
            15, 13, 2, 6, 18, 11, 8, 14, 17, 4, 19, 16, 20, 10, 9, 5, 3, 1, 7, 12,
            6, 4, 13, 17, 9, 2, 19, 5, 8, 15, 10, 7, 11, 1, 20, 16, 14, 12, 18, 3,
            7, 5, 14, 18, 10, 3, 20, 6, 9, 16, 11, 8, 12, 2, 1, 17, 15, 13, 19, 4,
            1, 19, 8, 12, 4, 17, 14, 20, 3, 10, 5, 2, 6, 16, 15, 11, 9, 7, 13, 18,
            14, 12, 1, 5, 17, 10, 7, 13, 16, 3, 18, 15, 19, 9, 8, 4, 2, 20, 6, 11,
            17, 15, 4, 8, 20, 13, 10, 16, 19, 6, 1, 18, 2, 12, 11, 7, 5, 3, 9, 14,
            10, 8, 17, 1, 13, 6, 3, 9, 12, 19, 14, 11, 15, 5, 4, 20, 18, 16, 2, 7,
            9, 7, 16, 20, 12, 5, 2, 8, 11, 18, 13, 10, 14, 4, 3, 19, 17, 15, 1, 6,
            11, 9, 18, 2, 14, 7, 4, 10, 13, 20, 15, 12, 16, 6, 5, 1, 19, 17, 3, 8,
            2, 20, 9, 13, 5, 18, 15, 1, 4, 11, 6, 3, 7, 17, 16, 12, 10, 8, 14, 19,
            3, 1, 10, 14, 6, 19, 16, 2, 5, 12, 7, 4, 8, 18, 17, 13, 11, 9, 15, 20,
            16, 14, 3, 7, 19, 12, 9, 15, 18, 5, 20, 17, 1, 11, 10, 6, 4, 2, 8, 13,
            13, 11, 20, 4, 16, 9, 6, 12, 15, 2, 17, 14, 18, 8, 7, 3, 1, 19, 5, 10,
            19, 17, 6, 10, 2, 15, 12, 18, 1, 8, 3, 20, 4, 14, 13, 9, 7, 5, 11, 16,
            8, 6, 15, 19, 11, 4, 1, 7, 10, 17, 12, 9, 13, 3, 2, 18, 16, 14, 20, 5,
            4, 2, 11, 15, 7, 20, 17, 3, 6, 13, 8, 5, 9, 19, 18, 14, 12, 10, 16, 1,
            5, 3, 12, 16, 8, 1, 18, 4, 7, 14, 9, 6, 10, 20, 19, 15, 13, 11, 17, 2
        };

        private readonly Puzzle puzzle;
        private bool activated;
        private CancellationTokenSource cts;
        private PuzzleState currentState = null;

        public Form1()
        {
            cts = new CancellationTokenSource();
            InitializeComponent();
            this.puzzle = new Puzzle(20, board, clues, answer);
            this.Paint += Form1_Paint;
            this.Activated += Form1_Activated;
        }

        public void ReportProgress(PuzzleState state)
        {
            this.currentState = state;
            if (InvokeRequired)
                Invoke(new Action(() => Invalidate()));
            else
                Invalidate();
        }

        private async void Form1_Activated(object sender, EventArgs e)
        {
            if (!activated)
            {
                activated = true;
                var cancelled = false;
                if (cts.IsCancellationRequested == true)
                {
                    cts.Dispose();
                    cts = new CancellationTokenSource();
                }

                var progressIndicator = new Progress<PuzzleState>(ReportProgress);

                try
                {
                    await puzzle.Solve(progressIndicator, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Text = "Cancelled";
                    cancelled = true;
                }

                if (!cancelled) return;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            puzzle.Draw(e.Graphics, currentState);
        }
    }
}