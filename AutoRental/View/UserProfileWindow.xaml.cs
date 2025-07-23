using System;
using System.Windows;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutoRental.View
{
    public partial class UserProfileWindow : Window
    {
        private readonly User _currentUser;

        public UserProfileWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            using (var context = new AutoRentalPrnContext())
            {
                var userFromDb = context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.UserId == _currentUser.UserId);

                if (userFromDb != null)
                {
                    txtUsername.Text = userFromDb.Username;
                    txtFullName.Text = userFromDb.FullName;
                    txtEmail.Text = userFromDb.Email;
                    txtPhone.Text = userFromDb.PhoneNumber ?? "";
                    txtRole.Text = userFromDb.Role.RoleName;
                    txtCreatedDate.Text = userFromDb.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new AutoRentalPrnContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == _currentUser.UserId);
                if (user != null)
                {
                    user.FullName = txtFullName.Text.Trim();
                    user.Email = txtEmail.Text.Trim();
                    user.PhoneNumber = txtPhone.Text.Trim();
                    context.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUserProfile();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy người dùng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnShowChangePassword_Click(object sender, RoutedEventArgs e)
        {
            panelChangePassword.Visibility = panelChangePassword.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            // Xóa dữ liệu cũ khi mở lại panel
            txtOldPassword.Password = "";
            txtNewPassword.Password = "";
            txtConfirmPassword.Password = "";
        }

        private void btnSavePassword_Click(object sender, RoutedEventArgs e)
        {
            string oldPass = txtOldPassword.Password;
            string newPass = txtNewPassword.Password;
            string confirmPass = txtConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(oldPass) || string.IsNullOrWhiteSpace(newPass) || string.IsNullOrWhiteSpace(confirmPass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các trường mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (newPass != confirmPass)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (newPass.Length < 4)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 4 ký tự!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var context = new AutoRentalPrnContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == _currentUser.UserId);
                if (user == null)
                {
                    MessageBox.Show("Không tìm thấy người dùng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (user.Password != oldPass)
                {
                    MessageBox.Show("Mật khẩu cũ không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                user.Password = newPass;
                context.SaveChanges();
                MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                // Ẩn panel đổi mật khẩu và xóa dữ liệu
                panelChangePassword.Visibility = Visibility.Collapsed;
                txtOldPassword.Password = "";
                txtNewPassword.Password = "";
                txtConfirmPassword.Password = "";
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 