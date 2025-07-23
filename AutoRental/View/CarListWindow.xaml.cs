using System.Windows;
using BusinessObjects;
using System.Linq;
using System.Collections.Generic;
using DataAccessObjects.Context;

namespace AutoRental
{
    public partial class CarListWindow : Window
    {
        private readonly User _currentUser;
        private List<Car> _cars;
        private List<CarBrand> _brands;
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

        private void LoadCars(string search = "", int? brandId = null)
        {
            using (var db = new AutoRentalPrnContext())
            {
                var query = db.Cars.Where(c => c.Status == "Available");
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(c => c.CarModel.Contains(search) || c.LicensePlate.Contains(search));
                }
                if (brandId.HasValue)
                {
                    query = query.Where(c => c.BrandId == brandId.Value);
                }
                _cars = query.OrderBy(c => c.CarModel).ToList();
                dgCars.ItemsSource = _cars;
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
            var car = dgCars.SelectedItem as Car;
            if (car != null)
            {
                var bookingWindow = new CarBookingWindow(_currentUser, car);
                bookingWindow.ShowDialog();
                BtnSearch_Click(null, null); // reload sau khi book
            }
        }
    }
} 