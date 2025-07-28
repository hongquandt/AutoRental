using System.Windows;
using BusinessObjects;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows.Input;
using System;


namespace AutoRental
{
    public partial class CarListWindow : Window, INotifyPropertyChanged
    {
        private readonly User _currentUser;
        private List<Car> _cars;
        private List<CarBrand> _brands;
        private int CurrentPage = 1;
        private int PageSize = 12;
        private int TotalPages = 1;
        private List<Car> _pagedCars = new List<Car>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public CarListWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadBrands();
            LoadCars();
        }

        private void LoadBrands()
        {
            using (var db = new AutoRentalPrnContext())
            {
                _brands = db.CarBrands.OrderBy(b => b.BrandName).ToList();
                cbBrand.ItemsSource = _brands;
                cbBrand.DisplayMemberPath = "BrandName";
                cbBrand.SelectedIndex = -1;
            }
        }

        private void UpdatePaging()
        {
            if (_cars == null) return;
            TotalPages = (_cars.Count + PageSize - 1) / PageSize;
            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
            var skip = (CurrentPage - 1) * PageSize;
            _pagedCars = _cars.Where(c => c != null).Skip(skip).Take(PageSize).ToList();
            dgCars.ItemsSource = _pagedCars;
            icCars.ItemsSource = _pagedCars;
            UpdatePagingButtons();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                UpdatePaging();
            }
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdatePaging();
            }
        }

        private void UpdatePagingButtons()
        {
            if (btnPrevPage != null) btnPrevPage.IsEnabled = CurrentPage > 1;
            if (btnNextPage != null) btnNextPage.IsEnabled = CurrentPage < TotalPages;
            if (txtPageInfo != null) txtPageInfo.Text = $"Trang {CurrentPage}/{TotalPages}";
        }

        private void LoadCars(string search = "", int? brandId = null)
        {
            using (var db = new AutoRentalPrnContext())
            {
                var query = db.Cars
                    .Include(c => c.CarImages)
                    .Include(c => c.Brand)
                    .Where(c => c.Status == "Available");
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(c => c.CarModel.Contains(search) || c.LicensePlate.Contains(search));
                }
                if (brandId.HasValue)
                {
                    query = query.Where(c => c.BrandId == brandId.Value);
                }
                _cars = query.OrderBy(c => c.CarModel).ToList().Where(c => c != null).ToList();
                CurrentPage = 1;
                UpdatePaging();
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string search = txtSearch.Text.Trim();
            int? brandId = cbBrand.SelectedItem is CarBrand brand ? brand.BrandId : (int?)null;
            LoadCars(search, brandId);
        }

        private void BookCar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            var car = button?.DataContext as Car;
            if (car != null)
            {
                var bookingWindow = new CarBookingWindow(_currentUser, car);
                bookingWindow.ShowDialog();
                BtnSearch_Click(null, null); // reload sau khi book
            }
        }
    }
} 