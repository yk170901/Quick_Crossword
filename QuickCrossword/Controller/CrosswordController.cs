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
        GridMode gridSize;

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
        public char[,]? GetMatrix()
        {
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

            PutWordsInMatrix(WordAndClueArray, 10);
        }

        private void HorizontallyPutWordByItself(string word, int size)
        {
            bool NearbyClear;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x + word.Length < size; x = x + word.Length)
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

                    NearbyClear = CheckIfNearbyClear();

                    if (!NearbyClear) continue;

                    // place word
                    for (int i = 0, test = x; i < word.Length; i++, test++)
                        _matrix[y, test] = word[i];

                    goto DONE;
                }
            }
        DONE:;
        }

        private void VerticallyPutWordByItself(string word, int size)
        {
            bool NearbyClear;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y + word.Length < size; y = y + word.Length)
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

                    NearbyClear = CheckIfNearbyClear();

                    if (!NearbyClear) continue;

                    // place word
                    for (int i = 0, test = y; i < word.Length; i++, test++)
                        _matrix[test, x] = word[i];

                    goto DONE;
                }
            }
        DONE:;
        }

        /// <summary>
        /// Put random words of WordAndClue into the matrix to see if they generates whole valid matrix with one another
        /// </summary>
        private void PutWordsInMatrix(WordAndClue[] WordAndClueArray, int size = 10)
        {
            _matrix = new char[size, size];
            Direction direction;
            // int x, y;

            bool NearbyClear = false;
            bool FitInGrid;


            foreach (WordAndClue WAC in WordAndClueArray)
            {
                string? word = WAC.Word;

                int IsolatedWordsNum = 0; // 다른 언어와 엮여서 들어간 게 아니라 빈 공간에 들어간 언어의 개수
                bool CanPlace = false;
                bool IsMatchingChar = false;

                // Horizontal try
                // 이게 fitInGrid 메소드인가


//                HorizontallyPutWordByItself(word, size);
                VerticallyPutWordByItself(word, size);

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

                if(dd % size == 0)
                    Debug.WriteLine("");
            }

        }

        private WordDetail[] SavePlacedWordDetail()
        {
            List<WordDetail> PlacedWordDetail = new();

            return PlacedWordDetail.ToArray();
        }

        private bool CheckIfNearbyClear()
        {
            return true;
        }

        private bool CheckIfFitInGrid()
        {
            return true;
        }


    }
}
