using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using AutoRental.ViewModels.Admin;
using Service.Implementations;
using Service.Interfaces;
using DataAccessObjects.Repositories.Implementations;
using DataAccessObjects.Repositories.Interfaces;

namespace AutoRental.View.Admin
{
    public partial class BookingManagementWindow : Window
    {
        private DispatcherTimer _refreshTimer;

        public BookingManagementWindow()
        {
            InitializeComponent();

            IBookingRepository bookingRepo = new BookingRepository();
            IBookingService bookingService = new BookingService(bookingRepo);

            var viewModel = new BookingManagementViewModel(bookingService);
            this.DataContext = viewModel;

            // Setup timer to refresh UI
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromMilliseconds(100);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private void bookingDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // Đảm bảo DataGrid không tự động tạo cột
            bookingDataGrid.AutoGenerateColumns = false;

            // Đảm bảo có thể chọn dòng
            bookingDataGrid.SelectionMode = DataGridSelectionMode.Single;
            bookingDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;

            // Thêm event handler để debug selection
            bookingDataGrid.SelectionChanged += BookingDataGrid_SelectionChanged;

            // Force binding update
            var binding = bookingDataGrid.GetBindingExpression(DataGrid.SelectedItemProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }

            // Force refresh UI after a short delay
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (DataContext is BookingManagementViewModel viewModel)
                {
                    viewModel.ForceRefreshUI();
                }
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void BookingDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is BookingManagementViewModel viewModel)
            {
                // Force update SelectedBooking property
                var selectedItem = bookingDataGrid.SelectedItem as BusinessObjects.Booking;
                if (selectedItem != viewModel.SelectedBooking)
                {
                    viewModel.SelectedBooking = selectedItem;
                }

                // Force refresh UI
                viewModel.ForceRefreshUI();

                System.Diagnostics.Debug.WriteLine($"Selection changed. SelectedBooking: {viewModel.SelectedBooking?.BookingCode ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"CanEditBooking: {viewModel.CanEditBooking}");
                System.Diagnostics.Debug.WriteLine($"CanDeleteBooking: {viewModel.CanDeleteBooking}");
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BookingManagementViewModel viewModel)
            {
                // Check if a booking is selected
                if (viewModel.SelectedBooking == null)
                {
                    MessageBox.Show("Vui lòng chọn booking cần sửa!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                viewModel.EditBookingCommand.Execute(null);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BookingManagementViewModel viewModel)
            {
                // Check if a booking is selected
                if (viewModel.SelectedBooking == null)
                {
                    MessageBox.Show("Vui lòng chọn booking cần xóa!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                viewModel.DeleteBookingCommand.Execute(null);
            }
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (DataContext is BookingManagementViewModel viewModel)
            {
                // Force update SelectedBooking from DataGrid
                var selectedItem = bookingDataGrid.SelectedItem as BusinessObjects.Booking;
                if (selectedItem != viewModel.SelectedBooking)
                {
                    viewModel.SelectedBooking = selectedItem;
                }

                // Force refresh UI periodically
                viewModel.ForceRefreshUI();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_refreshTimer != null)
            {
                _refreshTimer.Stop();
                _refreshTimer.Tick -= RefreshTimer_Tick;
            }
            base.OnClosed(e);
        }


    }
}
