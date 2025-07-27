using System.Windows;
using AutoRental;
using System.Windows.Input;
using BusinessObjects;

namespace AutoRental.View
{
    public partial class HomeWindow : Window
    {
        private readonly User _currentUser;
        public HomeWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            this.DataContext = new ViewModels.HomeViewModel();
        }

        private void BookCar_Click(object sender, RoutedEventArgs e)
        {
            var carListWindow = new CarListWindow(_currentUser); // Truyền user đúng
            carListWindow.ShowDialog();
        }
    }
} 