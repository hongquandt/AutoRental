using BusinessObjects;
using Service.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AutoRental.ViewModels.Admin
{
    public class VoucherManagementViewModel : INotifyPropertyChanged
    {
        private readonly IDiscountService _discountService;
        private ObservableCollection<Discount> _discounts;
        private Discount? _selectedDiscount;
        private string _searchText = "";
        private List<Discount> _allDiscounts; // Cache tất cả discounts để search nhanh hơn
        private string _statusMessage = "Sẵn sàng";
        private string _selectedSortBy = "Tên (A-Z)";
        private ObservableCollection<string> _sortOptions;

        // Properties
        public ObservableCollection<Discount> Discounts
        {
            get => _discounts;
            set
            {
                _discounts = value;
                OnPropertyChanged();
            }
        }

        public Discount? SelectedDiscount
        {
            get => _selectedDiscount;
            set
            {
                _selectedDiscount = value;
                OnPropertyChanged();
                // Cập nhật trạng thái buttons khi chọn discount
                OnPropertyChanged(nameof(CanEditDiscount));
                OnPropertyChanged(nameof(CanDeleteDiscount));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                // Real-time search khi text thay đổi
                SearchDiscounts();
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



        public string SelectedSortBy
        {
            get => _selectedSortBy;
            set
            {
                _selectedSortBy = value;
                OnPropertyChanged();
                // Apply sorting when selection changes
                ApplyFilters();
            }
        }

        public ObservableCollection<string> SortOptions
        {
            get => _sortOptions;
            set
            {
                _sortOptions = value;
                OnPropertyChanged();
            }
        }

        // Computed Properties for Button States
        public bool CanEditDiscount => SelectedDiscount != null;
        public bool CanDeleteDiscount => SelectedDiscount != null;

        // Commands
        public ICommand LoadDiscountsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand AddDiscountCommand { get; }
        public ICommand EditDiscountCommand { get; }
        public ICommand DeleteDiscountCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand BackCommand { get; }

        public VoucherManagementViewModel(IDiscountService discountService)
        {
            _discountService = discountService;
            _discounts = new ObservableCollection<Discount>();
            _allDiscounts = new List<Discount>();
            _sortOptions = new ObservableCollection<string>();

            // Initialize Commands
            LoadDiscountsCommand = new RelayCommand(LoadDiscounts);
            SearchCommand = new RelayCommand(SearchDiscounts);
            RefreshCommand = new RelayCommand(RefreshDiscounts);
            AddDiscountCommand = new RelayCommand(AddDiscount);
            EditDiscountCommand = new RelayCommand(EditDiscount, () => CanEditDiscount);
            DeleteDiscountCommand = new RelayCommand(DeleteDiscount, () => CanDeleteDiscount);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            BackCommand = new RelayCommand(BackToAdmin);

            // Load discounts and sort options when ViewModel is created
            LoadDiscounts();
            LoadSortOptions();
        }

        // Command Methods
        private void LoadDiscounts()
        {
            try
            {
                StatusMessage = "Đang tải...";
                _allDiscounts = _discountService.GetAll().ToList(); // Cache tất cả discounts
                Discounts = new ObservableCollection<Discount>(_allDiscounts);



                StatusMessage = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi tải dữ liệu";
                System.Windows.MessageBox.Show($"Lỗi khi tải danh sách voucher: {ex.Message}",
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void SearchDiscounts()
        {
            // Use ApplyFilters to handle both search and value filter
            ApplyFilters();
        }

        private void RefreshDiscounts()
        {
            SearchText = ""; // Clear search
            SelectedSortBy = "Tên (A-Z)"; // Reset sort to default
            LoadDiscounts(); // Load lại từ database
        }



        private void LoadSortOptions()
        {
            try
            {
                SortOptions = new ObservableCollection<string>
                {
                    "Tên (A-Z)",
                    "Tên (Z-A)",
                    "Giá tăng dần",
                    "Giá giảm dần"
                };
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi tải tùy chọn sắp xếp: {ex.Message}",
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                StatusMessage = "Đang lọc...";

                var filteredDiscounts = _allDiscounts.AsEnumerable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var searchTerm = SearchText.Trim().ToLower();
                    filteredDiscounts = filteredDiscounts.Where(d =>
                        // 1. Search theo DiscountId
                        d.DiscountId.ToString().Contains(searchTerm) ||
                        // 2. Search theo DiscountName
                        (d.DiscountName?.ToLower().Contains(searchTerm) == true) ||
                        // 3. Search theo DiscountValue
                        d.DiscountValue.ToString().Contains(searchTerm) ||
                        d.DiscountValue.ToString("0.##").Contains(searchTerm)
                    );
                }

                // Apply sorting
                filteredDiscounts = ApplySorting(filteredDiscounts);

                Discounts = new ObservableCollection<Discount>(filteredDiscounts.ToList());
                StatusMessage = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi lọc dữ liệu";
                System.Windows.MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}",
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private IEnumerable<Discount> ApplySorting(IEnumerable<Discount> discounts)
        {
            switch (SelectedSortBy)
            {
                case "Tên (A-Z)":
                    return discounts.OrderBy(d => d.DiscountName);
                case "Tên (Z-A)":
                    return discounts.OrderByDescending(d => d.DiscountName);
                case "Giá tăng dần":
                    return discounts.OrderBy(d => d.DiscountValue);
                case "Giá giảm dần":
                    return discounts.OrderByDescending(d => d.DiscountValue);
                default:
                    return discounts.OrderBy(d => d.DiscountName); // Default sort
            }
        }

        private void ClearFilters()
        {
            SearchText = "";
            SelectedSortBy = "Tên (A-Z)";
            ApplyFilters();
        }

        private void AddDiscount()
        {
            try
            {
                StatusMessage = "Đang mở form thêm voucher...";

                // Tạo ViewModel cho dialog thêm discount
                var dialogViewModel = new VoucherDialogViewModel(_discountService);
                var dialog = new View.Admin.VoucherDialog(dialogViewModel);

                // Hiển thị dialog modal
                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    // Discount được thêm thành công, refresh danh sách
                    StatusMessage = "Thêm voucher thành công";
                    LoadDiscounts();
                }
                else
                {
                    StatusMessage = "Sẵn sàng";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi thêm voucher";
                System.Windows.MessageBox.Show($"Lỗi khi thêm voucher: {ex.Message}",
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void EditDiscount()
        {
            try
            {
                if (SelectedDiscount == null)
                {
                    System.Windows.MessageBox.Show("Vui lòng chọn voucher cần sửa!",
                        "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                StatusMessage = "Đang mở form sửa voucher...";

                // Tạo ViewModel cho dialog sửa discount
                var dialogViewModel = new VoucherDialogViewModel(_discountService, SelectedDiscount);
                var dialog = new View.Admin.VoucherDialog(dialogViewModel);

                // Hiển thị dialog modal
                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    // Discount được sửa thành công, refresh danh sách
                    StatusMessage = "Sửa voucher thành công";
                    LoadDiscounts();
                }
                else
                {
                    StatusMessage = "Sẵn sàng";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi sửa voucher";
                System.Windows.MessageBox.Show($"Lỗi khi sửa voucher: {ex.Message}",
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void DeleteDiscount()
        {
            try
            {
                if (SelectedDiscount == null)
                {
                    System.Windows.MessageBox.Show("Vui lòng chọn voucher cần xóa!",
                        "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                // Xác nhận xóa
                var result = System.Windows.MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa voucher '{SelectedDiscount.DiscountName}'?\n\n" +
                    $"ID: {SelectedDiscount.DiscountId}\n" +
                    $"Giá trị: {SelectedDiscount.DiscountValue}%",
                    "Xác nhận xóa",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    StatusMessage = "Đang xóa voucher...";

                    bool success = _discountService.Delete(SelectedDiscount.DiscountId);

                    if (success)
                    {
                        StatusMessage = "Xóa voucher thành công";
                        LoadDiscounts(); // Refresh danh sách

                        System.Windows.MessageBox.Show($"Đã xóa voucher '{SelectedDiscount.DiscountName}' thành công!",
                            "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                    else
                    {
                        StatusMessage = "Lỗi xóa voucher";
                        System.Windows.MessageBox.Show("Không thể xóa voucher. Vui lòng thử lại!",
                            "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
                else
                {
                    StatusMessage = "Sẵn sàng";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi xóa voucher";
                System.Windows.MessageBox.Show($"Lỗi khi xóa voucher: {ex.Message}",
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void BackToAdmin()
        {
            try
            {
                // Tìm AdminWindow hiện tại để lấy thông tin admin
                var currentAdminWindow = System.Windows.Application.Current.Windows
                    .OfType<AdminWindow>()
                    .FirstOrDefault();

                if (currentAdminWindow != null)
                {
                    // Mở lại AdminWindow hiện tại
                    currentAdminWindow.Show();
                    currentAdminWindow.WindowState = WindowState.Normal;
                    currentAdminWindow.Activate();
                }
                else
                {
                    // Nếu không tìm thấy AdminWindow, tạo mới với admin mặc định
                    // (Trong thực tế, bạn có thể cần lưu thông tin admin trong session)
                    var adminUser = new BusinessObjects.User { UserId = 1, Username = "admin" }; // Giá trị mặc định
                    var adminWindow = new AdminWindow(adminUser);
                    adminWindow.Show();
                }

                // Đóng VoucherManagementWindow hiện tại
                var currentWindow = System.Windows.Application.Current.Windows
                    .OfType<View.Admin.VoucherManagementWindow>()
                    .FirstOrDefault();

                if (currentWindow != null)
                {
                    currentWindow.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi quay lại Admin Window: {ex.Message}",
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
