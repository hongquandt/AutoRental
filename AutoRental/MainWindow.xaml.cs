using System.Windows;
using AutoRental.View;
using BusinessObjects;

namespace AutoRental
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            txtWelcome.Text = $"Xin chào, {_currentUser.Username}!";
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void btnViewCars_Click(object sender, RoutedEventArgs e)
        {
            var carListWindow = new CarListWindow(_currentUser);
            carListWindow.ShowDialog();
        }

        private void btnBookingHistory_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new BookingHistoryWindow(_currentUser);
            historyWindow.ShowDialog();
        }

        private void btnViewProfile_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new UserProfileWindow(_currentUser);
            profileWindow.ShowDialog();
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            var homeWindow = new HomeWindow();
            homeWindow.ShowDialog();
        }
    }
}