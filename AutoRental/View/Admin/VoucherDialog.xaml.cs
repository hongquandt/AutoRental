using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using AutoRental.ViewModels.Admin;
using System;
using System.Windows.Threading;

namespace AutoRental.View.Admin
{
    public partial class VoucherDialog : Window
    {
        public VoucherDialog(VoucherDialogViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            viewModel.SetDialogWindow(this);
        }

        private void VoucherDialog_Loaded(object sender, RoutedEventArgs e)
        {
            // Trigger initial validation for all fields
            Dispatcher.BeginInvoke(new Action(() => 
            {
                ValidateDiscountNameRealTime();
                ValidateDiscountValueRealTime();
            }), DispatcherPriority.Loaded);
        }

        // Discount Name Validation
        private void txtDiscountName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateDiscountNameRealTime();
        }

        private void txtDiscountName_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateDiscountNameRealTime();
        }

        private void ValidateDiscountNameRealTime()
        {
            string discountName = txtDiscountName.Text;
            
            if (string.IsNullOrWhiteSpace(discountName))
            {
                ShowFieldError(txtDiscountNameError, "Tên voucher không được để trống!");
                return;
            }

            if (discountName.Length < 2)
            {
                ShowFieldError(txtDiscountNameError, "Tên voucher phải có ít nhất 2 ký tự!");
                return;
            }

            if (discountName.Length > 100)
            {
                ShowFieldError(txtDiscountNameError, "Tên voucher không được quá 100 ký tự!");
                return;
            }

            HideFieldError(txtDiscountNameError);
        }

        // Discount Value Validation
        private void txtDiscountValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateDiscountValueRealTime();
        }

        private void txtDiscountValue_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateDiscountValueRealTime();
        }

        private void ValidateDiscountValueRealTime()
        {
            string discountValueText = txtDiscountValue.Text;
            
            if (string.IsNullOrWhiteSpace(discountValueText))
            {
                ShowFieldError(txtDiscountValueError, "Giá trị giảm giá không được để trống!");
                return;
            }

            if (!decimal.TryParse(discountValueText, out decimal discountValue))
            {
                ShowFieldError(txtDiscountValueError, "Giá trị giảm giá phải là số!");
                return;
            }

            if (discountValue < 0)
            {
                ShowFieldError(txtDiscountValueError, "Giá trị giảm giá không được âm!");
                return;
            }

            if (discountValue > 100)
            {
                ShowFieldError(txtDiscountValueError, "Giá trị giảm giá không được quá 100%!");
                return;
            }

            HideFieldError(txtDiscountValueError);
        }

        // Helper methods
        private void ShowFieldError(TextBlock errorBlock, string message)
        {
            errorBlock.Text = message;
            errorBlock.Visibility = Visibility.Visible;
            
            // Update ViewModel validation state
            if (DataContext is VoucherDialogViewModel viewModel)
            {
                viewModel.HasValidationErrors = true;
            }
        }

        private void HideFieldError(TextBlock errorBlock)
        {
            errorBlock.Visibility = Visibility.Collapsed;
            
            // Check if all errors are hidden
            CheckAllErrorsHidden();
        }

        private void CheckAllErrorsHidden()
        {
            // Check if all error blocks are hidden
            bool allHidden = txtDiscountNameError.Visibility == Visibility.Collapsed &&
                           txtDiscountValueError.Visibility == Visibility.Collapsed;

            // Update ViewModel validation state
            if (DataContext is VoucherDialogViewModel viewModel)
            {
                viewModel.HasValidationErrors = !allHidden;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Gọi SaveCommand từ ViewModel
            if (DataContext is VoucherDialogViewModel viewModel)
            {
                viewModel.SaveCommand.Execute(null);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
} 