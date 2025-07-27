using System.Windows;
using AutoRental;
using System.Windows.Input;

namespace AutoRental.View
{
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.HomeViewModel();
        }

        private void BookCar_Click(object sender, RoutedEventArgs e)
        {
            var carListWindow = new CarListWindow(null); // Truyền null nếu không cần user
            carListWindow.ShowDialog();
        }
    }
} 