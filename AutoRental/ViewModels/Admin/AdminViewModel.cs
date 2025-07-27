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
            ManageVouchersCommand = new RelayCommand(ManageVouchers);
            LogoutCommand = new RelayCommand(Logout);
        }

        // Commands
        public ICommand ManageUsersCommand { get; }
        public ICommand ManageBookingsCommand { get; }
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
            // TODO: Open Booking Management Window
            MessageBox.Show("Chức năng quản lý Booking sẽ được phát triển sau!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
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
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => _execute();
    }
}