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
using Microsoft.EntityFrameworkCore;

namespace AutoRental.ViewModels.Admin
{
    public class CarDialogViewModel : INotifyPropertyChanged
    {
        private readonly ICarService _carService;
        private readonly Car? _originalCar;
        private readonly bool _isEditMode;
        private Window? _dialogWindow;

        public string DialogTitle => _isEditMode ? "Sửa xe" : "Thêm xe";
        public string DialogSubtitle => _isEditMode ? "Cập nhật thông tin xe" : "Thêm xe mới vào hệ thống";
        public string DialogIcon => _isEditMode ? "✏️" : "🚗";
        public string SaveButtonText => _isEditMode ? "Cập nhật" : "Thêm";
        public bool IsEditMode => _isEditMode;
        public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

        private string _carModel = "";
        private string _licensePlate = "";
        private int _seats = 5;
        private decimal _pricePerDay = 0;
        private string _selectedStatus = "Available";
        private CarBrand? _selectedBrand;
        private string _errorMessage = "";
        private bool _hasValidationErrors = false;

        public string CarModel
        {
            get => _carModel;
            set
            {
                _carModel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public string LicensePlate
        {
            get => _licensePlate;
            set
            {
                _licensePlate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public int Seats
        {
            get => _seats;
            set
            {
                _seats = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public decimal PricePerDay
        {
            get => _pricePerDay;
            set
            {
                _pricePerDay = value;
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

        public CarBrand? SelectedBrand
        {
            get => _selectedBrand;
            set
            {
                _selectedBrand = value;
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

        public ObservableCollection<CarBrand> Brands { get; set; }
        public ObservableCollection<string> Statuses { get; set; }

        public ICommand SaveCommand { get; }

        public CarDialogViewModel(ICarService carService, Car? carToEdit = null)
        {
            _carService = carService;
            _originalCar = carToEdit;
            _isEditMode = carToEdit != null;

            SaveCommand = new RelayCommand(SaveCar, () => CanSave);

            LoadData();

            if (_isEditMode && _originalCar != null)
            {
                CarModel = _originalCar.CarModel;
                LicensePlate = _originalCar.LicensePlate;
                Seats = _originalCar.Seats;
                PricePerDay = _originalCar.PricePerDay;
                SelectedStatus = _originalCar.Status;
                SelectedBrand = _originalCar.Brand;
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
                using var context = new AutoRentalPrnContext();
                Brands = new ObservableCollection<CarBrand>(context.CarBrands.OrderBy(b => b.BrandName).ToList());
                Statuses = new ObservableCollection<string> { "Available", "Maintenance", "Unavailable" };
                
                // Set default values if not in edit mode
                if (!_isEditMode)
                {
                    if (Brands.Count > 0)
                        SelectedBrand = Brands.First();
                    SelectedStatus = "Available";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsFormValid()
        {
            if (string.IsNullOrWhiteSpace(CarModel))
                return false;

            if (string.IsNullOrWhiteSpace(LicensePlate))
                return false;

            if (SelectedBrand == null)
                return false;

            if (Seats <= 0)
                return false;

            if (PricePerDay <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(SelectedStatus))
                return false;

            return true;
        }

        private void SaveCar()
        {
            try
            {
                ErrorMessage = "";

                if (!IsFormValid())
                {
                    return;
                }

                Car car;
                bool success;

                if (_isEditMode && _originalCar != null)
                {
                    car = _originalCar;
                    car.CarModel = CarModel;
                    car.LicensePlate = LicensePlate;
                    car.BrandId = SelectedBrand.BrandId;
                    car.Seats = Seats;
                    car.PricePerDay = PricePerDay;
                    car.Status = SelectedStatus;

                    success = _carService.Update(car);
                }
                else
                {
                    car = new Car
                    {
                        CarModel = CarModel,
                        LicensePlate = LicensePlate,
                        BrandId = SelectedBrand.BrandId,
                        Seats = Seats,
                        PricePerDay = PricePerDay,
                        Status = SelectedStatus,
                        CreatedDate = DateTime.Now
                    };

                    success = _carService.Add(car);
                }

                if (success)
                {
                    string successMessage = _isEditMode
                        ? $"Cập nhật xe '{car.CarModel}' thành công!"
                        : $"Thêm xe '{car.CarModel}' thành công!";

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
                        ? "Không thể cập nhật xe. Vui lòng kiểm tra lại thông tin và thử lại!"
                        : "Không thể thêm xe. Vui lòng kiểm tra lại thông tin và thử lại!";

                    MessageBox.Show(errorMessage,
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = _isEditMode
                    ? $"Lỗi khi cập nhật xe: {ex.Message}"
                    : $"Lỗi khi thêm xe: {ex.Message}";

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