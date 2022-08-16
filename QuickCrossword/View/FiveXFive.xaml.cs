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
        private char[] _board;
        private string[] _userAnswer;
        private BoardMode _boardMode;

        public FiveXFive()
        {
            InitializeComponent();
        }

        private void SetBoard()
        {
            FiveGrid.RowDefinitions.Add(new RowDefinition() { });
            FiveGrid.RowDefinitions.Add(new RowDefinition() { });
            FiveGrid.RowDefinitions.Add(new RowDefinition() { });
            FiveGrid.RowDefinitions.Add(new RowDefinition() { });
            FiveGrid.RowDefinitions.Add(new RowDefinition() { });

            FiveGrid.ColumnDefinitions.Add(new ColumnDefinition() { });
            FiveGrid.ColumnDefinitions.Add(new ColumnDefinition() { });
            FiveGrid.ColumnDefinitions.Add(new ColumnDefinition() { });
            FiveGrid.ColumnDefinitions.Add(new ColumnDefinition() { });
            FiveGrid.ColumnDefinitions.Add(new ColumnDefinition() { });
        }

        // answer 때 넣기
        public void GetBoard(char[] board)
        {
            SetBoard();
            ///
            _userAnswer = new string[(int)BoardMode.FiveXFive* (int)BoardMode.FiveXFive];
            ///
            FiveGrid.Children.Clear();

            _board = board;

            int boardIdx = 0;
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    var textBox = new TextBox();

                    textBox.TextChanged += new TextChangedEventHandler(this.TextChanged);
                    textBox.GotFocus += new RoutedEventHandler(this.TextBoxGotFocus);
                    textBox.LostFocus += new RoutedEventHandler(this.TextBoxLostFocus);

                    textBox.Text = "";

                    if (_board[boardIdx].Equals('\0'))
                    {
                        textBox.Background = Brushes.DarkGray;
                        textBox.IsHitTestVisible = false;
                        textBox.IsTabStop = false;
                    }
                    else
                    {
                        textBox.FontSize = 30;
                        textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                        textBox.VerticalContentAlignment = VerticalAlignment.Center;
                    }
                    textBox.SetValue(Grid.RowProperty, row);
                    textBox.SetValue(Grid.ColumnProperty, col);

                    FiveGrid.Children.Add(textBox);

                    boardIdx++;
                }
            }
        }

        public void GetAnswer()
        {
            FiveGrid.Children.Clear();

            int boardIdx = 0;
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    var textBox = new TextBox();

                    textBox.Text = "";
                    textBox.IsHitTestVisible = false;

                    if (_board[boardIdx].Equals('\0'))
                    {
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

                    FiveGrid.Children.Add(textBox);

                    boardIdx++;
                }
            }
        }

        public void LabelIdx(WordDetail[] wordDetailArray, BoardMode boardMode)
        {
            var boardModeNum = (int)boardMode;

            foreach(var wordDetail in wordDetailArray)
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

                FiveGrid.Children.Add(label);
            }
        }

        public void GetUserAnswer()
        {
            FiveGrid.Children.Clear();

            int boardIdx = 0;
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
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

                    FiveGrid.Children.Add(textBox);

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
