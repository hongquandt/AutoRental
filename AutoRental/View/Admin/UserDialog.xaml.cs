using System.Windows;
using AutoRental.ViewModels.Admin;
using System.Text.RegularExpressions;

namespace AutoRental.View.Admin
{
    public partial class UserDialog : Window
    {
        private bool _isPasswordVisible = false;

        public UserDialog(UserDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // Set owner để dialog hiển thị modal
            this.Owner = Application.Current.MainWindow;
            
            // Set dialog window reference
            viewModel.SetDialogWindow(this);
            
            // Handle form setup
            this.Loaded += UserDialog_Loaded;
            
            // Handle property changes
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void UserDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserDialogViewModel viewModel)
            {
                // Disable Username field if editing
                if (viewModel.IsEditMode)
                {
                    txtUsername.IsEnabled = false;
                    txtUsername.Background = System.Windows.Media.Brushes.LightGray;
                }
                
                // Set password if editing (show current password)
                if (viewModel.IsEditMode && !string.IsNullOrEmpty(viewModel.Password))
                {
                    // Show current password in fields
                    txtPassword.Password = viewModel.Password;
                    txtPasswordVisible.Text = viewModel.Password;
                }
                
                // Trigger initial validation for all fields
                ValidateUsernameRealTime();
                ValidateFullNameRealTime();
                ValidateEmailRealTime();
                ValidatePasswordRealTime();
                ValidatePhoneNumberRealTime();
                ValidateRoleRealTime();
                
                // Force password validation after a short delay to ensure UI is ready
                Dispatcher.BeginInvoke(new Action(() => 
                {
                    ValidatePasswordRealTime();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        // Username Validation
        private void txtUsername_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (DataContext is UserDialogViewModel viewModel && !viewModel.IsEditMode)
            {
                ValidateUsernameRealTime();
            }
        }

        private void txtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserDialogViewModel viewModel && !viewModel.IsEditMode)
            {
                ValidateUsernameRealTime();
            }
        }

        private void ValidateUsernameRealTime()
        {
            string username = txtUsername.Text;
            
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowFieldError(txtUsernameError, "Tên đăng nhập không được để trống!");
                return;
            }

            if (username.Length < 3)
            {
                ShowFieldError(txtUsernameError, "Tên đăng nhập phải có ít nhất 3 ký tự!");
                return;
            }

            if (username.Length > 50)
            {
                ShowFieldError(txtUsernameError, "Tên đăng nhập không được quá 50 ký tự!");
                return;
            }

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            {
                ShowFieldError(txtUsernameError, "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới!");
                return;
            }

            HideFieldError(txtUsernameError);
        }

        // FullName Validation
        private void txtFullName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateFullNameRealTime();
        }

        private void txtFullName_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateFullNameRealTime();
        }

        private void ValidateFullNameRealTime()
        {
            string fullName = txtFullName.Text;
            
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowFieldError(txtFullNameError, "Họ và tên không được để trống!");
                return;
            }

            if (fullName.Length < 2)
            {
                ShowFieldError(txtFullNameError, "Họ và tên phải có ít nhất 2 ký tự!");
                return;
            }

            if (fullName.Length > 100)
            {
                ShowFieldError(txtFullNameError, "Họ và tên không được quá 100 ký tự!");
                return;
            }

            if (!Regex.IsMatch(fullName, @"^[a-zA-ZÀ-ỹ\s]+$"))
            {
                ShowFieldError(txtFullNameError, "Họ và tên chỉ được chứa chữ cái và khoảng trắng!");
                return;
            }

            HideFieldError(txtFullNameError);
        }

        // Email Validation
        private void txtEmail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateEmailRealTime();
        }

        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateEmailRealTime();
        }

        private void ValidateEmailRealTime()
        {
            string email = txtEmail.Text;
            
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowFieldError(txtEmailError, "Email không được để trống!");
                return;
            }

            if (email.Length > 100)
            {
                ShowFieldError(txtEmailError, "Email không được quá 100 ký tự!");
                return;
            }

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                ShowFieldError(txtEmailError, "Email không đúng định dạng! (VD: example@domain.com)");
                return;
            }

            HideFieldError(txtEmailError);
        }

        // Password Validation
        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            SyncPasswordToViewModel();
            ValidatePasswordRealTime();
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            SyncPasswordToViewModel();
            ValidatePasswordRealTime();
        }

        private void txtPasswordVisible_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SyncPasswordToViewModel();
            ValidatePasswordRealTime();
        }

        private void txtPasswordVisible_LostFocus(object sender, RoutedEventArgs e)
        {
            SyncPasswordToViewModel();
            ValidatePasswordRealTime();
        }

        private void ValidatePasswordRealTime()
        {
            string password = _isPasswordVisible ? txtPasswordVisible.Text : txtPassword.Password;
            
            // Kiểm tra xem có phải edit mode không
            bool isEditMode = false;
            if (DataContext is UserDialogViewModel viewModel)
            {
                isEditMode = viewModel.IsEditMode;
            }
            
            // Debug: Log password và mode chi tiết
            System.Diagnostics.Debug.WriteLine($"Password: '{password}', Length: {password?.Length}, IsEditMode: {isEditMode}");
            System.Diagnostics.Debug.WriteLine($"IsPasswordVisible: {_isPasswordVisible}");
            System.Diagnostics.Debug.WriteLine($"txtPasswordVisible.Text: '{txtPasswordVisible.Text}'");
            System.Diagnostics.Debug.WriteLine($"txtPassword.Password: '{txtPassword.Password}'");
            
            // Trong add mode, password bắt buộc
            if (!isEditMode && string.IsNullOrWhiteSpace(password))
            {
                ShowFieldError(txtPasswordError, "Mật khẩu không được để trống!");
                return;
            }

            // Trong edit mode, nếu password trống thì OK (giữ nguyên password cũ)
            if (isEditMode && string.IsNullOrWhiteSpace(password))
            {
                HideFieldError(txtPasswordError);
                return;
            }

            // Nếu có nhập password (dù là add hay edit), thì validate
            if (!string.IsNullOrWhiteSpace(password))
            {
                System.Diagnostics.Debug.WriteLine($"Validating password: '{password}', Length: {password.Length}");
                
                if (password.Length < 6)
                {
                    System.Diagnostics.Debug.WriteLine($"Password too short: {password.Length} < 6");
                    ShowFieldError(txtPasswordError, "Mật khẩu phải có ít nhất 6 ký tự!");
                    return;
                }

                if (password.Length > 50)
                {
                    System.Diagnostics.Debug.WriteLine($"Password too long: {password.Length} > 50");
                    ShowFieldError(txtPasswordError, "Mật khẩu không được quá 50 ký tự!");
                    return;
                }

                if (!Regex.IsMatch(password, @"^[a-zA-Z0-9!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]+$"))
                {
                    System.Diagnostics.Debug.WriteLine($"Password contains invalid characters: '{password}'");
                    ShowFieldError(txtPasswordError, "Mật khẩu chứa ký tự không hợp lệ!");
                    return;
                }
                
                System.Diagnostics.Debug.WriteLine($"Password validation passed: '{password}'");
            }

            HideFieldError(txtPasswordError);
        }

        // PhoneNumber Validation
        private void txtPhoneNumber_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidatePhoneNumberRealTime();
        }

        private void txtPhoneNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePhoneNumberRealTime();
        }

        private void ValidatePhoneNumberRealTime()
        {
            string phoneNumber = txtPhoneNumber.Text;
            
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                if (phoneNumber.Length < 10 || phoneNumber.Length > 15)
                {
                    ShowFieldError(txtPhoneNumberError, "Số điện thoại phải có từ 10-15 chữ số!");
                    return;
                }

                if (!Regex.IsMatch(phoneNumber, @"^[0-9+\-\s()]+$"))
                {
                    ShowFieldError(txtPhoneNumberError, "Số điện thoại chỉ được chứa số và ký tự: +, -, (, ), khoảng trắng!");
                    return;
                }

                string digitsOnly = Regex.Replace(phoneNumber, @"[^0-9]", "");
                if (digitsOnly.Length < 10)
                {
                    ShowFieldError(txtPhoneNumberError, "Số điện thoại phải có ít nhất 10 chữ số!");
                    return;
                }
            }

            HideFieldError(txtPhoneNumberError);
        }

        // Role Validation
        private void cmbRole_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ValidateRoleRealTime();
        }

        private void cmbRole_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateRoleRealTime();
        }

        private void ValidateRoleRealTime()
        {
            if (cmbRole.SelectedItem == null)
            {
                ShowFieldError(txtRoleError, "Vui lòng chọn vai trò!");
                return;
            }

            HideFieldError(txtRoleError);
        }

        // Helper methods
        private void ShowFieldError(System.Windows.Controls.TextBlock errorBlock, string message)
        {
            errorBlock.Text = message;
            errorBlock.Visibility = Visibility.Visible;
            
            // Update ViewModel validation state
            if (DataContext is UserDialogViewModel viewModel)
            {
                viewModel.HasValidationErrors = true;
            }
        }

        private void HideFieldError(System.Windows.Controls.TextBlock errorBlock)
        {
            errorBlock.Visibility = Visibility.Collapsed;
            
            // Check if all errors are hidden
            CheckAllErrorsHidden();
        }

        private void CheckAllErrorsHidden()
        {
            // Check if all error blocks are hidden
            bool allHidden = txtUsernameError.Visibility == Visibility.Collapsed &&
                           txtFullNameError.Visibility == Visibility.Collapsed &&
                           txtEmailError.Visibility == Visibility.Collapsed &&
                           txtPasswordError.Visibility == Visibility.Collapsed &&
                           txtPhoneNumberError.Visibility == Visibility.Collapsed &&
                           txtRoleError.Visibility == Visibility.Collapsed;

            // Update ViewModel validation state
            if (DataContext is UserDialogViewModel viewModel)
            {
                viewModel.HasValidationErrors = !allHidden;
            }
        }

        private void btnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;
            
            if (_isPasswordVisible)
            {
                // Hiện password
                txtPassword.Visibility = Visibility.Collapsed;
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtToggleIcon.Text = "🙈";
                btnTogglePassword.ToolTip = "Ẩn mật khẩu";
                
                // Sync password từ PasswordBox sang TextBox
                txtPasswordVisible.Text = txtPassword.Password;
            }
            else
            {
                // Ẩn password
                txtPassword.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                txtToggleIcon.Text = "👁️";
                btnTogglePassword.ToolTip = "Hiện mật khẩu";
                
                // Sync password từ TextBox sang PasswordBox
                txtPassword.Password = txtPasswordVisible.Text;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Sync password từ UI sang ViewModel trước khi save
            SyncPasswordToViewModel();
            
            // Gọi SaveCommand từ ViewModel
            if (DataContext is UserDialogViewModel viewModel)
            {
                viewModel.SaveCommand.Execute(null);
            }
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserDialogViewModel.ErrorMessage))
            {
                if (DataContext is UserDialogViewModel viewModel)
                {
                    // Show/hide error message
                    if (!string.IsNullOrEmpty(viewModel.ErrorMessage))
                    {
                        txtErrorMessage.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        txtErrorMessage.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // Method để lấy password từ PasswordBox
        public string GetPassword()
        {
            if (_isPasswordVisible)
            {
                return txtPasswordVisible.Text;
            }
            else
            {
                return txtPassword.Password;
            }
        }

        // Method để set password vào ViewModel trước khi save
        public void SyncPasswordToViewModel()
        {
            if (DataContext is UserDialogViewModel viewModel)
            {
                string currentPassword = GetPassword();
                System.Diagnostics.Debug.WriteLine($"Syncing password to ViewModel: '{currentPassword}'");
                viewModel.Password = currentPassword;
                
                // Force validation after sync
                ValidatePasswordRealTime();
            }
        }
    }
} 