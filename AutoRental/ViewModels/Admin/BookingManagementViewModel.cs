using BusinessObjects;
using Service.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System;
using System.Linq;
using AutoRental.View.Admin;
using AutoRental;
using AutoRental.ViewModels.Admin;

namespace AutoRental.ViewModels.Admin
{
    public class BookingManagementViewModel : INotifyPropertyChanged
    {
        private readonly IBookingService _bookingService;
        private ObservableCollection<Booking> _bookings;
        private Booking? _selectedBooking;
        private string _searchText = "";
        private List<Booking> _allBookings;
        private string _statusMessage = "Sẵn sàng";
        private string? _selectedStatusFilter = null;
        private ObservableCollection<string> _availableStatuses;

        public ObservableCollection<Booking> Bookings
        {
            get => _bookings;
            set
            {
                _bookings = value;
                OnPropertyChanged();
            }
        }

        public Booking? SelectedBooking
        {
            get => _selectedBooking;
            set
            {
                if (_selectedBooking != value)
                {
                    _selectedBooking = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanEditBooking));
                    OnPropertyChanged(nameof(CanDeleteBooking));
                    CommandManager.InvalidateRequerySuggested();

                    // Debug info
                    System.Diagnostics.Debug.WriteLine($"SelectedBooking changed to: {_selectedBooking?.BookingCode ?? "null"}");
                    System.Diagnostics.Debug.WriteLine($"CanEditBooking: {CanEditBooking}");
                    System.Diagnostics.Debug.WriteLine($"CanDeleteBooking: {CanDeleteBooking}");
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                SearchBookings();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public string? SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                _selectedStatusFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ObservableCollection<string> AvailableStatuses
        {
            get => _availableStatuses;
            set
            {
                _availableStatuses = value;
                OnPropertyChanged();
            }
        }

        public bool CanEditBooking
        {
            get
            {
                var canEdit = SelectedBooking != null;
                System.Diagnostics.Debug.WriteLine($"CanEditBooking: {canEdit}, SelectedBooking: {SelectedBooking?.BookingCode ?? "null"}");
                return canEdit;
            }
        }

        public bool CanDeleteBooking
        {
            get
            {
                var canDelete = SelectedBooking != null;
                System.Diagnostics.Debug.WriteLine($"CanDeleteBooking: {canDelete}, SelectedBooking: {SelectedBooking?.BookingCode ?? "null"}");
                return canDelete;
            }
        }

        // Force enable buttons for testing
        public bool ForceEnableEdit => true;
        public bool ForceEnableDelete => true;

        public ICommand LoadBookingsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand AddBookingCommand { get; }
        public ICommand EditBookingCommand { get; }
        public ICommand DeleteBookingCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand BackCommand { get; }

        public BookingManagementViewModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
            _bookings = new ObservableCollection<Booking>();
            _allBookings = new List<Booking>();
            _availableStatuses = new ObservableCollection<string>();

            LoadBookingsCommand = new RelayCommand(LoadBookings);
            SearchCommand = new RelayCommand(SearchBookings);
            RefreshCommand = new RelayCommand(RefreshBookings);
            AddBookingCommand = new RelayCommand(AddBooking);
            EditBookingCommand = new RelayCommand(EditBooking);
            DeleteBookingCommand = new RelayCommand(DeleteBooking);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            BackCommand = new RelayCommand(BackToAdmin);

            LoadBookings();
            LoadAvailableStatuses();
        }

        private void LoadBookings()
        {
            try
            {
                StatusMessage = "Đang tải...";
                _allBookings = _bookingService.GetAll().ToList();
                Bookings = new ObservableCollection<Booking>(_allBookings);
                LoadAvailableStatuses();
                StatusMessage = "Sẵn sàng";

                // Reset selection
                SelectedBooking = null;

                System.Diagnostics.Debug.WriteLine($"Loaded {_allBookings.Count} bookings");
                foreach (var booking in _allBookings.Take(3))
                {
                    System.Diagnostics.Debug.WriteLine($"Booking: {booking.BookingCode}, User: {booking.User?.FullName}, Car: {booking.Car?.CarModel}");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi tải dữ liệu";
                MessageBox.Show($"Lỗi khi tải danh sách booking: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchBookings()
        {
            ApplyFilters();
        }

        private void RefreshBookings()
        {
            SearchText = "";
            SelectedStatusFilter = null;
            LoadBookings();
        }

        private void LoadAvailableStatuses()
        {
            try
            {
                var statuses = new List<string> { "Tất cả trạng thái" };
                statuses.AddRange(new[] { "Pending", "Confirmed", "Cancelled", "Completed" });
                AvailableStatuses = new ObservableCollection<string>(statuses);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách trạng thái: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                StatusMessage = "Đang lọc...";

                var filteredBookings = _allBookings.AsEnumerable();

                if (SelectedStatusFilter != null && SelectedStatusFilter != "Tất cả trạng thái")
                {
                    filteredBookings = filteredBookings.Where(b => b.Status == SelectedStatusFilter);
                }

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var searchTerm = SearchText.Trim().ToLower();
                    filteredBookings = filteredBookings.Where(b =>
                        b.BookingCode.ToLower().Contains(searchTerm) ||
                        b.User.FullName.ToLower().Contains(searchTerm) ||
                        b.Car.CarModel.ToLower().Contains(searchTerm) ||
                        b.Car.LicensePlate.ToLower().Contains(searchTerm) ||
                        b.Status.ToLower().Contains(searchTerm) ||
                        b.TotalAmount.ToString().Contains(searchTerm) ||
                        b.PickupDateTime.ToString("dd/MM/yyyy").Contains(searchTerm) ||
                        b.ReturnDateTime.ToString("dd/MM/yyyy").Contains(searchTerm) ||
                        b.CreatedDate.ToString("dd/MM/yyyy").Contains(searchTerm)
                    );
                }

                Bookings = new ObservableCollection<Booking>(filteredBookings.ToList());
                StatusMessage = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi lọc dữ liệu";
                MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFilters()
        {
            SearchText = "";
            SelectedStatusFilter = null;
            ApplyFilters();
        }

        private void AddBooking()
        {
            try
            {
                StatusMessage = "Đang mở form thêm booking...";

                var dialogViewModel = new BookingDialogViewModel(_bookingService);
                var dialog = new View.Admin.BookingDialog(dialogViewModel);

                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    StatusMessage = "Thêm booking thành công";
                    LoadBookings();
                }
                else
                {
                    StatusMessage = "Sẵn sàng";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi thêm booking";
                MessageBox.Show($"Lỗi khi thêm booking: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditBooking()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("EditBooking method called");

                if (SelectedBooking == null)
                {
                    MessageBox.Show("Vui lòng chọn booking cần sửa!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                StatusMessage = "Đang mở form sửa booking...";

                var dialogViewModel = new BookingDialogViewModel(_bookingService, SelectedBooking);
                var dialog = new View.Admin.BookingDialog(dialogViewModel);

                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    StatusMessage = "Sửa booking thành công";
                    LoadBookings();
                }
                else
                {
                    StatusMessage = "Sẵn sàng";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi sửa booking";
                MessageBox.Show($"Lỗi khi sửa booking: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBooking()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("DeleteBooking method called");

                if (SelectedBooking == null)
                {
                    MessageBox.Show("Vui lòng chọn booking cần xóa!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var bookingToDelete = SelectedBooking;

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa booking '{bookingToDelete.BookingCode}'?\n\n" +
                    $"Khách hàng: {bookingToDelete.User.FullName}\n" +
                    $"Xe: {bookingToDelete.Car.CarModel}\n" +
                    $"Trạng thái: {bookingToDelete.Status}",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    StatusMessage = "Đang xóa booking...";

                    bool success = _bookingService.Delete(bookingToDelete.BookingId);

                    if (success)
                    {
                        _allBookings.Remove(bookingToDelete);
                        Bookings.Remove(bookingToDelete);
                        SelectedBooking = null;

                        StatusMessage = "Xóa thành công";

                        MessageBox.Show($"Xóa booking '{bookingToDelete.BookingCode}' thành công!",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        StatusMessage = "Lỗi xóa booking";

                        MessageBox.Show("Không thể xóa booking. Vui lòng kiểm tra lại và thử lại!",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                StatusMessage = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi xóa booking";

                MessageBox.Show($"Lỗi khi xóa booking: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToAdmin()
        {
            try
            {
                var currentAdminWindow = Application.Current.Windows
                    .OfType<AdminWindow>()
                    .FirstOrDefault();

                if (currentAdminWindow != null)
                {
                    currentAdminWindow.Show();
                    currentAdminWindow.WindowState = WindowState.Normal;
                    currentAdminWindow.Activate();
                }
                else
                {
                    var adminUser = new User { UserId = 1, Username = "admin" };
                    var adminWindow = new AdminWindow(adminUser);
                    adminWindow.Show();
                }

                var currentWindow = Application.Current.Windows
                    .OfType<View.Admin.BookingManagementWindow>()
                    .FirstOrDefault();

                if (currentWindow != null)
                {
                    currentWindow.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay lại Admin Window: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ForceRefreshUI()
        {
            OnPropertyChanged(nameof(SelectedBooking));
            OnPropertyChanged(nameof(CanEditBooking));
            OnPropertyChanged(nameof(CanDeleteBooking));
            CommandManager.InvalidateRequerySuggested();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
