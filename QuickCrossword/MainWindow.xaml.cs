﻿using QuickCrossword.Controller;
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

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            runCount++;
            LoadFiveXFiveBoard();
        }

        private void GridModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (runCount < 1) return;

            ComboBox comboBox = (ComboBox)sender;

            string? selectedMode = comboBox.SelectedValue.ToString()?.Split(" ")[1];

            switch (selectedMode)
            {
                case "5x5":
                    FiveGrid.Visibility = Visibility.Visible;
                    SevenGrid.Visibility = Visibility.Collapsed;
                    TenGrid.Visibility = Visibility.Collapsed;

                    LoadFiveXFiveBoard();
                    return;
                case "7x7":
                    FiveGrid.Visibility = Visibility.Collapsed;
                    SevenGrid.Visibility = Visibility.Visible;
                    TenGrid.Visibility = Visibility.Collapsed;
                    return;
                case "10x10":
                    FiveGrid.Visibility = Visibility.Collapsed;
                    SevenGrid.Visibility = Visibility.Collapsed;
                    TenGrid.Visibility = Visibility.Visible;
                    return;
            }
        }

        private void LoadFiveXFiveBoard()
        {
            char[] board = CrosswordController.Instance().GetBoard(BoardMode.FiveXFive);
            var WordDetailArray = CrosswordController.Instance().GetPlacedWordDetailArray();

            #region 함 보기 결과
            int dd = 0;
            Debug.WriteLine("");
            foreach (var g in board)
            {
                if (g == '\0')
                    Debug.Write("()");
                else
                    Debug.Write(g);
                dd++;
                if (dd % 5 == 0)
                    Debug.WriteLine("");
            }

            foreach (var b in WordDetailArray)
            {
                Debug.WriteLine(b.Word);

            }
            #endregion

            
            FiveGrid.GetNewBoard(board);

            HorizontalClue.GetClueListView(WordDetailArray.Where(o => o.WordDirection == Direction.Horizontal).ToArray());
            VerticalClue.GetClueListView(WordDetailArray.Where(o => o.WordDirection == Direction.Vertical).ToArray());


        }





        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewPuzzleBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
