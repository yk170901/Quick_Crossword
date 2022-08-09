using QuickCrossword.Model;
using QuickCrossword.Model.Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuickCrossword.View
{
    /// <summary>
    /// Interaction logic for Crossword.xaml
    /// </summary>
    public partial class Crossword : UserControl
    {
        // Matrix to Datagrid
        // https://stackoverflow.com/questions/28018974/binding-matrix-arrays-to-wpf-datagrid

        List<WordAndClue> wordAndClueList = new();

        public Crossword()
        {
            InitializeComponent();
            // DataGrid Approach

            List<FiveGridModel> lsts = new List<FiveGridModel>();

            // 하나의 row들을 집어넣는 것
            lsts.Add(new FiveGridModel() { First = '#', Second = '#', Third = '#', Fourth = '#', Fifth = '임' });
            lsts.Add(new FiveGridModel() { First = '#', Second = '#', Third = '#', Fourth = '#', Fifth = '산' });
            lsts.Add(new FiveGridModel() { First = '#', Second = '잉', Third = '꼬', Fourth = '부', Fifth = '부' });
            lsts.Add(new FiveGridModel() { First = '핑', Second = '크', Third = '#', Fourth = '금', Fifth = '#' });
            lsts.Add(new FiveGridModel() { First = '#', Second = '병', Third = '세', Fourth = '#', Fifth = '#' });

            CrosswordDatagird.Items.Clear();

            CrosswordDatagird.ItemsSource = lsts.ToArray();
            CrosswordDatagird.ColumnWidth = 60;
            CrosswordDatagird.RowHeight = 60;

            // LoadWordAndClueList();
        }

        private void LoadWordAndClueList()
        {
            wordAndClueList = SqliteDataAccess.LoadWordAndClue();

            foreach (var dd in wordAndClueList)
            {
                Debug.WriteLine(dd.Id);
                Debug.WriteLine(dd.Word);
                Debug.WriteLine(dd.Clue);
            }
        }
    }
}
