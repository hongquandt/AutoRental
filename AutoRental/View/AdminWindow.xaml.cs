using System.Windows;
using BusinessObjects;

namespace AutoRental
{
    public partial class AdminWindow : Window
    {
        private readonly User _admin;
        public AdminWindow(User admin)
        {
            InitializeComponent();
            _admin = admin;
            txtAdminWelcome.Text = $"Xin chào quản trị viên, {_admin.Username}!";
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
} 