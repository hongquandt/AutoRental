using BusinessObjects;
using Service.Interfaces;
using Service.Implementations;
using DataAccessObjects.Repositories.Implementations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System;
using System.Linq;
using AutoRental.View.Admin;
using AutoRental;

namespace AutoRental.ViewModels.Admin
{
    public class BookingDialogViewModel : INotifyPropertyChanged
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;
        private readonly Booking? _originalBooking;
        private readonly bool _isEditMode;
        private Window? _dialogWindow;

        public string DialogTitle => _isEditMode ? "Sửa Booking" : "Thêm Booking";
        public string DialogSubtitle => _isEditMode ? "Cập nhật thông tin booking" : "Thêm booking mới vào hệ thống";
        public string DialogIcon => _isEditMode ? "✏️" : "➕";
        public string SaveButtonText => _isEditMode ? "Cập nhật" : "Thêm";
        public bool IsEditMode => _isEditMode;
        public bool IsBookingCodeEnabled => !_isEditMode;
        public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

        private string _bookingCode = "";
        private string _customerName = "";
        private Car? _selectedCar;
        private DateTime _pickupDateTime = DateTime.Today;
        private DateTime _returnDateTime = DateTime.Today.AddDays(1);
        private decimal _totalAmount = 0;
        private string _selectedStatus = "Pending";
        private string _errorMessage = "";
        private bool _hasValidationErrors = false;

        public string BookingCode
        {
            get => _bookingCode;
            set
            {
                _bookingCode = value;
                OnPropertyChanged();
            }
        }

        public string CustomerName
        {
            get => _customerName;
            set
            {
                _customerName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public Car? SelectedCar
        {
            get => _selectedCar;
            set
            {
                _selectedCar = value;
                OnPropertyChanged();
                // Chỉ tính toán giá mặc định khi chọn xe, không ghi đè giá đã nhập
                if (_selectedCar != null && _totalAmount == 0)
                {
                    CalculateDefaultTotalAmount();
                }
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public DateTime PickupDateTime
        {
            get => _pickupDateTime;
            set
            {
                _pickupDateTime = value;
                OnPropertyChanged();
                // Chỉ tính toán giá mặc định khi thay đổi ngày, không ghi đè giá đã nhập
                if (_selectedCar != null && _totalAmount == 0)
                {
                    CalculateDefaultTotalAmount();
                }
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public DateTime ReturnDateTime
        {
            get => _returnDateTime;
            set
            {
                _returnDateTime = value;
                OnPropertyChanged();
                // Chỉ tính toán giá mặc định khi thay đổi ngày, không ghi đè giá đã nhập
                if (_selectedCar != null && _totalAmount == 0)
                {
                    CalculateDefaultTotalAmount();
                }
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                _totalAmount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasErrorMessage));
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

        public ObservableCollection<Car> Cars { get; set; }
        public ObservableCollection<string> Statuses { get; set; }

        public ICommand SaveCommand { get; }

        public BookingDialogViewModel(IBookingService bookingService, Booking? bookingToEdit = null)
        {
            _bookingService = bookingService;
            _userService = new UserService(new UserRepository());
            _originalBooking = bookingToEdit;
            _isEditMode = bookingToEdit != null;

            SaveCommand = new RelayCommand(SaveBooking, () => CanSave);

            LoadData();

            if (_isEditMode && _originalBooking != null)
            {
                BookingCode = _originalBooking.BookingCode;
                CustomerName = _originalBooking.User?.FullName ?? "";
                SelectedCar = _originalBooking.Car;
                PickupDateTime = _originalBooking.PickupDateTime.Date;
                ReturnDateTime = _originalBooking.ReturnDateTime.Date;
                TotalAmount = _originalBooking.TotalAmount;
                SelectedStatus = _originalBooking.Status;
            }
            else
            {
                BookingCode = _bookingService.GenerateBookingCode();
                // Không tự động tính giá khi thêm mới, để admin có thể nhập giá tùy chỉnh
            }

            HasValidationErrors = false;
        }

        public void SetDialogWindow(Window window)
        {
            _dialogWindow = window;
        }

        private void LoadData()
        {
            try
            {
                Cars = new ObservableCollection<Car>(GetAvailableCars());
                Statuses = new ObservableCollection<string> { "Pending", "Confirmed", "Cancelled", "Completed" };
                
                // Chỉ tính giá mặc định nếu không phải edit mode và chưa có giá
                if (!_isEditMode && _totalAmount == 0 && Cars.Count > 0)
                {
                    // Không tự động tính giá, để admin nhập thủ công
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private IEnumerable<Car> GetAvailableCars()
        {
            try
            {
                using var context = new AutoRentalPrnContext();
                return context.Cars.ToList(); // Load tất cả cars để test
            }
            catch
            {
                return new List<Car>();
            }
        }

        private void CalculateDefaultTotalAmount()
        {
            if (SelectedCar != null)
            {
                var days = (ReturnDateTime.Date - PickupDateTime.Date).Days;
                if (days <= 0) days = 1; // Ít nhất 1 ngày
                TotalAmount = days * SelectedCar.PricePerDay;
                
                // Debug info
                System.Diagnostics.Debug.WriteLine($"Car: {SelectedCar.CarModel}, PricePerDay: {SelectedCar.PricePerDay}");
                System.Diagnostics.Debug.WriteLine($"Days: {days}, TotalAmount: {TotalAmount}");
            }
            else
            {
                TotalAmount = 0;
                System.Diagnostics.Debug.WriteLine("No car selected, TotalAmount: 0");
            }
        }

        private void CalculateTotalAmount()
        {
            // Phương thức này giữ lại để tương thích, nhưng không tự động gọi
            CalculateDefaultTotalAmount();
        }

        public void RecalculateTotalAmount()
        {
            CalculateDefaultTotalAmount();
        }

        private bool IsFormValid()
        {
            if (string.IsNullOrWhiteSpace(BookingCode))
                return false;

            if (string.IsNullOrWhiteSpace(CustomerName))
                return false;

            if (SelectedCar == null)
                return false;

            if (ReturnDateTime.Date <= PickupDateTime.Date)
                return false;

            if (TotalAmount <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(SelectedStatus))
                return false;

            return true;
        }

        private void SaveBooking()
        {
            try
            {
                ErrorMessage = "";

                if (!IsFormValid())
                {
                    return;
                }

                Booking booking;
                bool success;

                if (_isEditMode && _originalBooking != null)
                {
                    booking = _originalBooking;
                    booking.BookingCode = BookingCode;
                    booking.CarId = SelectedCar.CarId;
                    booking.PickupDateTime = PickupDateTime.Date.AddHours(8); // Mặc định 8h sáng
                    booking.ReturnDateTime = ReturnDateTime.Date.AddHours(8); // Mặc định 8h sáng
                    booking.TotalAmount = TotalAmount;
                    booking.Status = SelectedStatus;

                    success = _bookingService.Update(booking);
                }
                else
                {
                    // Tạo User mới từ CustomerName
                    var newUser = new User
                    {
                        Username = $"user_{DateTime.Now.Ticks}",
                        FullName = CustomerName,
                        Email = $"customer_{DateTime.Now.Ticks}@example.com",
                        Password = "123456", // Mật khẩu mặc định
                        PhoneNumber = "",
                        RoleId = 1, // Role User
                        CreatedDate = DateTime.Now
                    };

                    bool userAdded = _userService.Add(newUser);
                    if (!userAdded)
                    {
                        MessageBox.Show("Không thể tạo thông tin khách hàng. Vui lòng thử lại!",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    booking = new Booking
                    {
                        BookingCode = BookingCode,
                        UserId = newUser.UserId,
                        CarId = SelectedCar.CarId,
                        PickupDateTime = PickupDateTime.Date.AddHours(8), // Mặc định 8h sáng
                        ReturnDateTime = ReturnDateTime.Date.AddHours(8), // Mặc định 8h sáng
                        TotalAmount = TotalAmount,
                        Status = SelectedStatus,
                        CreatedDate = DateTime.Now
                    };

                    success = _bookingService.Add(booking);
                }

                if (success)
                {
                    string successMessage = _isEditMode
                        ? $"Cập nhật booking '{booking.BookingCode}' thành công!"
                        : $"Thêm booking '{booking.BookingCode}' thành công!";

                    MessageBox.Show(successMessage,
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (_dialogWindow != null)
                    {
                        _dialogWindow.DialogResult = true;
                        _dialogWindow.Close();
                    }
                }
                else
                {
                    string errorMessage = _isEditMode
                        ? "Không thể cập nhật booking. Vui lòng kiểm tra lại thông tin và thử lại!"
                        : "Không thể thêm booking. Vui lòng kiểm tra lại thông tin và thử lại!";

                    MessageBox.Show(errorMessage,
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = _isEditMode
                    ? $"Lỗi khi cập nhật booking: {ex.Message}"
                    : $"Lỗi khi thêm booking: {ex.Message}";

                MessageBox.Show(exceptionMessage,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 