using QuickCrossword.Model;
using QuickCrossword.Model.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCrossword.Controller
{
    public class CrosswordController
    {
        private char[,]? _matrix;

        /// <summary>
        /// PUBLIC
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

            foreach(WordAndClue wordAndClue in wordAndClueList)
            {

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
