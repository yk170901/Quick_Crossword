using QuickCrossword.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickCrossword.Model.Db;

namespace QuickCrossword.Controller
{
    public class WordAndClueController
    {
        List<WordAndClue> wordAndClueList = new();

        private void LoadWordAndClueList()
        {
            wordAndClueList = SqliteDataAccess.LoadWordAndClue();
        }

    }
}
