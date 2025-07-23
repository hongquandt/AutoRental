using System.Windows;
using BusinessObjects;
using AutoRental.ViewModels;
using System.Windows.Controls;

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
            if (_viewModel.SelectedBooking == null)
            {
                MessageBox.Show("Vui lòng chọn một booking để huỷ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            if (_viewModel.SelectedBooking.Status != "Confirmed")
            {
                MessageBox.Show("Chỉ có thể huỷ những booking có trạng thái 'Confirmed'!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc muốn huỷ đặt xe {_viewModel.SelectedBooking.Car.CarModel} ({_viewModel.SelectedBooking.BookingCode})?", 
                "Xác nhận huỷ đặt xe", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _viewModel.CancelSelected();
                MessageBox.Show("Đã huỷ đặt xe thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                // Cập nhật lại danh sách từ database để đảm bảo dữ liệu mới nhất
                _viewModel.LoadByUser(_user.UserId);
                UpdateCancelButtonState();
            }
        }
    }
} 