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
        /// Check if the board is null as the length of the word
        /// </summary>
        /// <param name="boardIdx"></param>
        /// <param name="wordLength"></param>
        /// <returns></returns>
        private bool CheckIfNullAsWordLength(int boardIdx, int wordLength)
        {
            // Horizontal
            for (int i = 1; i < wordLength; i++)
            {
                // 배열의 오른쪽 끝단에 막히나 (BoardSize의 배수에 해당하나 확인)
                // boardIdx + i + 1 = 배열은 0부터 시작하니 +1을 해줌
                // -1 : 지금 확인하는 배열 index의 전의 index 숫자가 끝자락에 다다른 거면 지금 숫자는 그 범위를 넘어가서 문제인 거니 -1을 해줌) // board 그리드 x 범위 넘어서면
                if ((boardIdx + i + 1 - 1) % BoardSize == 0
                    || (boardIdx + i) >= BoardSize* BoardSize
                    || !_board[boardIdx + i].Equals('\0')) return false;
            }
            return true;

        }

        private bool CheckIfNearbyClearForRandomPlacement(int boardIdxInit, int wordLength)
        {
            // Horizontal
            int CellCheckNum = BoardSize / 3;

            int LastWordIdx = boardIdxInit + wordLength - 1;

            for (int CurrentBoardIdx = boardIdxInit; CurrentBoardIdx <= LastWordIdx; CurrentBoardIdx++)
            {
                for (int i = 1; i < CellCheckNum; i++)
                {
                    // 첫글자
                    if (CurrentBoardIdx.Equals(boardIdxInit))
                    {
                        // 비교하려는 Index가 왼쪽 그리드 끝을 넘어서면 굳이 비교 x
                        if (CurrentBoardIdx - i % BoardSize == 0
                            || CurrentBoardIdx - i < 0) continue;

                        // Left
                        if (!_board[CurrentBoardIdx - i].Equals('\0'))
                        {
                            return false;
                        }
                    }

                    // 마지막 글자
                    if (CurrentBoardIdx.Equals(LastWordIdx))
                    {
                        // 비교하려는 Index가 오른쪽 그리드 끝을 넘어서면 굳이 비교 x
                        if (CurrentBoardIdx + i % BoardSize == 0
                            || CurrentBoardIdx + i >= BoardSize * BoardSize) continue;

                        // Right
                        if (!_board[CurrentBoardIdx + i].Equals('\0'))
                        {
                            return false;
                        }
                    }

                    // 비교하려는 Index가 위 그리드 끝을 넘어서면 굳이 비교 x
                    if (CurrentBoardIdx - (i * BoardSize) < 0) continue;

                    // Up
                    if (!_board[CurrentBoardIdx - (i * BoardSize)].Equals('\0'))
                    {
                        return false;
                    }

                    // 비교하려는 Index가 아래 그리드 끝을 넘어서면 굳이 비교 x
                    if (CurrentBoardIdx + (i * BoardSize) >= (BoardSize * BoardSize)) continue;

                    // Down
                    if (!_board[CurrentBoardIdx + (i * BoardSize)].Equals('\0'))
                    {
                        return false;
                    }

                }
            }

            return true;

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
                    for (int boardIdx = 0; boardIdx < _board.Length; boardIdx++)
                    {
                        // 만약 해당 cell이 null이라면 
                        if (_board[boardIdx].Equals('\0'))
                        {
                            bool nullAsLength = CheckIfNullAsWordLength(boardIdx, word.Length);
                            if (!nullAsLength) continue;

                            bool nearbyClearEnough = CheckIfNearbyClearForRandomPlacement(boardIdx, word.Length);
                            if (!nearbyClearEnough) continue;

                            bool diagonalClearByOne;

                            for(int wordIdx = 0; wordIdx < word.Length; wordIdx++)
                            {
                                _board[boardIdx+ wordIdx] = word[wordIdx];
                            }

                            break;
                        }



                    }
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
