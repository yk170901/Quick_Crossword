using QuickCrossword.Model;
using QuickCrossword.Model.Db;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCrossword.Controller
{
    public class CrosswordController
    {
        private char[]? _board;
        private static int BoardSize;

        private static CrosswordController _instance = null;
        public static CrosswordController Instance()
        {
            if(_instance == null)
                _instance = new CrosswordController();
            return _instance;
        }

        // PUBLIC
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Completed matrix of crossword</returns>
        public char[]? GetBoard(BoardMode boardMode = BoardMode.TenXTen)
        {
            BoardSize = (int)boardMode;
            CreateBoard();
            return _board;
        }

        /// <summary>
        /// Retrieve an random array of 30 words and clues from Sqlite DB
        /// </summary>
        private WordAndClue[] GetWACFromDb()
        {
            Random rnd = new Random();
            var WordAndClueList_raw = SqliteDataAccess.LoadWordAndClue();

            List<WordAndClue> WordAndClueList = new();
            // Take 50 random numbers in range of 4 to 214 with NO duplicate
            var randomNumbers = Enumerable.Range(4, 214).OrderBy(x => rnd.Next()).Take(30).ToArray();

            // Add up 50 random WordAndClue to the list with NO duplicate
            foreach (int rndNum in randomNumbers)
            {
                var Enumerable_WAC = WordAndClueList_raw.Where(o => o.Id == rndNum);
                foreach (var WAC in Enumerable_WAC)
                {
                    WordAndClueList.Add(new WordAndClue() { Id = WAC.Id, Word = WAC.Word, Clue = WAC.Clue });
                }
            }
            return WordAndClueList.ToArray();
        }

        // IMPORTANT
        /// <summary>
        /// Create matrix of crossword based on data from Sqlite DB
        /// </summary>
        private void CreateBoard()
        {
            // Will contain tons of methods
            WordAndClue[] WordAndClueArray = GetWACFromDb();

            PutWordsInMatrix(WordAndClueArray);
        }
        

        /// <summary>
        /// Put random words from WordAndClue into the _board to see if they generates whole valid a board of 1 dimension with one another
        /// </summary>
        private void PutWordsInMatrix(WordAndClue[] WordAndClueArray)
        {
            _board = new char[BoardSize* BoardSize];
            Direction direction;
            // int x, y;

            bool NearbyClear = false;
            bool Linked = false;
            bool FitInGrid;

            int cnt = -1;
            foreach (WordAndClue WAC in WordAndClueArray)
            {
                cnt++;

                string? word = WAC.Word;

                bool CanPlace = false;
                bool IsMatchingChar = false;

                if (!Linked) // Put it at random location
                {

                }


            }

            // grid 형태로 _board를 보기
            int dd = 0;
            foreach (var g in _board)
            {
                if (g == '\0')
                    Debug.Write("()");
                else
                    Debug.Write(g);
                dd++;
                if(dd % BoardSize == 0)
                    Debug.WriteLine("");
            }

        }

        private WordDetail[] SavePlacedWordDetail()
        {
            List<WordDetail> PlacedWordDetail = new();

            return PlacedWordDetail.ToArray();
        }

    }
}
