using QuickCrossword.Model;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Clue.xaml
    /// </summary>
    public partial class Clue : UserControl
    {
        public Clue()
        {
            InitializeComponent();
        }

        public void GetClueListView(WordDetail[] WordDetailArray)
        {
            ClueListView.Items.Clear();

            foreach (var item in WordDetailArray)
            {
                ClueListView.Items.Add(new ListViewItem() { Content = item.Index + " - " + item.Clue });
            }
        }
    }
}
