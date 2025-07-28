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
    public class CarManagementViewModel : INotifyPropertyChanged
    {
        private readonly ICarService _carService;
        private ObservableCollection<Car> _cars;
        private Car? _selectedCar;
        private string _searchText = "";
        private List<Car> _allCars;
        private string _statusMessage = "Sẵn sàng";
        private string _selectedStatusFilter = "Tất cả trạng thái";
        private CarBrand? _selectedBrandFilter = null;
        private ObservableCollection<string> _availableStatuses;
        private ObservableCollection<CarBrand> _availableBrands;

        public ObservableCollection<Car> Cars
        {
            get => _cars;
            set
            {
                _cars = value;
                OnPropertyChanged();
            }
        }

        public Car? SelectedCar
        {
            get => _selectedCar;
            set
            {
                if (_selectedCar != value)
                {
                    _selectedCar = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanEditCar));
                    OnPropertyChanged(nameof(CanDeleteCar));
                    CommandManager.InvalidateRequerySuggested();

                    // Debug info
                    System.Diagnostics.Debug.WriteLine($"SelectedCar changed to: {_selectedCar?.CarModel ?? "null"}");
                    System.Diagnostics.Debug.WriteLine($"CanEditCar: {CanEditCar}");
                    System.Diagnostics.Debug.WriteLine($"CanDeleteCar: {CanDeleteCar}");
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
                SearchCars();
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

        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                _selectedStatusFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public CarBrand? SelectedBrandFilter
        {
            get => _selectedBrandFilter;
            set
            {
                _selectedBrandFilter = value;
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

        public ObservableCollection<CarBrand> AvailableBrands
        {
            get => _availableBrands;
            set
            {
                _availableBrands = value;
                OnPropertyChanged();
            }
        }

        public bool CanEditCar
        {
            get
            {
                var canEdit = SelectedCar != null;
                System.Diagnostics.Debug.WriteLine($"CanEditCar: {canEdit}, SelectedCar: {SelectedCar?.CarModel ?? "null"}");
                return canEdit;
            }
        }

        public bool CanDeleteCar
        {
            get
            {
                var canDelete = SelectedCar != null;
                System.Diagnostics.Debug.WriteLine($"CanDeleteCar: {canDelete}, SelectedCar: {SelectedCar?.CarModel ?? "null"}");
                return canDelete;
            }
        }

        public ICommand LoadCarsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand AddCarCommand { get; }
        public ICommand EditCarCommand { get; }
        public ICommand DeleteCarCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand BackCommand { get; }

        public CarManagementViewModel(ICarService carService)
        {
            _carService = carService;
            _cars = new ObservableCollection<Car>();
            _allCars = new List<Car>();
            _availableStatuses = new ObservableCollection<string>();
            _availableBrands = new ObservableCollection<CarBrand>();
            _selectedStatusFilter = "Tất cả trạng thái";
            _selectedBrandFilter = new CarBrand { BrandId = 0, BrandName = "Tất cả hãng xe" };

            LoadCarsCommand = new RelayCommand(LoadCars);
            SearchCommand = new RelayCommand(SearchCars);
            RefreshCommand = new RelayCommand(RefreshCars);
            AddCarCommand = new RelayCommand(AddCar);
            EditCarCommand = new RelayCommand(EditCar, () => CanEditCar);
            DeleteCarCommand = new RelayCommand(DeleteCar, () => CanDeleteCar);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            BackCommand = new RelayCommand(BackToAdmin);

            LoadAvailableStatuses();
            LoadAvailableBrands();
            LoadCars();
        }

        private void LoadCars()
        {
            try
            {
                StatusMessage = "Đang tải...";
                _allCars = _carService.GetAll().ToList();
                Cars = new ObservableCollection<Car>(_allCars);
                StatusMessage = "Sẵn sàng";

                // Reset selection
                SelectedCar = null;
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi tải dữ liệu";
                MessageBox.Show($"Lỗi khi tải danh sách xe: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchCars()
        {
            ApplyFilters();
        }

        private void RefreshCars()
        {
            SearchText = "";
            if (AvailableStatuses.Count > 0)
            {
                _selectedStatusFilter = "Tất cả trạng thái";
                OnPropertyChanged(nameof(SelectedStatusFilter));
            }
            if (AvailableBrands.Count > 0)
            {
                var defaultBrand = AvailableBrands.FirstOrDefault(b => b.BrandName == "Tất cả hãng xe");
                if (defaultBrand != null)
                {
                    _selectedBrandFilter = defaultBrand;
                    OnPropertyChanged(nameof(SelectedBrandFilter));
                }
            }
            LoadCars();
        }

        private void LoadAvailableStatuses()
        {
            try
            {
                var statuses = new List<string> { "Tất cả trạng thái" };
                statuses.AddRange(new[] { "Available", "Maintenance", "Unavailable" });
                AvailableStatuses = new ObservableCollection<string>(statuses);
                
                // Set default selected status filter
                if (AvailableStatuses.Count > 0)
                {
                    _selectedStatusFilter = "Tất cả trạng thái";
                    OnPropertyChanged(nameof(SelectedStatusFilter));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách trạng thái: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAvailableBrands()
        {
            try
            {
                using var context = new AutoRentalPrnContext();
                var brands = context.CarBrands.OrderBy(b => b.BrandName).ToList();
                var allBrands = new List<CarBrand> { new CarBrand { BrandId = 0, BrandName = "Tất cả hãng xe" } };
                allBrands.AddRange(brands);
                AvailableBrands = new ObservableCollection<CarBrand>(allBrands);
                
                // Set default selected brand filter
                if (AvailableBrands.Count > 0)
                {
                    var defaultBrand = AvailableBrands.FirstOrDefault(b => b.BrandName == "Tất cả hãng xe");
                    if (defaultBrand != null)
                    {
                        _selectedBrandFilter = defaultBrand;
                        OnPropertyChanged(nameof(SelectedBrandFilter));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách hãng xe: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                StatusMessage = "Đang lọc...";

                var filteredCars = _allCars.AsEnumerable();

                if (!string.IsNullOrEmpty(SelectedStatusFilter) && SelectedStatusFilter != "Tất cả trạng thái")
                {
                    filteredCars = filteredCars.Where(c => c.Status == SelectedStatusFilter);
                }

                if (SelectedBrandFilter != null && !string.IsNullOrEmpty(SelectedBrandFilter.BrandName) && SelectedBrandFilter.BrandName != "Tất cả hãng xe")
                {
                    filteredCars = filteredCars.Where(c => c.Brand.BrandName == SelectedBrandFilter.BrandName);
                }

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var searchTerm = SearchText.Trim().ToLower();
                    filteredCars = filteredCars.Where(c =>
                        c.CarId.ToString().Contains(searchTerm) ||
                        c.Brand.BrandName.ToLower().Contains(searchTerm) ||
                        c.CarModel.ToLower().Contains(searchTerm) ||
                        c.LicensePlate.ToLower().Contains(searchTerm) ||
                        c.Seats.ToString().Contains(searchTerm) ||
                        c.PricePerDay.ToString().Contains(searchTerm) ||
                        c.Status.ToLower().Contains(searchTerm) ||
                        c.CreatedDate.ToString("dd/MM/yyyy").Contains(searchTerm)
                    );
                }

                Cars = new ObservableCollection<Car>(filteredCars.ToList());
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
            if (AvailableStatuses.Count > 0)
            {
                _selectedStatusFilter = "Tất cả trạng thái";
                OnPropertyChanged(nameof(SelectedStatusFilter));
            }
            if (AvailableBrands.Count > 0)
            {
                var defaultBrand = AvailableBrands.FirstOrDefault(b => b.BrandName == "Tất cả hãng xe");
                if (defaultBrand != null)
                {
                    _selectedBrandFilter = defaultBrand;
                    OnPropertyChanged(nameof(SelectedBrandFilter));
                }
            }
            ApplyFilters();
        }

        private void AddCar()
        {
            try
            {
                StatusMessage = "Đang mở form thêm xe...";

                var dialogViewModel = new CarDialogViewModel(_carService);
                var dialog = new CarDialog(dialogViewModel);

                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    StatusMessage = "Thêm xe thành công";
                    LoadCars();
                }
                else
                {
                    StatusMessage = "Sẵn sàng";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi thêm xe";
                MessageBox.Show($"Lỗi khi thêm xe: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditCar()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("EditCar method called");
                
                if (SelectedCar == null)
                {
                    MessageBox.Show("Vui lòng chọn xe cần sửa!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                StatusMessage = "Đang mở form sửa xe...";

                var dialogViewModel = new CarDialogViewModel(_carService, SelectedCar);
                var dialog = new CarDialog(dialogViewModel);

                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    StatusMessage = "Sửa xe thành công";
                    LoadCars();
                }
                else
                {
                    StatusMessage = "Sẵn sàng";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi sửa xe";
                MessageBox.Show($"Lỗi khi sửa xe: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteCar()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("DeleteCar method called");
                
                if (SelectedCar == null)
                {
                    MessageBox.Show("Vui lòng chọn xe cần xóa!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var carToDelete = SelectedCar;

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa xe '{carToDelete.CarModel}'?\n\n" +
                    $"Hãng xe: {carToDelete.Brand.BrandName}\n" +
                    $"Biển số: {carToDelete.LicensePlate}\n" +
                    $"Trạng thái: {carToDelete.Status}",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    StatusMessage = "Đang xóa xe...";

                    bool success = _carService.Delete(carToDelete.CarId);

                    if (success)
                    {
                        _allCars.Remove(carToDelete);
                        Cars.Remove(carToDelete);
                        SelectedCar = null;

                        StatusMessage = "Xóa thành công";

                        MessageBox.Show($"Xóa xe '{carToDelete.CarModel}' thành công!",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        StatusMessage = "Lỗi xóa xe";

                        MessageBox.Show("Không thể xóa xe. Vui lòng kiểm tra lại và thử lại!",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                StatusMessage = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi xóa xe";

                MessageBox.Show($"Lỗi khi xóa xe: {ex.Message}",
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
                    .OfType<CarManagementWindow>()
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
            OnPropertyChanged(nameof(SelectedCar));
            OnPropertyChanged(nameof(CanEditCar));
            OnPropertyChanged(nameof(CanDeleteCar));
            CommandManager.InvalidateRequerySuggested();
            
            // Debug info
            System.Diagnostics.Debug.WriteLine($"ForceRefreshUI called. SelectedCar: {SelectedCar?.CarModel ?? "null"}");
            System.Diagnostics.Debug.WriteLine($"CanEditCar: {CanEditCar}");
            System.Diagnostics.Debug.WriteLine($"CanDeleteCar: {CanDeleteCar}");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 