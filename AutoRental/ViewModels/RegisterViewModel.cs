using System.ComponentModel;
using System.Runtime.CompilerServices;
using Service.Interfaces;
using BusinessObjects;
using Repositories.Interfaces;
using System.Text.RegularExpressions;

namespace AutoRental.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private string _email;
        private string _password;
        private string _username;
        private string _fullName;
        private string _phoneNumber;
        private string _errorMessage;
        private readonly IUserRepository _userRepository;

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
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public RegisterViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool Register()
        {
            ErrorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(PhoneNumber))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin!";
                return false;
            }
            if (Username.Length < 3 || Username.Length > 50)
            {
                ErrorMessage = "Username phải từ 3-50 ký tự!";
                return false;
            }
            if (FullName.Length < 3 || FullName.Length > 100)
            {
                ErrorMessage = "Full Name phải từ 3-100 ký tự!";
                return false;
            }
            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ErrorMessage = "Email không đúng định dạng!";
                return false;
            }
            if (Password.Length < 6 || Password.Length > 50)
            {
                ErrorMessage = "Mật khẩu phải từ 6-50 ký tự!";
                return false;
            }
            if (!Regex.IsMatch(PhoneNumber, @"^\d{10,12}$"))
            {
                ErrorMessage = "Số điện thoại phải là 10-12 số!";
                return false;
            }
            if (_userRepository.GetUserByEmail(Email) != null)
            {
                ErrorMessage = "Email đã tồn tại!";
                return false;
            }
            using (var db = new AutoRentalPrnContext())
            {
                var user = new User
                {
                    Email = Email,
                    Password = Password,
                    Username = Username,
                    FullName = FullName,
                    PhoneNumber = PhoneNumber,
                    RoleId = 2,
                    CreatedDate = System.DateTime.Now
                };
                db.Users.Add(user);
                db.SaveChanges();
            }
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 