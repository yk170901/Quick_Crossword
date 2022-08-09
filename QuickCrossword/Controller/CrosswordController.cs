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
        /// Loads data of words and clues from Sqlite DB
        /// </summary>
        private void LoadWords()
        {
            var wordAndClueList = SqliteDataAccess.LoadWordAndClue();

            Random rnd = new Random();

            // Take 50 random numbers in range of 4 to 214 with NO duplicate
            var randomNumbers = Enumerable.Range(4, 214).OrderBy(x => rnd.Next()).Take(50).ToArray();

            foreach (int rndNum in randomNumbers)
            {
                var test = wordAndClueList.Select(o => o.Id == rndNum);

                Debug.WriteLine(test);
            }

        }

        // IMPORTANT
        /// <summary>
        /// Create matrix of crossword based on data from Sqlite DB
        /// </summary>
        private void CreateMatrix()
        {
            // Will contain tons of methods
        }


    }
}
