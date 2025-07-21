using System.Windows;
using BusinessObjects;
using AutoRental.ViewModels;

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

        private void BtnCancelBooking_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedBooking == null || _viewModel.SelectedBooking.Status != "Confirmed") return;
            if (MessageBox.Show("Bạn có chắc muốn huỷ đặt xe này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _viewModel.CancelSelected();
                _viewModel.LoadByUser(_user.UserId);
            }
        }
    }
} 