using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCrossword.Model.Db
{
    public class SqliteDataAccess
    {
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public static List<WordAndClue> LoadWordAndClue()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = conn.Query<WordAndClue>("SELECT * FROM WORDANDCLUE", new DynamicParameters());
                return output.ToList();
            }
        }
    }
}
