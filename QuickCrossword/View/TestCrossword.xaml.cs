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
    /// Interaction logic for TestCrossword.xaml
    /// </summary>
    public partial class TestCrossword : UserControl
    {


        public TestCrossword()
        {
            //List<List<char>> WordCharList = new();

            //for (int i = 0; i < 5; i++)
            //{
            //    WordCharList.Add(new List<char>());

            //    for (int j = 0; j < 5; j++)
            //    {
            //        WordCharList[i].Add(Char.Parse(j.ToString()));
            //    }
            //}
            //var gsa = WordCharList.ToArray();

            InitializeComponent();

            // lst.ItemsSource = WordCharList;

            var boxes = new TextBox[,]
            {
                 { new TextBox(){ Text="잉" }, new TextBox(){  Text="꼬" }, new TextBox(){  Text="#" } },
                 { new TextBox(){  Text="#" }, new TextBox(){ Text="리" }, new TextBox(){ Text="본"  } },
                 { new TextBox(){ Text="#"  }, new TextBox(){ Text="탕"  }, new TextBox(){  Text="#" } },
            };


            var data = new char[,] { { '잉','#','7' }, { '꼬','#','8' }, { '부', '%','9' }, { '부','%','0'} };

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    boxes[i, j].Text = data[i,j].ToString();

            lst.ItemsSource = boxes;
        }
    }
}
