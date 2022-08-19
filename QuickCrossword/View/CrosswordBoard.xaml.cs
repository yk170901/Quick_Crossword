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
    public enum NewBoard
    {
        Setting,
        Answer,
        Submit
    }

    /// <summary>
    /// Interaction logic for CrosswordBoard.xaml
    /// </summary>
    public partial class CrosswordBoard : UserControl
    {
        private char[] _board;
        private int _boardModeNum;
        private string[] _userAnswer;

        public CrosswordBoard()
        {
            InitializeComponent();
        }

        #region TextBox Related Event
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

            int OneDimensionChar = (row * _boardModeNum) + col;
            _userAnswer[OneDimensionChar] = text;
        }
        #endregion

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

            PlaceTextBoxes(NewBoard.Setting);
        }

        public void GetAnswer()
        {
            CrosswordGrid.Children.Clear();
            PlaceTextBoxes(NewBoard.Answer);
        }

        public void ResetBoard()
        {
            CrosswordGrid.Children.Clear();
            PlaceTextBoxes(NewBoard.Setting);
        }

        public void GetUserAnswer()
        {
            CrosswordGrid.Children.Clear();
            PlaceTextBoxes(NewBoard.Submit);
        }

        private void AnswerSetting(TextBox textBox, int boardIdx)
        {
            if (_board[boardIdx].ToString() == _userAnswer[boardIdx])
            {
                textBox.Foreground = Brushes.SkyBlue;
            }
            else
            {
                textBox.Foreground = Brushes.Red;
            }

            // 이걸 하면 TextChanged 이벤트로 userAnswer도 업데이트 되기 때문에 위의 정답 체크 후 Text를 바꾸어야 한다
            textBox.Text = _board[boardIdx].ToString();
        }

        private void UserAnswerSubmit(TextBox textBox, int boardIdx)
        {
            textBox.Text = _userAnswer[boardIdx];
            if (_board[boardIdx].ToString() == _userAnswer[boardIdx])
            {
                textBox.Foreground = Brushes.SkyBlue;
            }
            else
            {
                textBox.Background = Brushes.IndianRed;
            }
        }

        private void PlaceTextBoxes(NewBoard purpose)
        {
            int boardIdx = 0;
            int fontSize = 150 / _boardModeNum;
            for (int row = 0; row < _boardModeNum; row++)
            {
                for (int col = 0; col < _boardModeNum; col++)
                {
                    var textBox = new TextBox();

                    textBox.Text = "";
                    textBox.FontSize = fontSize;
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                    textBox.VerticalContentAlignment = VerticalAlignment.Center;

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
                        if (purpose.Equals(NewBoard.Answer))
                        {
                            AnswerSetting(textBox, boardIdx);
                        }
                        else if (purpose.Equals(NewBoard.Submit))
                        {
                            UserAnswerSubmit(textBox, boardIdx);
                        }
                    }

                    CrosswordGrid.Children.Add(textBox);

                    boardIdx++;
                }
            }
        }

        public void LabelIdx(WordDetail[] wordDetailArray)
        {
            foreach (var wordDetail in wordDetailArray)
            {
                var idxToPlaceLabel = wordDetail.IdxsOnBoard.Min();

                var col = idxToPlaceLabel % _boardModeNum;
                var row = idxToPlaceLabel / _boardModeNum;

                var label = new Label();

                label.Content = wordDetail.Index;
                label.FontSize = 15;
                label.Foreground = Brushes.DeepSkyBlue;
                label.Width = 30;
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.Height = 40;
                label.VerticalAlignment = VerticalAlignment.Top;
                label.SetValue(Grid.ColumnProperty, col);
                label.SetValue(Grid.RowProperty, row);

                CrosswordGrid.Children.Add(label);
            }
        }
    }
}
