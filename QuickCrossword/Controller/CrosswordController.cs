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
                case Direction.Horizontal:
                    // x가 시작 좌표, y는 고정 좌표 (x ~ (x + wordLength -1))
                    bool rightCellsClear = HaveEmptyRightCells(direction, x + wordLength, y, CellsToCheckNum);
                    bool leftCellsClear = HaveEmptyLeftCells(direction, x - 1, y, CellsToCheckNum);
                    bool UpCellsClear = HaveEmptyUpCells(direction, x, y - 1, CellsToCheckNum, wordLength);
                    bool DownCellsClear = HaveEmptyDownCells(direction, x, y + 1, CellsToCheckNum, wordLength);
                    bool DiagonalCellsClear = HaveOneEmptyDiagonalCell(direction, x, y, wordLength);


                    if (rightCellsClear && leftCellsClear && UpCellsClear && DownCellsClear && DiagonalCellsClear) return true;
                    
                    return false;

                case Direction.Vertical:
                    // x가 고정 좌표, y는 시작 좌표 (y ~ (y + wordLength -1))

                    break;
            }

            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="xToCheck">The cell at JUST UP to the first letter of the word</param>
        private bool HaveOneEmptyDiagonalCell(Direction direction, int x, int y, int wordLength)
        {
            switch (direction)
            {
                case Direction.Horizontal: // Need wordLength

                    bool LeftUpDiagonalClear;
                    bool RightUpDiagonalClear;
                    bool LeftDownDiagonalClear;
                    bool RightDownDiagonalClear;

                    // Left Up
                    if ((x - 1 < 0) || (y - 1 < 0))
                        LeftUpDiagonalClear = true;
                    else
                        if (_matrix[y - 1, x - 1].Equals('\0'))
                        LeftUpDiagonalClear = true;
                    else
                        LeftUpDiagonalClear = false;

                    // Right Up
                    if ((x + wordLength >= GridSize) || (y - 1 < 0))
                        RightUpDiagonalClear = true;
                    else
                        if (_matrix[y - 1, x + wordLength].Equals('\0'))
                        RightUpDiagonalClear = true;
                    else
                        RightUpDiagonalClear = false;

                    // Left Down
                    if ((x - 1 < 0) || (y + 1 >= GridSize))
                        LeftDownDiagonalClear = true;
                    else
                        if (_matrix[y + 1, x - 1].Equals('\0'))
                        LeftDownDiagonalClear = true;
                    else
                        LeftDownDiagonalClear = false;

                    // Right Down
                    if ((x + wordLength >= GridSize) || (y + 1 >= GridSize))
                        RightDownDiagonalClear = true;
                    else
                        if (_matrix[y + 1, x + wordLength].Equals('\0'))
                        RightDownDiagonalClear = true;
                    else
                        RightDownDiagonalClear = false;


                    if (LeftUpDiagonalClear && RightUpDiagonalClear && LeftDownDiagonalClear && RightDownDiagonalClear) return true;

                    return false;

                case Direction.Vertical:
                    // x가 고정 좌표, y는 시작 좌표 (y ~ (y + wordLength -1))

                    break;
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xToCheck">The cell at JUST UP to the first letter of the word</param>
        private bool HaveEmptyUpCells(Direction direction, int xToCheck, int y, int CellsToCheckNum, int wordLength = 0)
        {
            switch (direction)
            {
                case Direction.Horizontal: // Need wordLength
                    // x가 시작 좌표, y는 고정 좌표 (x ~ (x + wordLength -1))
                    for (int col = y; col > (y - CellsToCheckNum); col--)                   //   >
                    {
                        for (int row = xToCheck; row < (xToCheck + wordLength); row++)      //   <
                        {
                            // Grid의 범위를 넘었을 경우
                            if (col < 0) return true;

                            if (!_matrix[col, row].Equals('\0')) return false;
                        }
                    }
                    return true;

                case Direction.Vertical:
                    // x가 고정 좌표, y는 시작 좌표 (y ~ (y + wordLength -1))

                    break;
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xToCheck">The cell at JUST UP to the first letter of the word</param>
        private bool HaveEmptyDownCells(Direction direction, int xToCheck, int y, int CellsToCheckNum, int wordLength = 0)
        {
            switch (direction)
            {
                case Direction.Horizontal: // Need wordLength
                    // x가 시작 좌표, y는 고정 좌표 (x ~ (x + wordLength -1))
                    for (int col = y; col < (y + CellsToCheckNum); col++)                   //   <
                    {
                        for (int row = xToCheck; row < (xToCheck + wordLength); row++)      //   <
                        {
                            // Grid의 범위를 넘었을 경우
                            if (col >= GridSize) return true;

                            if (!_matrix[col, row].Equals('\0')) return false;
                        }
                    }
                    return true;

                case Direction.Vertical:
                    // x가 고정 좌표, y는 시작 좌표 (y ~ (y + wordLength -1))

                    break;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xToCheck">The cell at JUST RIGHT to the word</param>
        private bool HaveEmptyRightCells(Direction direction, int xToCheck, int y, int CellsToCheckNum, int wordLength = 0)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    // x가 시작 좌표, y는 고정 좌표 (x ~ (x + wordLength -1))
                    for (int i = xToCheck; i <= (xToCheck + CellsToCheckNum); i++)
                    {
                        // Grid의 범위를 넘었을 경우
                        if (i >= GridSize) return true;

                        if (!_matrix[y, i].Equals('\0')) return false;
                    }
                    return true;

                case Direction.Vertical:
                    // x가 고정 좌표, y는 시작 좌표 (y ~ (y + wordLength -1))

                    break;
            }

            return true;
        }

        /// <summary>
        /// 이거 좀 위태하다
        /// </summary>
        /// <param name="xToCheck">The cell at JUST LEFT(x-1) to the word</param>
        private bool HaveEmptyLeftCells(Direction direction, int xToCheck, int y, int CellsToCheckNum, int wordLength = 0)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    // x가 시작 좌표, y는 고정 좌표 ((x-1-(1+CellsToCheckNum)) ~ (x-1))
                    for (int i = xToCheck; i >= (xToCheck - CellsToCheckNum); i--)
                    {
                        // Grid의 범위를 넘었을 경우
                        if (i < 0) return true;

                        if (!_matrix[y, i].Equals('\0')) return false;
                    }
                    return true;

                case Direction.Vertical:
                    // x가 고정 좌표, y는 시작 좌표 (y ~ (y + wordLength -1))

                    break;
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
            HorizontallyPutWordByItself(word);
            //if (cnt % 2 == 0)
            //{
            //    HorizontallyPutWordByItself(word);
            //}
            //else
            //{
            //    VerticallyPutWordByItself(word);
            //}
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
