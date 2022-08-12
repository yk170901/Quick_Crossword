using QuickCrossword.Model;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for FiveXFive.xaml
    /// </summary>
    public partial class FiveXFive : UserControl
    {
        public FiveXFive()
        {
            InitializeComponent();
        }

        public void GetNewBoard(char[] board)
        {
            // How can I populate a 2D array with a textbox
            // https://www.codeproject.com/Questions/1012575/How-can-I-populate-a-D-array-with-a-textbox

            int boardIdx = 0;
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    var gg = new TextBox();

                    // gg.TextChanged += new TextChangedEventHandler(this.Text10);
                    gg.GotFocus += new RoutedEventHandler(this.TextBoxGotFocus);
                    gg.LostFocus += new RoutedEventHandler(this.TextBoxLostFocus);

                    if (board[boardIdx].Equals('\0'))
                    {
                        gg.Text = "";
                        gg.Background = Brushes.DarkGray;
                        gg.IsHitTestVisible = false;
                    }
                    else
                    {
                        gg.Text = board[boardIdx].ToString();
                        gg.FontSize = 30;
                        gg.HorizontalContentAlignment = HorizontalAlignment.Center;
                        gg.VerticalContentAlignment = VerticalAlignment.Center;
                    }
                    gg.SetValue(Grid.RowProperty, row);
                    gg.SetValue(Grid.ColumnProperty, col);
                   //  gg.Name = board[boardIdx].ToString();
                    gg.SetValue(TagProperty, row + "" + col);

                    FiveGrid.Children.Add(gg);

                    boardIdx++;
                }

            }
        }


        public char[] GetUserAnswer()
        {
            char[] Answer = new char[(int)BoardMode.FiveXFive * (int)BoardMode.FiveXFive ];
            return null;
        }

        void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Background = Brushes.LightBlue;
        }

        void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Background = Brushes.Transparent;
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
                default:
                    MessageBox.Show(textBox.Text);
                    return;
            }
        }




    }
}
