using System.Windows;
using System.Windows.Input;
using AutoRental.ViewModels;
using Repositories.Implementations;
using Repositories.Interfaces;

namespace AutoRental
{
    public partial class RegisterWindow : Window
    {
        private readonly RegisterViewModel _viewModel;
        public RegisterWindow()
        {
            InitializeComponent();
            IUserRepository userRepo = new UserRepository();
            _viewModel = new RegisterViewModel(userRepo);
            this.DataContext = _viewModel;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = txtPassword.Password;
            if (_viewModel.Register())
            {
                MessageBox.Show("Đăng ký thành công! Bạn có thể đăng nhập.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnRegister_Click(sender, e);
            }
        }
    }
} 