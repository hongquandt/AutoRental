using BusinessObjects;
using DataAccessObjects.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AutoRentalPrnContext _context;
        public BookingRepository()
        {
            _context = new AutoRentalPrnContext();
        }

        public IEnumerable<Booking> GetAll() => _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Car)
            .OrderByDescending(b => b.CreatedDate)
            .ToList();

        public IEnumerable<Booking> GetByUser(int userId) => _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Car)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedDate)
            .ToList();

        public Booking? GetById(int bookingId) => _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Car)
            .FirstOrDefault(b => b.BookingId == bookingId);

        public bool Add(Booking booking)
        {
            try
            {
                _context.Bookings.Add(booking);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(Booking booking)
        {
            try
            {
                var existingBooking = _context.Bookings.Find(booking.BookingId);
                if (existingBooking != null)
                {
                    _context.Entry(existingBooking).CurrentValues.SetValues(booking);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int bookingId)
        {
            try
            {
                var booking = _context.Bookings.Find(bookingId);
                if (booking != null)
                {
                    _context.Bookings.Remove(booking);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
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
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Car)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(b =>
                    b.BookingCode.Contains(keyword) ||
                    b.User.FullName.Contains(keyword) ||
                    b.Car.CarModel.Contains(keyword) ||
                    b.Car.LicensePlate.Contains(keyword) ||
                    b.Status.Contains(keyword) ||
                    b.TotalAmount.ToString().Contains(keyword)
                );
            }
            return query.OrderByDescending(b => b.CreatedDate).ToList();
        }

        public string GenerateBookingCode()
        {
            var allBookings = _context.Bookings.ToList();
            int maxNumber = 0;

            foreach (var booking in allBookings)
            {
                if (booking.BookingCode != null && booking.BookingCode.StartsWith("BK-"))
                {
                    if (int.TryParse(booking.BookingCode.Substring(3), out int num))
                    {
                        if (num > maxNumber) maxNumber = num;
                    }
                }
            }

            return $"BK-{(maxNumber + 1).ToString("D3")}";
        }
    }
}