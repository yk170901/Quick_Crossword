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
    /// Interaction logic for CrosswordBoard.xaml
    /// </summary>
    public partial class CrosswordBoard : UserControl
    {
        private char[] _board;
        private string[] _userAnswer;
        private int _boardModeNum;

        public CrosswordBoard()
        {
            InitializeComponent();
        }
        private void SetBoard(char[] board, BoardMode boardMode)
        {
            _board = board;
            _boardModeNum = (int)boardMode;
            _userAnswer = new string[_boardModeNum * _boardModeNum];

            for (int i = 0; i<(int)boardMode; i++)
            {
                CrosswordGrid.RowDefinitions.Add(new RowDefinition() { });
                CrosswordGrid.ColumnDefinitions.Add(new ColumnDefinition() { });
            }
        }

        public void GetBoard(char[] board, BoardMode boardMode)
        {
            CrosswordGrid.RowDefinitions.Clear();
            CrosswordGrid.ColumnDefinitions.Clear();
            CrosswordGrid.Children.Clear();

            SetBoard(board, boardMode);

            PlaceTextBoxes();
        }

        private void PlaceTextBoxes()
        {
            int boardIdx = 0;
            for (int row = 0; row < _boardModeNum; row++)
            {
                for (int col = 0; col < _boardModeNum; col++)
                {
                    var textBox = new TextBox();

                    textBox.Text = "";
                    textBox.TextChanged += new TextChangedEventHandler(this.TextChanged);
                    textBox.GotFocus += new RoutedEventHandler(this.TextBoxGotFocus);
                    textBox.LostFocus += new RoutedEventHandler(this.TextBoxLostFocus);

                    textBox.SetValue(Grid.RowProperty, row);
                    textBox.SetValue(Grid.ColumnProperty, col);

                    if (_board[boardIdx].Equals('\0'))
                    {
                        textBox.Background = Brushes.DarkGray;
                        textBox.IsHitTestVisible = false;
                        textBox.IsTabStop = false;
                    }
                    else
                    {
                        // textBox.Text = _board[boardIdx].ToString();
                        textBox.FontSize = 30;
                        textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                        textBox.VerticalContentAlignment = VerticalAlignment.Center;
                    }

                    CrosswordGrid.Children.Add(textBox);

                    boardIdx++;
                }
            }
        }

        private void AnswerSetting(TextBox textBox)
        {

        }

        public void GetAnswer()
        {
            CrosswordGrid.Children.Clear();

            int boardIdx = 0;
            for (int row = 0; row < _boardModeNum; row++)
            {
                for (int col = 0; col < _boardModeNum; col++)
                {
                    var textBox = new TextBox();

                    textBox.IsHitTestVisible = false;

                    if (_board[boardIdx].Equals('\0'))
                    {
                        textBox.Text = "";
                        textBox.Background = Brushes.DarkGray;
                    }
                    else
                    {
                        textBox.Text = _board[boardIdx].ToString();
                        textBox.FontSize = 30;
                        textBox.Foreground = Brushes.DarkRed;
                        textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                        textBox.VerticalContentAlignment = VerticalAlignment.Center;
                    }
                    textBox.SetValue(Grid.RowProperty, row);
                    textBox.SetValue(Grid.ColumnProperty, col);

                    CrosswordGrid.Children.Add(textBox);

                    boardIdx++;
                }
            }
        }

        public void LabelIdx(WordDetail[] wordDetailArray, BoardMode boardMode)
        {
            var boardModeNum = (int)boardMode;

            foreach (var wordDetail in wordDetailArray)
            {
                var idxToPlaceLabel = wordDetail.IdxsOnBoard.Min();

                var col = idxToPlaceLabel % boardModeNum;
                var row = idxToPlaceLabel / boardModeNum;

                var label = new Label();

                label.Content = wordDetail.Index;
                label.FontSize = 20;
                label.Foreground = Brushes.DeepSkyBlue;
                label.Width = 20;
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.Height = 40;
                label.VerticalAlignment = VerticalAlignment.Top;
                label.SetValue(Grid.ColumnProperty, col);
                label.SetValue(Grid.RowProperty, row);

                CrosswordGrid.Children.Add(label);
            }
        }

        public void GetUserAnswer()
        {
            CrosswordGrid.Children.Clear();

            int boardIdx = 0;
            for (int row = 0; row < _boardModeNum; row++)
            {
                for (int col = 0; col < _boardModeNum; col++)
                {
                    var textBox = new TextBox();

                    textBox.TextChanged += new TextChangedEventHandler(this.TextChanged);
                    textBox.GotFocus += new RoutedEventHandler(this.TextBoxGotFocus);
                    textBox.LostFocus += new RoutedEventHandler(this.TextBoxLostFocus);

                    if (_board[boardIdx].Equals('\0'))
                    {
                        textBox.Background = Brushes.DarkGray;
                        textBox.IsHitTestVisible = false;
                        textBox.IsTabStop = false;
                    }
                    else
                    {
                        textBox.Text = _userAnswer[boardIdx];
                        textBox.FontSize = 30;
                        textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                        textBox.VerticalContentAlignment = VerticalAlignment.Center;

                        if (_board[boardIdx].ToString() == _userAnswer[boardIdx])
                        {
                            textBox.Foreground = Brushes.SkyBlue;
                        }
                        else
                        {
                            textBox.Background = Brushes.IndianRed;
                        }

                    }
                    textBox.SetValue(Grid.RowProperty, row);
                    textBox.SetValue(Grid.ColumnProperty, col);

                    CrosswordGrid.Children.Add(textBox);

                    boardIdx++;
                }
            }
        }




        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Background = Brushes.LightBlue;
        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Background = Brushes.Transparent;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;

            var row = (int)textBox.GetValue(Grid.RowProperty);
            var col = (int)textBox.GetValue(Grid.ColumnProperty);

            int OneDimensionChar = (row * (int)BoardMode.FiveXFive) + col;
            _userAnswer[OneDimensionChar] = text;
        }

    }
}
