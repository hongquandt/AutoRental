using BusinessObjects;
using Service.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AutoRental.ViewModels
{
    public class BookingViewModel : INotifyPropertyChanged
    {
        private readonly IBookingService _service;
        public ObservableCollection<Booking> Bookings { get; set; }
        public Booking? SelectedBooking { get; set; }

        public BookingViewModel(IBookingService? service = null)
        {
            _service = service ?? new Service.Implementations.BookingService();
            Bookings = new ObservableCollection<Booking>(_service.GetAll());
        }

        public void LoadAll() => Bookings = new ObservableCollection<Booking>(_service.GetAll());
        public void LoadByUser(int userId) => Bookings = new ObservableCollection<Booking>(_service.GetByUser(userId));
        public void Search(string keyword) => Bookings = new ObservableCollection<Booking>(_service.Search(keyword));
        public void CancelSelected()
        {
            if (SelectedBooking != null)
            {
                _service.Cancel(SelectedBooking.BookingId);
                LoadAll();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
} 