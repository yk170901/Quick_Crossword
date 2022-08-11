using QuickCrossword.Model;
using QuickCrossword.Model.Db;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuickCrossword.Controller
{
    public class CrosswordController
    {
        private char[]? _board;
        private static int BoardSize;
        private static List<WordDetail> PlacedWordDetail;

        private static CrosswordController _instance = null;
        public static CrosswordController Instance()
        {
            if (_instance == null)
                _instance = new CrosswordController();
            return _instance;
        }

        // PUBLIC, IMPORTANT
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Completed matrix of crossword</returns>
        public char[]? GetBoard(BoardMode boardMode = BoardMode.TenXTen)
        {
            PlacedWordDetail = new();

            BoardSize = (int)boardMode;

            WordAndClue[] WordAndClueArray = GetWACFromDb();

            PutWordsOnBoard(WordAndClueArray);

            GetRidOfIsolztedWords();

            return _board;
        }

        private void GetRidOfIsolztedWords()
        {

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

        /// <summary>
        /// Check if the board is null as the length of the word
        /// </summary>
        private bool CheckIfNullAsWordLengthHorizontally(int boardIdx, int wordLength)
        {
            // Horizontal
            for (int i = 1; i < wordLength; i++)
            {
                /* 배열의 오른쪽 끝단에 막히나 (BoardSize의 배수에 해당하나 확인)
                   boardIdx + i + 1 = 배열은 0부터 시작하니 +1을 해줌
                   -1 : 지금 확인하는 배열 index의 전의 index 숫자가 끝자락에 다다른 거면 지금 숫자는 그 범위를 넘어가서 문제인 거니 -1을 해줌) */
                if ((boardIdx + i + 1 - 1) % BoardSize == 0
                    || (boardIdx + i) >= BoardSize * BoardSize
                    || !_board[boardIdx + i].Equals('\0')) return false;
            }
            return true;
        }

        // IS T?HIS RRIGHT??
        /// <summary>
        /// Check if the board is null as the length of the word
        /// </summary>
        private bool CheckIfNullAsWordLengthVertically(int boardIdx, int wordLength)
        {
            // Horizontal
            for (int i = boardIdx; i <= boardIdx+((wordLength - 1) * BoardSize); i = i + BoardSize)
            {
                if (i >= BoardSize * BoardSize
                    || !_board[i].Equals('\0')) return false;
            }
            return true;
        }

        // Vertical
        /// <summary>
        /// 
        /// </summary>
        private bool CheckIfDiagonalClearByOneVertically(int boardIdxInit, int wordLength)
        {
            int LastWordIdx = boardIdxInit + ((wordLength - 1) * BoardSize);

            // Top Left
            bool TopLeftClear = false;
            int TopLeftIdx = (boardIdxInit - BoardSize) - 1;
            if ((TopLeftIdx + 1) % BoardSize == 0 // 0부터 시작하는 배열이니 +1
                || TopLeftIdx < 0
                || _board[TopLeftIdx].Equals('\0')) TopLeftClear = true;

            if (!TopLeftClear) return false;

            // Bottom Left
            bool BottomLeftClear = false;
            int BottomLeftIdx = (LastWordIdx + BoardSize) - 1;
            if ((BottomLeftIdx + 1) % BoardSize == 0 // 0부터 시작하는 배열이니 +1
                || BottomLeftIdx >= BoardSize * BoardSize
                || _board[BottomLeftIdx].Equals('\0')) BottomLeftClear = true;

            if (!BottomLeftClear) return false;

            // Top Right
            bool TopRightClear = false;
            int TopRightIdx = (boardIdxInit - BoardSize) + 1;
            if ((TopRightIdx) % BoardSize == 0 // 왜 이건 그냥 해도 되지???
                || TopRightIdx < 0
                || _board[TopRightIdx].Equals('\0')) TopRightClear = true;

            if (!TopRightClear) return false;

            bool BottomRightClear = false;
            // Bottom Right
            int BottomRightIdx = (LastWordIdx + BoardSize) + 1; // 여기에서 이미 +1을 했기 때문에?
            if ((BottomRightIdx) % BoardSize == 0 // 왜 이건 그냥 해도 되지???
                || BottomRightIdx >= BoardSize * BoardSize
                || _board[BottomRightIdx].Equals('\0')) BottomRightClear = true;

            if (!BottomRightClear) return false;

            return true;
        }


        private bool CheckIfDiagonalClearByOneHorizontally(int boardIdxInit, int wordLength)
        {
            int LastWordIdx = boardIdxInit + wordLength - 1;

            // Top Left
            bool TopLeftClear = false;
            int TopLeftIdx = (boardIdxInit - BoardSize) - 1;
            if ((TopLeftIdx + 1) % BoardSize == 0 // 0부터 시작하는 배열이니 +1
                || TopLeftIdx < 0
                || _board[TopLeftIdx].Equals('\0')) TopLeftClear = true;

            if (!TopLeftClear) return false;

            // Bottom Left
            bool BottomLeftClear = false;
            int BottomLeftIdx = (boardIdxInit + BoardSize) - 1;
            if ((BottomLeftIdx + 1) % BoardSize == 0 // 0부터 시작하는 배열이니 +1
                || BottomLeftIdx >= BoardSize * BoardSize
                || _board[BottomLeftIdx].Equals('\0')) BottomLeftClear = true;

            if (!BottomLeftClear) return false;

            // Top Right
            bool TopRightClear = false;
            int TopRightIdx = (LastWordIdx - BoardSize) + 1;
            if ((TopRightIdx) % BoardSize == 0 // 왜 이건 그냥 해도 되지???
                || TopRightIdx < 0
                || _board[TopRightIdx].Equals('\0')) TopRightClear = true;

            if (!TopRightClear) return false;

            bool BottomRightClear = false;
            // Bottom Right
            int BottomRightIdx = (LastWordIdx + BoardSize) + 1; // 여기에서 이미 +1을 했기 때문에?
            if ((BottomRightIdx) % BoardSize == 0 // 왜 이건 그냥 해도 되지???
                || BottomRightIdx >= BoardSize * BoardSize
                || _board[BottomRightIdx].Equals('\0')) BottomRightClear = true;

            if (!BottomRightClear) return false;

            return true;
        }

        // 이거 다른 로직 덧붙한 검이
        private bool CheckIfNearbyClearEnoughVertically(int boardIdxInit, int wordLength)
        {
            // Horizontal
            int CellCheckNum = BoardSize / 2;

            int LastWordIdx = boardIdxInit + ((wordLength -1) * BoardSize);

            for (int CurrentBoardIdx = boardIdxInit; CurrentBoardIdx <= LastWordIdx; CurrentBoardIdx = CurrentBoardIdx + BoardSize) // CurrentBoardIdx += BoardSize
            {
                // currenBoardIdx = 18 25 32
                for (int i = 1; i < CellCheckNum; i++)
                {
                    // 첫글자
                    if (CurrentBoardIdx.Equals(boardIdxInit))
                    {
                        // 비교하려는 Index가 위 그리드 끝을 넘어서면 굳이 비교 x
                        if (CurrentBoardIdx - (i * BoardSize) < 0)
                        {
                            goto SkipUpOrDown;
                        }

                        // Up
                        if (!_board[CurrentBoardIdx - (i * BoardSize)].Equals('\0'))
                        {
                            return false;
                        }
                    }

                    // 마지막 글자
                    if (CurrentBoardIdx.Equals(LastWordIdx))
                    {
                        // 비교하려는 Index가 아래 그리드 끝을 넘어서면 굳이 비교 x
                        if (CurrentBoardIdx + (i * BoardSize) >= (BoardSize * BoardSize))
                        {
                            goto SkipUpOrDown;
                        }

                        // Down
                        if (!_board[CurrentBoardIdx + (i * BoardSize)].Equals('\0'))
                        {
                            return false;
                        }
                    }

                SkipUpOrDown:;

                    // 비교하려는 Index가 왼쪽 그리드 끝을 넘어서면 굳이 비교 x
                    if (CurrentBoardIdx - i % BoardSize == 0
                        || CurrentBoardIdx - i < 0)
                    {
                        goto SkipLeft;
                    }

                    // Left
                    if (!_board[CurrentBoardIdx - i].Equals('\0'))
                    {
                        return false;
                    }

                SkipLeft:;

                    // 비교하려는 Index가 오른쪽 그리드 끝을 넘어서면 굳이 비교 x
                    if (CurrentBoardIdx + i % BoardSize == 0
                        || CurrentBoardIdx + i >= BoardSize * BoardSize)
                    {
                        goto SkipRight;
                    }

                    // Right
                    if (!_board[CurrentBoardIdx + i].Equals('\0'))
                    {
                        return false;
                    }

                SkipRight:;
                }
            }
            return true;
        }

        private bool CheckIfNearbyClearEnoughHorizontally(int boardIdxInit, int wordLength)
        {
            // Horizontal
            int CellCheckNum = BoardSize / 2;

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
                            || CurrentBoardIdx - i < 0)
                        {
                            goto SkipUpOrDown;
                        }

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
                            || CurrentBoardIdx + i >= BoardSize * BoardSize)
                        {
                            goto SkipUpOrDown;
                        }

                        // Right
                        if (!_board[CurrentBoardIdx + i].Equals('\0'))
                        {
                            return false;
                        }
                    }

                SkipUpOrDown:;

                    // 비교하려는 Index가 위 그리드 끝을 넘어서면 굳이 비교 x
                    if (CurrentBoardIdx - (i * BoardSize) < 0)
                    {
                        goto SkipUp;
                    }

                    // Up
                    if (!_board[CurrentBoardIdx - (i * BoardSize)].Equals('\0'))
                    {
                        return false;
                    }

                SkipUp:;

                    // 비교하려는 Index가 아래 그리드 끝을 넘어서면 굳이 비교 x
                    if (CurrentBoardIdx + (i * BoardSize) >= (BoardSize * BoardSize))
                    {
                        goto SkipDown;
                    }

                    // Down
                    if (!_board[CurrentBoardIdx + (i * BoardSize)].Equals('\0'))
                    {
                        return false;
                    }
                SkipDown:;
                }
            }

            return true;

        }
        
        private bool CheckIfNearbyClearHorizontally(Direction direction, int wordLength)
        {
            return true;
        }

        private int GetMatchingCharIndex(char wordChar)
        {
            for(int boardIdx = 0; boardIdx < _board.Length; boardIdx++)
            {
            }

            return -1;
        }

        private bool CanLinkToWordOnBoard(string word)
        {
            // 글자 단어 하나씩 확인
            for (int i = 0; i < word.Length; i++)
            {
                int MatchingCharIndex = GetMatchingCharIndex(word[i]);

                // 일치하는 글자가 없을 경우
                if (MatchingCharIndex == -1) continue;
                Direction direction = Direction.Horizontal;
                bool NearbyClear = CheckIfNearbyClearHorizontally(direction, word.Length); // Horizontal

            }

            return false;
        }

        /// <summary>
        /// Put random words from WordAndClue into the _board to see if they generates whole valid a board of 1 dimension with one another
        /// </summary>
        private void PutWordsOnBoard(WordAndClue[] WordAndClueArray)
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
                if(WAC == null)
                {
                    MessageBox.Show("Unexpected Error Occured. WAC is null");
                    continue;
                }

                cnt++;

                string? word = WAC.Word;

                bool LinkedToWordOnBoard = CanLinkToWordOnBoard(word);

                if (!LinkedToWordOnBoard) // Put it at random location
                {
                    if (cnt % 2 == 0)
                    {
                        PlaceHorizontallyRandom(word, WAC.Clue);
                    }
                    else
                    {
                        PlaceVerticallyRandom(word, WAC.Clue);
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

        // Vertical
        /// <summary>
        /// 
        /// </summary>
        private void PlaceVerticallyRandom(string word, string clue)
        {
            for (int boardIdx = 0; boardIdx < _board.Length; boardIdx++)
            {
                if (_board[boardIdx].Equals('\0'))
                {
                    bool nullAsLength = CheckIfNullAsWordLengthVertically(boardIdx, word.Length);
                    if (!nullAsLength) continue;

                    bool nearbyClearEnough = CheckIfNearbyClearEnoughVertically(boardIdx, word.Length);
                    if (!nearbyClearEnough) continue;

                    bool diagonalClearByOne = CheckIfDiagonalClearByOneVertically(boardIdx, word.Length);
                    if (!diagonalClearByOne) continue;

                    List<int> IdxsOnBoard = new();

                    for (int wordIdx = 0; wordIdx < word.Length; wordIdx++)
                    {
                        _board[boardIdx + (wordIdx * BoardSize)] = word[wordIdx];
                        IdxsOnBoard.Add(boardIdx + (wordIdx * BoardSize));
                    }

                    PlacedWordDetail.Add(new WordDetail()
                    {
                        // Index는 어떻게 할지 생각해보아야겠다.
                        Word = word,
                        Clue = clue,
                        IdxsOnBoard = IdxsOnBoard,
                        WordDirection = Direction.Vertical
                    });

                    return;
                }
            }
        }

        // Horizontal
        /// <summary>
        /// 
        /// </summary>
        private void PlaceHorizontallyRandom(string word, string clue)
        {
            for (int boardIdx = 0; boardIdx < _board.Length; boardIdx++)
            {
                // 만약 해당 cell이 null이라면 
                if (_board[boardIdx].Equals('\0'))
                {
                    bool nullAsLength = CheckIfNullAsWordLengthHorizontally(boardIdx, word.Length);
                    if (!nullAsLength) continue;

                    bool nearbyClearEnough = CheckIfNearbyClearEnoughHorizontally(boardIdx, word.Length);
                    if (!nearbyClearEnough) continue;

                    bool diagonalClearByOne = CheckIfDiagonalClearByOneHorizontally(boardIdx, word.Length);
                    if (!diagonalClearByOne) continue;

                    List<int> IdxsOnBoard = new();

                    for (int wordIdx = 0; wordIdx < word.Length; wordIdx++)
                    {
                        _board[boardIdx + wordIdx] = word[wordIdx];
                        IdxsOnBoard.Add(boardIdx + wordIdx);
                    }


                    PlacedWordDetail.Add(new WordDetail()
                    {
                        // Index는 어떻게 할지 생각해보아야겠다.
                        Word = word,
                        Clue = clue,
                        IdxsOnBoard = IdxsOnBoard,
                        WordDirection = Direction.Horizontal
                    });

                    return;
                }
            }
        }

        // PUBLIC
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WordDetail[] GetPlacedWordDetailArray()
        {
            return PlacedWordDetail.ToArray();
        }

    }
}
