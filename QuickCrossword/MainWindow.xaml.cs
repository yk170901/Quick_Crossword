using QuickCrossword.Controller;
using QuickCrossword.Model;
using QuickCrossword.Model.Db;
using QuickCrossword.View;
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

namespace QuickCrossword
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte runCount = 0;
        private char[] _board;
        private WordDetail[] _wordDetailArray;
        private BoardMode _boardMode;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _boardMode = BoardMode.FiveXFive;
            runCount++;
            GetNewCrossword();
            LoadBoard(_boardMode);
        }

        private void GetNewCrossword()
        {
            _board = CrosswordController.Instance().GetBoard(_boardMode);
            _wordDetailArray = CrosswordController.Instance().GetPlacedWordDetailArray();
        }

        private void GridModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (runCount < 1) return; // Review nono

            ComboBox comboBox = (ComboBox)sender;

            string? selectedMode = comboBox.SelectedValue.ToString()?.Split(" ")[1];

            switch (selectedMode)
            {
                case "5x5":
                    _boardMode = BoardMode.FiveXFive;
                    break;
                case "7x7":
                    _boardMode = BoardMode.SevenXSeven;
                    break;
                case "10x10":
                    _boardMode = BoardMode.TenXTen;
                    break;
            }

            GetNewCrossword();
            LoadBoard(_boardMode);
        }

        private void LoadBoard(BoardMode boardMode)
        {
            CrosswordGrid.GetBoard(_board, boardMode);
            CrosswordGrid.LabelIdx(_wordDetailArray);

            HorizontalClue.GetClueListView(_wordDetailArray.Where(o => o.WordDirection == Direction.Horizontal).ToArray());
            VerticalClue.GetClueListView(_wordDetailArray.Where(o => o.WordDirection == Direction.Vertical).ToArray());

            SubmitBtn.IsEnabled = true;
            ResetBtn.IsEnabled = true;
        }

        private void NewPuzzleBtn_Click(object sender, RoutedEventArgs e)
        {
            GetNewCrossword();
            LoadBoard(_boardMode);
        }
        private void AnswerBtn_Click(object sender, RoutedEventArgs e)
        {
            CrosswordGrid.GetAnswer();
            CrosswordGrid.LabelIdx(_wordDetailArray);
            SubmitBtn.IsEnabled = false;
            ResetBtn.IsEnabled = false;
        }

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            CrosswordGrid.GetUserAnswer();
            CrosswordGrid.LabelIdx(_wordDetailArray);
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            CrosswordGrid.ResetBoard();
            CrosswordGrid.LabelIdx(_wordDetailArray);
        }

    }
}
