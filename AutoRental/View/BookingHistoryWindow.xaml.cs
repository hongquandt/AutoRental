using System.Windows;
using BusinessObjects;
using AutoRental.ViewModels;
using System.Windows.Controls;
using System;

namespace AutoRental
{
    public partial class BookingHistoryWindow : Window
    {
        private readonly User _user;
        private readonly BookingViewModel _viewModel;
        public BookingHistoryWindow(User user)
        {
            InitializeComponent();
            _user = user;
            _viewModel = new BookingViewModel();
            _viewModel.LoadByUser(_user.UserId);
            this.DataContext = _viewModel;
        }
        
        private void UpdateCancelButtonState()
        {
            if (_viewModel.SelectedBooking != null && _viewModel.SelectedBooking.Status == "Confirmed")
            {
                btnCancelBooking.IsEnabled = true;
            }
            else
            {
                btnCancelBooking.IsEnabled = false;
            }
        }
        
        private void DgBookings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCancelButtonState();
        }

        private void BtnCancelBooking_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra đã chọn booking chưa
            if (_viewModel.SelectedBooking == null)
            {
                MessageBox.Show("Vui lòng chọn một booking để huỷ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Kiểm tra trạng thái booking
            if (_viewModel.SelectedBooking.Status != "Confirmed")
            {
                MessageBox.Show("Chỉ có thể huỷ những booking có trạng thái 'Confirmed'!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Xác nhận huỷ - kiểm tra Car có null không
            string message;
            if (_viewModel.SelectedBooking.Car == null)
            {
                message = $"Bạn có chắc muốn huỷ đặt xe (Mã: {_viewModel.SelectedBooking.BookingCode})?";
            }
            else
            {
                message = $"Bạn có chắc muốn huỷ đặt xe {_viewModel.SelectedBooking.Car.CarModel} ({_viewModel.SelectedBooking.BookingCode})?";
            }

            if (MessageBox.Show(message, "Xác nhận huỷ đặt xe", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Gọi hàm huỷ booking
                _viewModel.CancelSelected();
                
                // Thông báo và cập nhật UI
                MessageBox.Show("Đã huỷ đặt xe thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                _viewModel.LoadByUser(_user.UserId);
                UpdateCancelButtonState();
            }
        }
    }
} 