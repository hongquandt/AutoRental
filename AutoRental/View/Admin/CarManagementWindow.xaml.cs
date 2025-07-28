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
    public partial class CarManagementWindow : Window
    {
        private DispatcherTimer _refreshTimer;

        public CarManagementWindow()
        {
            InitializeComponent();

            ICarRepository carRepo = new CarRepository();
            ICarService carService = new CarService(carRepo);

            var viewModel = new CarManagementViewModel(carService);
            this.DataContext = viewModel;

            // Setup timer to refresh UI
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromMilliseconds(100);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private void carDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // Đảm bảo DataGrid không tự động tạo cột
            carDataGrid.AutoGenerateColumns = false;

            // Đảm bảo có thể chọn dòng
            carDataGrid.SelectionMode = DataGridSelectionMode.Single;
            carDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;

            // Force binding update
            var binding = carDataGrid.GetBindingExpression(DataGrid.SelectedItemProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }

            // Force refresh UI after a short delay
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (DataContext is CarManagementViewModel viewModel)
                {
                    viewModel.ForceRefreshUI();
                }
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void CarDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is CarManagementViewModel viewModel)
            {
                // Force update SelectedCar property
                var selectedItem = carDataGrid.SelectedItem as BusinessObjects.Car;
                if (selectedItem != viewModel.SelectedCar)
                {
                    viewModel.SelectedCar = selectedItem;
                }

                // Force refresh UI
                viewModel.ForceRefreshUI();

                System.Diagnostics.Debug.WriteLine($"Selection changed. SelectedCar: {viewModel.SelectedCar?.CarModel ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"CanEditCar: {viewModel.CanEditCar}");
                System.Diagnostics.Debug.WriteLine($"CanDeleteCar: {viewModel.CanDeleteCar}");
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CarManagementViewModel viewModel)
            {
                // Check if a car is selected
                if (viewModel.SelectedCar == null)
                {
                    MessageBox.Show("Vui lòng chọn xe cần sửa!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                viewModel.EditCarCommand.Execute(null);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CarManagementViewModel viewModel)
            {
                // Check if a car is selected
                if (viewModel.SelectedCar == null)
                {
                    MessageBox.Show("Vui lòng chọn xe cần xóa!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                viewModel.DeleteCarCommand.Execute(null);
            }
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (DataContext is CarManagementViewModel viewModel)
            {
                // Force update SelectedCar from DataGrid
                var selectedItem = carDataGrid.SelectedItem as BusinessObjects.Car;
                if (selectedItem != viewModel.SelectedCar)
                {
                    viewModel.SelectedCar = selectedItem;
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