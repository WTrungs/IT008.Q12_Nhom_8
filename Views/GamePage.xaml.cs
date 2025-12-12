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

namespace TetrisApp.Views {
    public partial class GamePage : Page {
        private Border[,] _Cells = new Border[20, 10];
        private Border[,] _NextCells = new Border[4, 4];

        public GamePage() {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e) {
            NavigationService?.Navigate(new MenuPage());
        }
    }
}