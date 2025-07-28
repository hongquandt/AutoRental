using BusinessObjects;
using Service.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;

namespace AutoRental.ViewModels.Admin
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly User _admin;
        private readonly IUserService _userService;
        private readonly IBookingService _bookingService;
        private readonly Window _adminWindow;

        // Properties
        public string WelcomeMessage => $"Xin chào quản trị viên, {_admin.Username}!";

        public AdminViewModel(User admin, IUserService userService, IBookingService bookingService, Window adminWindow)
        {
            _admin = admin;
            _userService = userService;
            _bookingService = bookingService;
            _adminWindow = adminWindow;

            // Initialize Commands
            ManageUsersCommand = new RelayCommand(ManageUsers);
            ManageBookingsCommand = new RelayCommand(ManageBookings);
            ManageCarsCommand = new RelayCommand(ManageCars);
            ManageVouchersCommand = new RelayCommand(ManageVouchers);
            LogoutCommand = new RelayCommand(Logout);
        }

        // Commands
        public ICommand ManageUsersCommand { get; }
        public ICommand ManageBookingsCommand { get; }
        public ICommand ManageCarsCommand { get; }
        public ICommand ManageVouchersCommand { get; }
        public ICommand LogoutCommand { get; }

        // Command Methods
        private void ManageUsers()
        {
            var userManagementWindow = new View.Admin.UserManagementWindow();
            userManagementWindow.Show();

            // Đóng AdminWindow chính xác
            _adminWindow.Close();
        }

        private void ManageBookings()
        {
            var bookingManagementWindow = new View.Admin.BookingManagementWindow();
            bookingManagementWindow.Show();

            // Đóng AdminWindow chính xác
            _adminWindow.Close();
        }

        private void ManageCars()
        {
            var carManagementWindow = new View.Admin.CarManagementWindow();
            carManagementWindow.Show();

            // Đóng AdminWindow chính xác
            _adminWindow.Close();
        }

        private void ManageVouchers()
        {
            var voucherManagementWindow = new View.Admin.VoucherManagementWindow();
            voucherManagementWindow.Show();

            // Đóng AdminWindow chính xác
            _adminWindow.Close();
        }

        private void Logout()
        {
            // TODO: Navigate to Login Window
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            _adminWindow.Close();
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // Simple RelayCommand implementation

}