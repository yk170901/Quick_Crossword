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
        private readonly int BoardTotalSize = BoardSize * BoardSize;
        private static List<WordDetail> PlacedWordDetail;

        private static CrosswordController _instance = null;
        public static CrosswordController Instance()
        {
            if (_instance == null)
                _instance = new CrosswordController();
            return _instance;
        }

        // PUBLIC
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public WordDetail[] GetPlacedWordDetailArray()
        {
            var sortedList = PlacedWordDetail.OrderBy(o => o.Index);
            return sortedList.ToArray();
        }

        // PUBLIC, IMPORTANT
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Completed matrix of crossword</returns>
        public char[] GetBoard(BoardMode boardMode = BoardMode.TenXTen)
        {
            BoardSize = (int)boardMode;
            _board = new char[BoardSize * BoardSize];
            PlacedWordDetail = new();

            WordAndClue[] WordAndClueArray = GetWACFromDb();

            PutWordsOnBoard(WordAndClueArray);
            GetRidOfIsolatedWords();

            byte MinimumWordsOnBoard = 0;
            switch (boardMode)
            {
                case BoardMode.FiveXFive:
                    MinimumWordsOnBoard = 6;
                    break;
                case BoardMode.SevenXSeven:
                    MinimumWordsOnBoard = 10;
                    break;
                case BoardMode.TenXTen:
                    MinimumWordsOnBoard = 18;
                    break;
            }

            if(PlacedWordDetail.Count < MinimumWordsOnBoard) GetBoard(boardMode); // 자신 부르기

            ClearIdxForLabeling();
            LabelIndex();

            return _board;
        }

        private void ClearIdxForLabeling()
        {
            foreach(var item in PlacedWordDetail)
            {
                item.Index = -1;
            }
        }

        private void LabelIndex()
        {
            short label = 1;

            for (int idx = 0; idx < _board.Length; idx++)
            {
                if (_board[idx].Equals('\0')) continue;

                var WordsAtIdx = PlacedWordDetail.FindAll(o => o.IdxsOnBoard.Contains(idx));

                bool PlacedIdx = false;
                foreach (var word in WordsAtIdx)
                {
                    if (!word.Index.Equals(-1)) continue;
                    word.Index = label;

                    PlacedIdx = true;
                }

                if(PlacedIdx) label++;
            }
        }

        private void GetRidOfIsolatedWords()
        {
            List<WordDetail> DeleteList = new ();

            foreach(var item in PlacedWordDetail)
            {
                if (!item.Isolated) continue;

                Debug.WriteLine(item.Word + "IS ISOLATED");

                foreach (var idx in item.IdxsOnBoard)
                {
                    _board[idx] = '\0';
                }
                DeleteList.Add(item);
            }

            foreach(var item in DeleteList)
            {
                PlacedWordDetail.Remove(item);
            }
        }

        /// <summary>
        /// Retrieve an random array of 30 words and clues from Sqlite DB
        /// </summary>
        private WordAndClue[] GetWACFromDb()
        {
            Random rnd = new Random();
            var WordAndClueList_raw = SqliteDataAccess.LoadWordAndClue();

            List<WordAndClue> WordAndClueList = new();
            // Take 150 random numbers in range of 4 to 214 with NO duplicate
            var randomNumbers = Enumerable.Range(4, 214).OrderBy(x => rnd.Next()).Take(150).ToArray();

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

        // Vertical
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
        
        private int GetMatchingCharIndex(char wordChar)
        {
            for(int boardIdx = 0; boardIdx < _board.Length; boardIdx++)
            {
                if (_board[boardIdx].Equals(wordChar))
                {
                    return boardIdx;
                }
            }
            return -1;
        }

        private bool CheckIfNearbyClearVertically(int[] wordToLinkIdxArray, int matchingCharIdx, string word, int wordIdx)
        {
            // 만약 단어를 링크하여 넣는다면 그 새로 넣은 단어의 첫글자가 들어갈 자리
            int Start = matchingCharIdx - (wordIdx * BoardSize);

            List<int> MyPlace = new();

            for (int i = 0; i < word.Length; i++)
            {
                // 첫 글자일 때
                if (i == 0)
                {
                    // Up
                    bool UpClear = false;
                    int Up = Start - BoardSize;
                    if (wordToLinkIdxArray.Contains(Up)
                        || Up < 0
                        || _board[Up].Equals('\0')) UpClear = true;

                    if (!UpClear) return false;
                }
                // 마지막 글자일 때
                else if (i == (word.Length - 1))
                {
                    // Down 
                    bool DownClear = false;
                    int Down = Start + ((i + 1)* BoardSize);
                    if (wordToLinkIdxArray.Contains(Down)
                        || Down >= BoardSize * BoardSize
                        || _board[Down].Equals('\0')) DownClear = true;

                    if (!DownClear) return false;
                }

                // Left
                bool LeftClear = false;
                int Left = Start + (i * BoardSize) - 1;
                if (wordToLinkIdxArray.Contains(Left)
                    || MyPlace.Contains(Left)
                    || Left < 0
                    || _board[Left].Equals('\0')) LeftClear = true;

                if (!LeftClear) return false;

                // Right
                bool RightClear = false;
                int Right = Start + (i * BoardSize) + 1;
                if (wordToLinkIdxArray.Contains(Right)
                    || Right >= BoardSize * BoardSize
                    || _board[Right].Equals('\0')) RightClear = true;

                if (!RightClear) return false;

                MyPlace.Add(Start);
            }
            return true;

        }

        private bool CheckIfNearbyClearHorizontally(int[] wordToLinkIdxArray, int matchingCharIdx, string word, int wordIdx)
        {
            // 만약 단어를 링크하여 넣는다면 그 새로 넣은 단어의 첫글자가 들어갈 자리
            int Start = matchingCharIdx - wordIdx;

            for (int i = 0; i < word.Length; i++)
            {
                // 첫 글자일 때
                if (i == 0)
                {
                    // Left
                    bool LeftClear = false;
                    int Left = Start - 1;
                    if (wordToLinkIdxArray.Contains(Left)
                        || Left < 0
                        || _board[Left].Equals('\0')) LeftClear = true;

                    if (!LeftClear) return false;
                }
                // 마지막 글자일 때
                else if (i == (word.Length - 1))
                {
                    // Right
                    bool RightClear = false;
                    int Right = Start + i + 1;
                    if (wordToLinkIdxArray.Contains(Right)
                        || Right >= BoardSize * BoardSize
                        || _board[Right].Equals('\0')) RightClear = true;

                    if (!RightClear) return false;
                }

                // Up
                bool UpClear = false;
                int Up = Start + i - BoardSize;
                if (wordToLinkIdxArray.Contains(Up)
                    || Up < 0
                    || _board[Up].Equals('\0')) UpClear = true;

                if (!UpClear) return false;

                // Down 
                bool DownClear = false;
                int Down = Start + i + BoardSize;
                if (wordToLinkIdxArray.Contains(Down)
                    || Down >= BoardSize * BoardSize
                    || _board[Down].Equals('\0')) DownClear = true;

                if (!DownClear) return false;

            }
            return true;
        }

        private bool CheckIfFitInBoardVertically(int matchingCharIdx, int wordLength, int wordIdx)
        {
            // 만약 단어를 링크하여 넣는다면 그 새로 넣은 단어의 첫글자가 들어갈 자리
            int Start = matchingCharIdx - (wordIdx * BoardSize);
            bool ExceededGrid;

            for (int i = 0; i < wordLength; i++)
            {
                int CurrentIdx = Start + (i * BoardSize);

                if (CurrentIdx >= BoardSize * BoardSize
                    || CurrentIdx < 0)
                {
                    ExceededGrid = true;
                }
                else
                {
                    ExceededGrid = false;
                }

                if (ExceededGrid) return false; // 단어가 그리드를 넘어가면 false 반환
                else if (CurrentIdx.Equals(matchingCharIdx)) continue; // 매칭 단어면 냅둠

                if (!_board[CurrentIdx].Equals('\0')) // 해당 공간이 비어있지 않으면
                {
                    var WordInWay = PlacedWordDetail.FirstOrDefault(o => o.IdxsOnBoard.Contains(CurrentIdx));

                    if (!WordInWay.Isolated) return false; // 다른 단어와 연결된 단어가 자리를 차지하는 거면 얌전히 return false

                    // 아니면 그 단어 지우고 진행
                    foreach (var idxToDelete in WordInWay.IdxsOnBoard)
                    {
                        _board[idxToDelete] = '\0';
                    }

                    PlacedWordDetail.Remove(WordInWay);
                }
            }

            return true;
        }

        private bool CheckIfFitInBoardHorizontally(int matchingCharIdx, int wordLength, int wordIdx)
        {
            // 만약 단어를 링크하여 넣는다면 그 새로 넣은 단어의 첫글자가 들어갈 자리
            int Start = matchingCharIdx - wordIdx;
            bool ExceededGrid;

            for (int i = 0; i < wordLength; i++)
            {
                int CurrentIdx = Start + i;

                if (CurrentIdx % BoardSize == 0
                    || CurrentIdx >= BoardSize * BoardSize
                    || CurrentIdx < 0)
                {
                    ExceededGrid = true;
                }
                else
                {
                    ExceededGrid = false;
                }

                if (ExceededGrid) return false; // 단어가 그리드를 넘어가면 false 반환
                else if (CurrentIdx.Equals(matchingCharIdx)) continue; // 매칭 단어면 냅둠

                if (!_board[CurrentIdx].Equals('\0'))
                {
                    var WordInWay = PlacedWordDetail.FirstOrDefault(o => o.IdxsOnBoard.Contains(CurrentIdx));

                    if (!WordInWay.Isolated) return false; // 다른 단어와 연결된 단어가 자리를 차지하는 거면 얌전히 return false

                    // 아니면 그 단어 지우고 진행
                    foreach (var idxToDelete in WordInWay.IdxsOnBoard)
                    {
                        _board[idxToDelete] = '\0';
                    }

                    PlacedWordDetail.Remove(WordInWay);
                }
            }
            return true;
        }

        private bool CanLinkToWordOnBoard(string word, string clue)
        {
            // 글자 단어 하나씩 확인
            for (int i = 0; i < word.Length; i++)
            {
                int MatchingCharIdx = GetMatchingCharIndex(word[i]);

                // 일치하는 글자가 없을 경우
                if (MatchingCharIdx == -1) continue;

                // 이미 그 글자에 링크된 다른 글자가 또 있을 경우
                if(PlacedWordDetail.Count(o => o.IdxsOnBoard.Contains(MatchingCharIdx)) > 1) continue;

                var WordToLinkTo = PlacedWordDetail.Find(o => o.IdxsOnBoard.Contains(MatchingCharIdx));

                switch (WordToLinkTo?.WordDirection)
                {
                    case Direction.Horizontal:

                        if (!CheckIfFitInBoardVertically(MatchingCharIdx, word.Length, i)) continue;
                        if (!CheckIfNearbyClearVertically(WordToLinkTo.IdxsOnBoard.ToArray(), MatchingCharIdx, word, i)) continue;

                        List<int> VerticalIdxsOnBoard = new();

                        int VerticalStart = MatchingCharIdx - (i * BoardSize);

                        for (int idx = 0; idx < word.Length; idx++)
                        {
                            _board[VerticalStart + (idx * BoardSize)] = word[idx];
                            VerticalIdxsOnBoard.Add(VerticalStart + (idx * BoardSize));
                        }

                        PlacedWordDetail.Add(new WordDetail()
                        {
                            // Index는 어떻게 할지 생각해보아야겠다.
                            Word = word,
                            Clue = clue,
                            IdxsOnBoard = VerticalIdxsOnBoard,
                            WordDirection = Direction.Vertical,

                            Isolated = false
                        });

                        PlacedWordDetail.Find(o => o.Word == WordToLinkTo.Word).Isolated = false;

                        return true;

                    case Direction.Vertical:

                        if (!CheckIfFitInBoardHorizontally(MatchingCharIdx, word.Length, i)) continue;

                        if (!CheckIfNearbyClearHorizontally(WordToLinkTo.IdxsOnBoard.ToArray(), MatchingCharIdx, word, i)) continue;

                        List<int> HorizontalIdxsOnBoard = new();

                        int HorizontalStart = MatchingCharIdx - i;

                        for (int idx = 0; idx < word.Length; idx++)
                        {
                            _board[HorizontalStart + idx] = word[idx];
                            HorizontalIdxsOnBoard.Add(HorizontalStart + idx);
                        }

                        PlacedWordDetail.Add(new WordDetail()
                        {
                            // Index는 어떻게 할지 생각해보아야겠다.
                            Word = word,
                            Clue = clue,
                            IdxsOnBoard = HorizontalIdxsOnBoard,
                            WordDirection = Direction.Horizontal,

                            Isolated = false
                        });

                        PlacedWordDetail.Find(o => o.Word == WordToLinkTo.Word).Isolated = false;

                        return true;
                }

            }

            return false;
        }


        /// <summary>
        /// Put random words from WordAndClue into the _board to see if they generates whole valid a board of 1 dimension with one another
        /// </summary>
        private void PutWordsOnBoard(WordAndClue[] WordAndClueArray)
        {
            int cnt = -1;
            foreach (WordAndClue WAC in WordAndClueArray)
            {
                if(WAC == null)
                {
                    MessageBox.Show("Unexpected Error Occured. WAC is null");
                    return;
                }

                cnt++;

                string word = WAC.Word;

                bool LinkedToWordOnBoard = CanLinkToWordOnBoard(word, WAC.Clue);

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
                        WordDirection = Direction.Vertical,

                        Isolated = true
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
                        WordDirection = Direction.Horizontal,

                        Isolated = true
                    });

                    return;
                }
            }
        }


    }
}
