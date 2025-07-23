using BusinessObjects;
using Service.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AutoRental.ViewModels
{
    public class BookingViewModel : INotifyPropertyChanged
    {
        private readonly IBookingService _service;
        private ObservableCollection<Booking> _bookings;
        private Booking? _selectedBooking;

        public ObservableCollection<Booking> Bookings 
        { 
            get => _bookings; 
            set
            {
                _bookings = value;
                OnPropertyChanged(nameof(Bookings));
            }
        }

        public Booking? SelectedBooking 
        { 
            get => _selectedBooking; 
            set
            {
                _selectedBooking = value;
                OnPropertyChanged(nameof(SelectedBooking));
            }
        }

        public BookingViewModel(IBookingService? service = null)
        {
            _service = service ?? new Service.Implementations.BookingService();
            _bookings = new ObservableCollection<Booking>(_service.GetAll());
        }

        public void LoadAll()
        {
            Bookings = new ObservableCollection<Booking>(_service.GetAll());
        }

        public void LoadByUser(int userId)
        {
            var bookings = _service.GetByUser(userId);
            foreach (var booking in bookings)
            {
                if (booking.Car == null)
                {
                    // Nếu Car null, load lại từ service
                    booking.Car = _service.GetById(booking.BookingId)?.Car;
                }
            }
            Bookings = new ObservableCollection<Booking>(bookings);
        }

        public void Search(string keyword)
        {
            Bookings = new ObservableCollection<Booking>(_service.Search(keyword));
        }

        public bool CancelSelected()
        {
            try
            {
                if (SelectedBooking != null && SelectedBooking.Status == "Confirmed")
                {
                    _service.Cancel(SelectedBooking.BookingId);
                    // Cập nhật trạng thái của booking đã chọn
                    SelectedBooking.Status = "Cancelled";
                    // Thông báo UI cập nhật
                    OnPropertyChanged(nameof(SelectedBooking));
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
} 