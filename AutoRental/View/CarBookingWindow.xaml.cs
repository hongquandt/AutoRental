using System.Windows;
using BusinessObjects;
using System;
using System.Linq;
using DataAccessObjects.Context;

namespace AutoRental
{
    public partial class CarBookingWindow : Window
    {
        private readonly User _user;
        private readonly Car _car;
        public CarBookingWindow(User user, Car car)
        {
            InitializeComponent();
            _user = user;
            _car = car;
            ShowCarInfo();
        }

        private void ShowCarInfo()
        {
            txtCarModel.Text = _car.CarModel;
            txtLicensePlate.Text = _car.LicensePlate;
            txtPrice.Text = _car.PricePerDay.ToString("N0") + " VNĐ";
            dpPickup.SelectedDate = DateTime.Today;
            dpReturn.SelectedDate = DateTime.Today.AddDays(1);
        }

        private void btnBook_Click(object sender, RoutedEventArgs e)
        {
            txtError.Visibility = Visibility.Collapsed;
            if (dpPickup.SelectedDate == null || dpReturn.SelectedDate == null)
            {
                ShowError("Vui lòng chọn ngày nhận và trả xe!");
                return;
            }
            var pickup = dpPickup.SelectedDate.Value;
            var ret = dpReturn.SelectedDate.Value;
            if (ret <= pickup)
            {
                ShowError("Ngày trả xe phải sau ngày nhận xe!");
                return;
            }
            var days = (ret - pickup).Days;
            var total = days * _car.PricePerDay;
            using (var db = new AutoRentalPrnContext())
            {
                // Kiểm tra xe còn available không
                var carDb = db.Cars.FirstOrDefault(c => c.CarId == _car.CarId && c.Status == "Available");
                if (carDb == null)
                {
                    ShowError("Xe này đã được đặt hoặc không còn khả dụng!");
                    return;
                }
                // Tạo booking
                var booking = new Booking
                {
                    UserId = _user.UserId,
                    CarId = _car.CarId,
                    PickupDateTime = pickup,
                    ReturnDateTime = ret,
                    TotalAmount = total,
                    Status = "Confirmed",
                    CreatedDate = DateTime.Now,
                    BookingCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                };
                db.Bookings.Add(booking);
                // Cập nhật trạng thái xe
                carDb.Status = "Rented";
                db.SaveChanges();
            }
            MessageBox.Show("Đặt xe thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void ShowError(string msg)
        {
            txtError.Text = msg;
            txtError.Visibility = Visibility.Visible;
        }
    }
} 