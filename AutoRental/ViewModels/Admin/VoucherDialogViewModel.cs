using BusinessObjects;
using Service.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;

namespace AutoRental.ViewModels.Admin
{
    public class VoucherDialogViewModel : INotifyPropertyChanged
    {
        private readonly IDiscountService _discountService;
        private readonly Discount? _originalDiscount;
        private readonly bool _isEditMode;
        private Window? _dialogWindow;

        // Properties
        public string DialogTitle => _isEditMode ? "Sửa Voucher" : "Thêm Voucher";
        public string DialogSubtitle => _isEditMode ? "Cập nhật thông tin voucher" : "Thêm voucher mới vào hệ thống";
        public string DialogIcon => _isEditMode ? "✏️" : "➕";
        public string SaveButtonText => _isEditMode ? "Cập nhật" : "Thêm";

        // Form Properties
        private string _discountName = "";
        private decimal _discountValue = 0;
        private string _errorMessage = "";
        private bool _hasValidationErrors = false;

        public string DiscountName 
        { 
            get => _discountName; 
            set 
            { 
                _discountName = value; 
                OnPropertyChanged();
            } 
        }

        public decimal DiscountValue 
        { 
            get => _discountValue; 
            set 
            { 
                _discountValue = value; 
                OnPropertyChanged();
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

        // Commands
        public ICommand SaveCommand { get; }

        public VoucherDialogViewModel(IDiscountService discountService, Discount? discountToEdit = null)
        {
            _discountService = discountService;
            _originalDiscount = discountToEdit;
            _isEditMode = discountToEdit != null;

            // Initialize Commands
            SaveCommand = new RelayCommand(SaveDiscount, () => CanSave);

            // If editing, populate form with existing data
            if (_isEditMode && _originalDiscount != null)
            {
                DiscountName = _originalDiscount.DiscountName;
                DiscountValue = _originalDiscount.DiscountValue;
            }
        }

        public void SetDialogWindow(Window window)
        {
            _dialogWindow = window;
        }

        private bool IsFormValid()
        {
            // Check required fields
            if (string.IsNullOrWhiteSpace(DiscountName))
                return false;

            if (DiscountName.Length < 2)
                return false;

            if (DiscountName.Length > 100)
                return false;

            // Check discount value range
            if (DiscountValue < 0 || DiscountValue > 100)
                return false;

            return true;
        }

        private void SaveDiscount()
        {
            try
            {
                // Clear previous error
                ErrorMessage = "";

                // Validate form
                if (!IsFormValid())
                {
                    return;
                }

                // Create or update discount
                Discount discount;
                bool success;

                if (_isEditMode && _originalDiscount != null)
                {
                    // Update existing discount
                    discount = _originalDiscount;
                    discount.DiscountName = DiscountName;
                    discount.DiscountValue = DiscountValue;

                    success = _discountService.Update(discount);
                }
                else
                {
                    // Create new discount
                    discount = new Discount
                    {
                        DiscountName = DiscountName,
                        DiscountValue = DiscountValue
                    };

                    success = _discountService.Add(discount);
                }

                if (success)
                {
                    // Show success message
                    string successMessage = _isEditMode 
                        ? $"Cập nhật voucher '{discount.DiscountName}' thành công!"
                        : $"Thêm voucher '{discount.DiscountName}' thành công!";
                    
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
                        ? "Không thể cập nhật voucher. Vui lòng kiểm tra lại thông tin và thử lại!"
                        : "Không thể thêm voucher. Vui lòng kiểm tra lại thông tin và thử lại!";
                    
                    MessageBox.Show(errorMessage, 
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Show exception message
                string exceptionMessage = _isEditMode 
                    ? $"Lỗi khi cập nhật voucher: {ex.Message}"
                    : $"Lỗi khi thêm voucher: {ex.Message}";
                
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