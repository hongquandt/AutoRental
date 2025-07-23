using System.ComponentModel;
using System.Runtime.CompilerServices;
using Service.Interfaces;
using BusinessObjects;

namespace AutoRental.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _email;
        private string _password;
        private string _errorMessage;
        private readonly IUserService _userService;

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public User? Authenticate()
        {     
            ErrorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập email và mật khẩu!";
                return null;
            }
            var user = _userService.Authenticate(Email, Password);
            if (user == null)
            {
                ErrorMessage = "Email hoặc mật khẩu không đúng!";
            }
            return user;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 