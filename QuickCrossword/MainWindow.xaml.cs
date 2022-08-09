using QuickCrossword.Model.Db;
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
            runCount++;

            var wordAndClueList = SqliteDataAccess.LoadWordAndClue();

            Random rnd = new Random();

            // Take 50 random numbers in range of 4 to 214 with NO duplicate
            var randomNumbers = Enumerable.Range(4, 214).OrderBy(x => rnd.Next()).Take(50).ToArray();

            foreach (int rndNum in randomNumbers)
            {
                var test = wordAndClueList.Where(o => o.Id == rndNum).Select(o => o.Word);

                foreach(var g in test)
                {
                    Debug.WriteLine(g);
                }
                Debug.WriteLine(test);
            }
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
