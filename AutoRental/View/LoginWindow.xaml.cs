using System.Windows;
using System.Windows.Input;
using AutoRental.ViewModels;
using Service.Implementations;
using Service.Interfaces;
using Repositories.Implementations;
using Repositories.Interfaces;
using BusinessObjects;

namespace AutoRental
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;
        public LoginWindow()
        {
            InitializeComponent();
            IUserRepository userRepo = new UserRepository();
            IUserService userService = new UserService(userRepo);
            _viewModel = new LoginViewModel(userService);
            this.DataContext = _viewModel;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = txtPassword.Password;
            var user = _viewModel.Authenticate();
            if (user != null)
            {
                if (user.RoleId == 1)
                {
                    var adminWindow = new AdminWindow(user);
                    adminWindow.Show();
                }
                else
                {
                    var mainWindow = new MainWindow(user);
                    mainWindow.Show();
                }
                this.Close();
            }
        }

        private void btnOpenRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }
    }
} 