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
    /// Interaction logic for testGrid.xaml
    /// </summary>
    public partial class testGrid : UserControl
    {

        public testGrid()
        {
            InitializeComponent();

            for(int row = 0; row < 7; row++)
            {
                for(int col = 0; col < 7; col++)
                {
                    var gg = new TextBox();

                    if (row % 2 == 0)
                        gg.IsHitTestVisible = false;

                    gg.TextChanged += new TextChangedEventHandler(this.Text10);

                    gg.Text = "Text" + row + "" + col;
                    gg.SetValue(Grid.RowProperty, row);
                    gg.SetValue(Grid.ColumnProperty, col);
                    gg.Name = "Text" + row + "" + col;
                    gg.SetValue(TagProperty, row+""+col);

                    testGridGrid.Children.Add(gg);
                }

            }
        }

        void Text10(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string content = textBox.Name;
            switch (content)
            {
                case "Text00":
                    MessageBox.Show(textBox.Text + " 00 Text Clicked");
                    return;
                case "Text10":
                    MessageBox.Show(textBox.Text + " 10 Text Clicked");
                    return;
                case "Text20":
                    MessageBox.Show(textBox.Text + " 20 Text Clicked");
                    return;
                case "Text30":
                    MessageBox.Show(textBox.Text + " 30 Text Clicked");
                    return;
            }

        }

    }
}
