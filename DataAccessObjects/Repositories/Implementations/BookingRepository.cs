using BusinessObjects;
using DataAccessObjects.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using DataAccessObjects.Context;

namespace DataAccessObjects.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AutoRentalPrnContext _context;
        public BookingRepository()
        {
            _context = new AutoRentalPrnContext();
        }

        public IEnumerable<Booking> GetAll() => _context.Bookings.OrderByDescending(b => b.CreatedDate).ToList();

        public IEnumerable<Booking> GetByUser(int userId) => _context.Bookings.Where(b => b.UserId == userId).OrderByDescending(b => b.CreatedDate).ToList();

        public Booking? GetById(int bookingId) => _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

        public void Add(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
        }

        public void Cancel(int bookingId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking != null && booking.Status == "Confirmed")
            {
                booking.Status = "Cancelled";
                var car = _context.Cars.FirstOrDefault(c => c.CarId == booking.CarId);
                if (car != null) car.Status = "Available";
                _context.SaveChanges();
            }
        }

        public IEnumerable<Booking> Search(string keyword)
        {
            var query = _context.Bookings.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(b => b.BookingCode.Contains(keyword) || b.Car.CarModel.Contains(keyword) || b.Car.LicensePlate.Contains(keyword));
            }
            return query.OrderByDescending(b => b.CreatedDate).ToList();
        }
    }
} 