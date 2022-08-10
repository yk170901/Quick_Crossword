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
        private char[,]? _matrix;
        private static int GridSize;

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
        public char[,]? GetMatrix(GridMode gridMode = GridMode.TenXTen)
        {
            GridSize = (int)gridMode;
            CreateMatrix();
            return _matrix;
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
        private void CreateMatrix()
        {
            // Will contain tons of methods
            WordAndClue[] WordAndClueArray = GetWACFromDb();

            PutWordsInMatrix(WordAndClueArray);
        }

        /// <summary>
        /// Check if at least 2 / 3 / 4 cells around a word is clear
        /// </summary>
        private bool CheckIfWideRangeClear(Direction direction, int x, int y, int wordLength)
        {
            int CellsToCheckNum = GridSize / 3;
            switch (direction)
            {
                // 이거 그냥 HaveEmptyRightCells(direction, x, y, CellsToCheckNum); 처럼 x y  변환 안 상태로 넘기고, 받은 쪽에서 로직으로 어케 하는 걸로 했으면 굳이 Switch문 안 써도 됐었다.
                case Direction.Horizontal:
                    // x가 시작 좌표, y는 고정 좌표 (x ~ (x + wordLength -1))
                    bool rightCellsClear_Hor = HaveEmptyRightCells(direction, x + wordLength, y, CellsToCheckNum);
                    bool leftCellsClear_Hor = HaveEmptyLeftCells(direction, x - 1, y, CellsToCheckNum);
                    bool UpCellsClear_Hor = HaveEmptyUpCells(direction, x, y - 1, CellsToCheckNum, wordLength);
                    bool DownCellsClear_Hor = HaveEmptyDownCells(direction, x, y + 1, CellsToCheckNum, wordLength);
                    bool DiagonalCellsClear_Hor = HaveOneEmptyDiagonalCell(direction, x, y, wordLength);


                    if (rightCellsClear_Hor && leftCellsClear_Hor && UpCellsClear_Hor && DownCellsClear_Hor && DiagonalCellsClear_Hor) return true;
                    
                    return false;

                case Direction.Vertical:
                    // x가 고정 좌표, y는 시작 좌표 (y ~ (y + wordLength -1))
                    bool rightCellsClear_Ver = HaveEmptyRightCells(direction, x + 1, y, CellsToCheckNum, wordLength);
                    bool leftCellsClear_Ver = HaveEmptyLeftCells(direction, x - 1, y, CellsToCheckNum, wordLength);
                    bool UpCellsClear_Ver = HaveEmptyUpCells(direction, x, y - 1, CellsToCheckNum, wordLength);
                    bool DownCellsClear_Ver = HaveEmptyDownCells(direction, x, y + 1, CellsToCheckNum, wordLength);
                    bool DiagonalCellsClear_Ver = HaveOneEmptyDiagonalCell(direction, x, y, wordLength);


                    if (rightCellsClear_Ver && leftCellsClear_Ver && UpCellsClear_Ver && DownCellsClear_Ver && DiagonalCellsClear_Ver) return true;

                    return false;

            }

            return true;
        }


        private bool HaveOneEmptyDiagonalCell(Direction direction, int x, int y, int wordLength)
        {
            switch (direction)
            {
                case Direction.Horizontal:

                    bool LeftUpDiagonalClear_Hor;
                    bool RightUpDiagonalClear_Hor;
                    bool LeftDownDiagonalClear_Hor;
                    bool RightDownDiagonalClear_Hor;

                    // Left Up
                    if ((x - 1 < 0) || (y - 1 < 0))
                    {
                        LeftUpDiagonalClear_Hor = true;
                    }
                    else
                    {
                        if (_matrix[y - 1, x - 1].Equals('\0'))
                        {
                            LeftUpDiagonalClear_Hor = true;
                        }
                        else
                        {
                            LeftUpDiagonalClear_Hor = false;
                        }
                    }

                    // Right Up
                    if ((x + wordLength >= GridSize) || (y - 1 < 0))
                    {
                        RightUpDiagonalClear_Hor = true;
                    }
                    else
                    {
                        if (_matrix[y - 1, x + wordLength].Equals('\0'))
                        {
                            RightUpDiagonalClear_Hor = true;
                        }
                        else
                        {
                            RightUpDiagonalClear_Hor = false;
                        }
                    }

                    // Left Down
                    if ((x - 1 < 0) || (y + 1 >= GridSize))
                    {
                        LeftDownDiagonalClear_Hor = true;
                    }
                    else
                    {
                        if (_matrix[y + 1, x - 1].Equals('\0'))
                        {
                            LeftDownDiagonalClear_Hor = true;
                        }
                        else
                        {
                            LeftDownDiagonalClear_Hor = false;
                        }
                    }

                    // Right Down
                    if ((x + wordLength >= GridSize) || (y + 1 >= GridSize))
                    {
                        RightDownDiagonalClear_Hor = true;
                    }
                    else
                    {
                        if (_matrix[y + 1, x + wordLength].Equals('\0'))
                        {
                            RightDownDiagonalClear_Hor = true;
                        }
                        else
                        {
                            RightDownDiagonalClear_Hor = false;
                        }
                    }

                    if (LeftUpDiagonalClear_Hor && RightUpDiagonalClear_Hor && LeftDownDiagonalClear_Hor && RightDownDiagonalClear_Hor) return true;

                    return false;

                case Direction.Vertical:
                    bool LeftUpDiagonalClear_Ver;
                    bool RightUpDiagonalClear_Ver;
                    bool LeftDownDiagonalClear_Ver;
                    bool RightDownDiagonalClear_Ver;

                    // Left Up
                    if ((x - 1 < 0) || (y - wordLength < 0))
                    {
                        LeftUpDiagonalClear_Ver = true;
                    }
                    else
                    {
                        if (_matrix[y - wordLength -1, x - 1].Equals('\0'))
                        {
                            LeftUpDiagonalClear_Ver = true;
                        }
                        else
                        {
                            LeftUpDiagonalClear_Ver = false;
                        }
                    }

                    // Right Up
                    if ((x + 1 >= GridSize) || (y - wordLength < 0))
                    {
                        RightUpDiagonalClear_Ver = true;
                    }
                    else
                    {
                        if (_matrix[y - wordLength -1, x + 1].Equals('\0'))
                        {
                            RightUpDiagonalClear_Ver = true;
                        }
                        else
                        {
                            RightUpDiagonalClear_Ver = false;
                        }
                    }

                    // Left Down
                    if ((x - 1 < 0) || (y + 1 >= GridSize))
                    {
                        LeftDownDiagonalClear_Ver = true;
                    }
                    else
                    {
                        if (_matrix[y + 1, x - 1].Equals('\0'))
                        {
                            LeftDownDiagonalClear_Ver = true;
                        }
                        else
                        {
                            LeftDownDiagonalClear_Ver = false;
                        }
                    }

                    // Right Down
                    if ((x + 1 >= GridSize) || (y + 1 >= GridSize))
                    {
                        RightDownDiagonalClear_Ver = true;
                    }
                    else
                    {
                        if (_matrix[y + 1, x + 1].Equals('\0'))
                        {
                            RightDownDiagonalClear_Ver = true;
                        }
                        else
                        {
                            RightDownDiagonalClear_Ver = false;
                        }
                    }


                    if (LeftUpDiagonalClear_Ver && RightUpDiagonalClear_Ver && LeftDownDiagonalClear_Ver && RightDownDiagonalClear_Ver) return true;

                    return false;
            }

            return true;
        }

        private bool HaveEmptyUpCells(Direction direction, int x, int y, int CellsToCheckNum, int wordLength = 0)
        {
            switch (direction)
            {
                case Direction.Horizontal: // Need wordLength
                    // x가 시작 좌표, y는 고정 좌표 (x ~ (x + wordLength -1))
                    for (int col = y; col > (y - CellsToCheckNum); col--)     //   >
                    {
                        for (int row = x; row < (x + wordLength); row++)      //   <
                        {
                            // Grid의 범위를 넘었을 경우
                            if (col < 0) return true;

                            if (!_matrix[col, row].Equals('\0')) return false;
                        }
                    }
                    return true;

                case Direction.Vertical:
                    for (int i = y; i > (y - CellsToCheckNum); i--)
                    {
                        // Grid의 범위를 넘었을 경우
                        if (i < 0) return true;

                        if (!_matrix[i, x].Equals('\0')) return false;
                    }
                    return true;
            }

            return true;
        }


        private bool HaveEmptyDownCells(Direction direction, int x, int y, int CellsToCheckNum, int wordLength = 0)
        {
            switch (direction)
            {
                case Direction.Horizontal: // Need wordLength
                    // x가 시작 좌표, y는 고정 좌표 (x ~ (x + wordLength -1))
                    for (int col = y; col < (y + CellsToCheckNum); col++)     //   <
                    {
                        for (int row = x; row < (x + wordLength); row++)      //   <
                        {
                            // Grid의 범위를 넘었을 경우
                            if (col >= GridSize) return true;

                            if (!_matrix[col, row].Equals('\0')) return false;
                        }
                    }
                    return true;

                case Direction.Vertical:
                    for (int i = y; i < (y + CellsToCheckNum); i++)
                    {
                        // Grid의 범위를 넘었을 경우
                        if (i >= GridSize) return true;

                        if (!_matrix[i, x].Equals('\0')) return false;
                    }
                    return true;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xToCheck">The cell at JUST RIGHT to the word</param>
        private bool HaveEmptyRightCells(Direction direction, int x, int y, int CellsToCheckNum, int wordLength = 0)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    // x가 시작 좌표, y는 고정 좌표 (x ~ (x + wordLength -1))
                    for (int i = x; i <= (x + CellsToCheckNum); i++)
                    {
                        // Grid의 범위를 넘었을 경우
                        if (i >= GridSize) return true;

                        if (!_matrix[y, i].Equals('\0')) return false;
                    }
                    return true;

                case Direction.Vertical:
                    // x가 고정 좌표, y는 시작 좌표 (y ~ (y + wordLength -1))
                    for (int row = x; row < (x + CellsToCheckNum); row++)
                    {
                        // Grid의 범위를 넘었을 경우
                        if (row >= GridSize) return true;

                        for (int col = y; col < (y + wordLength); col++)
                        {
                            if (!_matrix[col, row].Equals('\0')) return false;
                        }
                    }
                    return true;
            }

            return true;
        }

        /// <summary>
        /// 이거 좀 위태하다
        /// </summary>
        /// <param name="xToCheck">The cell at JUST LEFT(x-1) to the word</param>
        private bool HaveEmptyLeftCells(Direction direction, int x, int y, int CellsToCheckNum, int wordLength = 0)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    // x가 시작 좌표, y는 고정 좌표 ((x-1-(1+CellsToCheckNum)) ~ (x-1))
                    for (int i = x; i >= (x - CellsToCheckNum); i--)
                    {
                        // Grid의 범위를 넘었을 경우
                        if (i < 0) return true;

                        if (!_matrix[y, i].Equals('\0')) return false;
                    }
                    return true;

                case Direction.Vertical:
                    // x가 고정 좌표, y는 시작 좌표 (y ~ (y + wordLength -1))
                    for (int row = x; row > (x - CellsToCheckNum); row--)
                    {
                        // Grid의 범위를 넘었을 경우
                        if (row < 0) return true;

                        for (int col = y; col < (y + wordLength); col++)
                        {
                            if (!_matrix[col, row].Equals('\0')) return false;
                        }
                    }
                    return true;
            }
            return true;
        }


        /// <summary>
        /// Check if cells around the word by one are clear, so that the word can be placed and linked to another word
        /// </summary>
        private bool CheckIfNearbyClear(Direction direction, int x, int y, int wordLength)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    // x가 시작 좌표, y는 고정 좌표 (x ~ (x + wordLength -1))

                    break;

                case Direction.Vertical:
                    // x가 고정 좌표, y는 시작 좌표 (y ~ (y + wordLength -1))

                    break;
            }

            return true;
        }

        /// <summary>
        /// To initialize crossword word placement, horizontally place a word by itself with ENOUGH empty cells around it
        /// </summary>
        private void HorizontallyPutWordByItself(string word)
        {
            bool NearbyClear;

            for (int y = 0; y < GridSize; y++)
            {
                for (int x = 0; x + word.Length < GridSize; x = x + word.Length)
                {
                    bool Placalble = true;

                    for (int test = x; test < x + word.Length; test++)
                    {
                        if (_matrix[y, test] != '\0')
                        {
                            Placalble = false;
                            break;
                        }
                    }

                    if (!Placalble) continue;

                    NearbyClear = CheckIfWideRangeClear(Direction.Horizontal, x, y, word.Length);
                    Debug.WriteLine(NearbyClear);
                    if (!NearbyClear) continue;

                    // place word
                    for (int i = 0, test = x; i < word.Length; i++, test++)
                        _matrix[y, test] = word[i];

                    goto DONE;
                }
            }
        DONE:;
        }

        /// <summary>
        /// To initialize crossword word placement, vertically place a word by itself with ENOUGH empty cells around it
        /// </summary>
        private void VerticallyPutWordByItself(string word)
        {
            bool NearbyClear;

            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y + word.Length < GridSize; y = y + word.Length)
                {
                    bool Placalble = true;

                    for (int test = y; test < y + word.Length; test++)
                    {
                        if (_matrix[test, x] != '\0')
                        {
                            Placalble = false;
                            break;
                        }
                    }

                    if (!Placalble) continue;

                    NearbyClear = CheckIfWideRangeClear(Direction.Vertical, x, y, word.Length);

                    if (!NearbyClear) continue;

                    // place word
                    for (int i = 0, test = y; i < word.Length; i++, test++)
                        _matrix[test, x] = word[i];

                    goto DONE;
                }
            }
        DONE:;
        }


        private void SetWordsBeforeLinking(string word, int cnt)
        {
            if (cnt % 2 == 0)
            {
                HorizontallyPutWordByItself(word);
            }
            else
            {
                VerticallyPutWordByItself(word);
            }
        }


        /// <summary>
        /// Put random words of WordAndClue into the matrix to see if they generates whole valid matrix with one another
        /// </summary>
        private void PutWordsInMatrix(WordAndClue[] WordAndClueArray)
        {
            _matrix = new char[GridSize, GridSize];
            Direction direction;
            // int x, y;

            bool NearbyClear = false;
            bool FitInGrid;

            int cnt = -1;
            foreach (WordAndClue WAC in WordAndClueArray)
            {
                cnt++;

                string? word = WAC.Word;

                bool CanPlace = false;
                bool IsMatchingChar = false;

                // 이걸 초반 몇몇 단어 뿐만이 아니라 아예 link가 안 된 단어들은 전부 이렇게 하게 하기
                if (cnt < GridSize - 3)
                    SetWordsBeforeLinking(word, cnt);

                //for (int charAt = 0; charAt < word.Length; charAt++)
                //{
                //    // 워드 매치 확인 로직. 나중에 하기.
                //    //for (int i = 0; i < 7; i++)
                //    //{
                //    //    for (int j = 0; j < 7; j++)
                //    //    {
                //    //        // 가정 1 : 같은 단어가 나왔다

                //    //        if (_matrix[i, j].Equals(word[charAt]))
                //    //        {
                //    //            NearbyClear = CheckIfNearbyClear();
                //    //            FitInGrid = CheckIfFitInGrid();

                //    //            if (!NearbyClear) continue;
                //    //            if (!FitInGrid) continue;

                //    //            // IsMatchingChar = true;
                //    //        }


                //    //    }
                //    //}

                //    // 일치하는 Char가 없다면
                //    if (!IsMatchingChar)
                //    {


                //    }


                //}
            }

            // grid 형태로 matrix를 보기
            int dd = 0;
            foreach (var g in _matrix)
            {
                if (g == '\0')
                    Debug.Write("()");
                else
                    Debug.Write(g);

                dd++;

                if(dd % GridSize == 0)
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
