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
    /// Interaction logic for SevenXSeven.xaml
    /// </summary>
    public partial class SevenXSeven : UserControl
    {
        public SevenXSeven()
        {
            InitializeComponent();

            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    var gg = new TextBox();

                    if (row == 100)
                        gg.IsHitTestVisible = false;


                    gg.Text = "Text" + row + "" + col;
                    gg.SetValue(Grid.RowProperty, row);
                    gg.SetValue(Grid.ColumnProperty, col);
                    gg.Name = "Text" + row + "" + col;
                    gg.SetValue(TagProperty, row + "" + col);

                    SevenGrid.Children.Add(gg);
                }

            }
        }
    }
}
