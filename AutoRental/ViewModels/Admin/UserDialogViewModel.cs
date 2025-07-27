using BusinessObjects;
using Service.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System.Text.RegularExpressions;

namespace AutoRental.ViewModels.Admin
{
    public class UserDialogViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly User? _originalUser;
        private readonly bool _isEditMode;
        private Window? _dialogWindow;

        // Properties
        public string DialogTitle => _isEditMode ? "Sửa Người dùng" : "Thêm Người dùng";
        public string DialogSubtitle => _isEditMode ? "Cập nhật thông tin người dùng" : "Thêm người dùng mới vào hệ thống";
        public string DialogIcon => _isEditMode ? "✏️" : "➕";
        public string SaveButtonText => _isEditMode ? "Cập nhật" : "Thêm";
        public bool IsEditMode => _isEditMode;
        public string PasswordLabel => _isEditMode ? "Mật khẩu (thay đổi nếu muốn)" : "Mật khẩu *";

        // Form Properties
        private string _username = "";
        private string _fullName = "";
        private string _email = "";
        private string _password = "";
        private string _phoneNumber = "";
        private Role? _selectedRole;
        private string _errorMessage = "";
        private bool _hasValidationErrors = false;

        public string Username 
        { 
            get => _username; 
            set 
            { 
                _username = value; 
                OnPropertyChanged();
                // Tắt ViewModel validation - chỉ dùng code-behind validation
                // ValidateUsername();
            } 
        }

        public string FullName 
        { 
            get => _fullName; 
            set 
            { 
                _fullName = value; 
                OnPropertyChanged();
                // Tắt ViewModel validation - chỉ dùng code-behind validation
                // ValidateFullName();
            } 
        }

        public string Email 
        { 
            get => _email; 
            set 
            { 
                _email = value; 
                OnPropertyChanged();
                // Tắt ViewModel validation - chỉ dùng code-behind validation
                // ValidateEmail();
            } 
        }

        public string Password 
        { 
            get => _password; 
            set 
            { 
                _password = value; 
                OnPropertyChanged();
                // Tắt ViewModel validation - chỉ dùng code-behind validation
                // ValidatePassword();
            } 
        }

        public string PhoneNumber 
        { 
            get => _phoneNumber; 
            set 
            { 
                _phoneNumber = value; 
                OnPropertyChanged();
                // Tắt ViewModel validation cho PhoneNumber - chỉ dùng code-behind validation
                // ValidatePhoneNumber();
            } 
        }

        public Role? SelectedRole 
        { 
            get => _selectedRole; 
            set 
            { 
                _selectedRole = value; 
                OnPropertyChanged();
                // Tắt ViewModel validation - chỉ dùng code-behind validation
                // ValidateRole();
            } 
        }

        public string ErrorMessage 
        { 
            get => _errorMessage; 
            set 
            { 
                _errorMessage = value; 
                OnPropertyChanged();
            } 
        }

        public bool HasValidationErrors
        {
            get => _hasValidationErrors;
            set
            {
                _hasValidationErrors = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public bool CanSave => !HasValidationErrors && IsFormValid();

        // Collections
        public ObservableCollection<Role> Roles { get; set; }

        // Commands
        public ICommand SaveCommand { get; }

        public UserDialogViewModel(IUserService userService, User? userToEdit = null)
        {
            _userService = userService;
            _originalUser = userToEdit;
            _isEditMode = userToEdit != null;

            // Initialize Commands
            SaveCommand = new RelayCommand(SaveUser, () => CanSave);

            // Load roles
            LoadRoles();

            // Initialize validation state
            HasValidationErrors = false;

            // If editing, populate form with existing data
            if (_isEditMode && _originalUser != null)
            {
                Username = _originalUser.Username;
                FullName = _originalUser.FullName;
                Email = _originalUser.Email;
                PhoneNumber = _originalUser.PhoneNumber ?? "";
                SelectedRole = _originalUser.Role;
                // Load password cũ để hiển thị
                Password = _originalUser.Password;
            }
        }

        public void SetDialogWindow(Window window)
        {
            _dialogWindow = window;
        }

        private void LoadRoles()
        {
            try
            {
                // TODO: Load roles from service
                // For now, create dummy roles
                Roles = new ObservableCollection<Role>
                {
                    new Role { RoleId = 1, RoleName = "Admin" },
                    new Role { RoleId = 2, RoleName = "User" }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách vai trò: {ex.Message}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Validation Methods
        private void ValidateUsername()
        {
            if (!_isEditMode && string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Tên đăng nhập không được để trống!";
                return;
            }

            if (!_isEditMode && Username.Length < 3)
            {
                ErrorMessage = "Tên đăng nhập phải có ít nhất 3 ký tự!";
                return;
            }

            if (!_isEditMode && Username.Length > 50)
            {
                ErrorMessage = "Tên đăng nhập không được quá 50 ký tự!";
                return;
            }

            if (!_isEditMode && !Regex.IsMatch(Username, @"^[a-zA-Z0-9_]+$"))
            {
                ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới!";
                return;
            }

            ClearErrorMessage();
        }

        private void ValidateFullName()
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Họ và tên không được để trống!";
                return;
            }

            if (FullName.Length < 2)
            {
                ErrorMessage = "Họ và tên phải có ít nhất 2 ký tự!";
                return;
            }

            if (FullName.Length > 100)
            {
                ErrorMessage = "Họ và tên không được quá 100 ký tự!";
                return;
            }

            if (!Regex.IsMatch(FullName, @"^[a-zA-ZÀ-ỹ\s]+$"))
            {
                ErrorMessage = "Họ và tên chỉ được chứa chữ cái và khoảng trắng!";
                return;
            }

            ClearErrorMessage();
        }

        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email không được để trống!";
                return;
            }

            if (Email.Length > 100)
            {
                ErrorMessage = "Email không được quá 100 ký tự!";
                return;
            }

            // Email regex pattern
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(Email, emailPattern))
            {
                ErrorMessage = "Email không đúng định dạng! (VD: example@domain.com)";
                return;
            }

            ClearErrorMessage();
        }

        private void ValidatePassword()
        {
            if (!_isEditMode && string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Mật khẩu không được để trống!";
                return;
            }

            if (!_isEditMode && Password.Length < 6)
            {
                ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự!";
                return;
            }

            if (!_isEditMode && Password.Length > 50)
            {
                ErrorMessage = "Mật khẩu không được quá 50 ký tự!";
                return;
            }

            if (!_isEditMode && !Regex.IsMatch(Password, @"^[a-zA-Z0-9!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]+$"))
            {
                ErrorMessage = "Mật khẩu chứa ký tự không hợp lệ!";
                return;
            }

            ClearErrorMessage();
        }

        private void ValidatePhoneNumber()
        {
            if (!string.IsNullOrWhiteSpace(PhoneNumber))
            {
                if (PhoneNumber.Length < 10 || PhoneNumber.Length > 15)
                {
                    ErrorMessage = "Số điện thoại phải có từ 10-15 chữ số!";
                    return;
                }

                if (!Regex.IsMatch(PhoneNumber, @"^[0-9+\-\s()]+$"))
                {
                    ErrorMessage = "Số điện thoại chỉ được chứa số và ký tự: +, -, (, ), khoảng trắng!";
                    return;
                }

                // Kiểm tra có ít nhất 10 chữ số
                string digitsOnly = Regex.Replace(PhoneNumber, @"[^0-9]", "");
                if (digitsOnly.Length < 10)
                {
                    ErrorMessage = "Số điện thoại phải có ít nhất 10 chữ số!";
                    return;
                }
            }

            ClearErrorMessage();
        }

        private void ValidateRole()
        {
            if (SelectedRole == null)
            {
                ErrorMessage = "Vui lòng chọn vai trò!";
                return;
            }

            ClearErrorMessage();
        }

        private void ClearErrorMessage()
        {
            if (string.IsNullOrWhiteSpace(Username) && !_isEditMode ||
                string.IsNullOrWhiteSpace(FullName) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) && !_isEditMode ||
                SelectedRole == null)
            {
                // Không clear error nếu còn field bắt buộc chưa nhập
                return;
            }

            ErrorMessage = "";
        }

        private bool IsFormValid()
        {
            // Kiểm tra tất cả validation
            if (!_isEditMode && string.IsNullOrWhiteSpace(Username))
                return false;

            if (string.IsNullOrWhiteSpace(FullName))
                return false;

            if (string.IsNullOrWhiteSpace(Email))
                return false;

            if (!_isEditMode && string.IsNullOrWhiteSpace(Password))
                return false;

            if (SelectedRole == null)
                return false;

            // Kiểm tra format
            if (!_isEditMode && Username.Length < 3)
                return false;

            if (FullName.Length < 2)
                return false;

            if (!Regex.IsMatch(Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                return false;

            if (!_isEditMode && Password.Length < 6)
                return false;

            return true;
        }

        private void SaveUser()
        {
            try
            {
                // Clear previous error
                ErrorMessage = "";

                // Validate form
                if (!IsFormValid())
                {
                    // Không trigger ViewModel validation nữa - chỉ dùng code-behind validation
                    // ValidateUsername();
                    // ValidateFullName();
                    // ValidateEmail();
                    // ValidatePassword();
                    // ValidateRole();
                    return;
                }

                // Create or update user
                User user;
                bool success;

                if (_isEditMode && _originalUser != null)
                {
                    // Update existing user
                    user = _originalUser;
                    user.FullName = FullName;
                    user.Email = Email;
                    user.PhoneNumber = PhoneNumber;
                    user.RoleId = SelectedRole.RoleId;
                    // KHÔNG gán Role navigation property để tránh tracking conflict
                    // user.Role = SelectedRole;

                    // Update password chỉ khi có nhập password mới và khác password cũ
                    if (!string.IsNullOrWhiteSpace(Password) && Password != _originalUser.Password)
                    {
                        user.Password = Password;
                    }
                    // Nếu password trống hoặc giống password cũ, giữ nguyên password cũ

                    success = _userService.Update(user);
                }
                else
                {
                    // Create new user
                    user = new User
                    {
                        Username = Username,
                        FullName = FullName,
                        Email = Email,
                        Password = Password,
                        PhoneNumber = PhoneNumber,
                        RoleId = SelectedRole.RoleId,
                        // KHÔNG gán Role navigation property để tránh tracking conflict
                        // Role = SelectedRole,
                        CreatedDate = DateTime.Now
                    };

                    success = _userService.Add(user);
                }

                if (success)
                {
                    // Show success message
                    string successMessage = _isEditMode 
                        ? $"Cập nhật thông tin user '{user.Username}' thành công!"
                        : $"Thêm user '{user.Username}' thành công!";
                    
                    MessageBox.Show(successMessage, 
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Close dialog with success
                    if (_dialogWindow != null)
                    {
                        _dialogWindow.DialogResult = true;
                        _dialogWindow.Close();
                    }
                }
                else
                {
                    // Show error message
                    string errorMessage = _isEditMode 
                        ? "Không thể cập nhật user. Vui lòng kiểm tra lại thông tin và thử lại!"
                        : "Không thể thêm user. Vui lòng kiểm tra lại thông tin và thử lại!";
                    
                    MessageBox.Show(errorMessage, 
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Show exception message
                string exceptionMessage = _isEditMode 
                    ? $"Lỗi khi cập nhật user: {ex.Message}"
                    : $"Lỗi khi thêm user: {ex.Message}";
                
                MessageBox.Show(exceptionMessage, 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 