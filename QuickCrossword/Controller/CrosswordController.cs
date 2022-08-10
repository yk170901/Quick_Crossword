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


        // PUBLIC
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Completed matrix of crossword</returns>
        public char[,]? GetMatrix()
        {



            return null;
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


        }

        /// <summary>
        /// Put random words of WordAndClue into the matrix to see if they generates whole valid matrix with one another
        /// </summary>
        private void PutWordsInMatrix(WordAndClue[] WordAndClueArray)
        {
            List<WordAndClue> OnMatrix = new();

            // 내가

        }


    }
}
