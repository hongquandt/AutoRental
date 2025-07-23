using BusinessObjects;
using DataAccessObjects.Repositories.Implementations;
using DataAccessObjects.Repositories.Interfaces;
using Service.Interfaces;

using System.Collections.Generic;


namespace Service.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        public BookingService(IBookingRepository? repo = null)
        {
            _repo = repo ?? new BookingRepository();
        }

        public IEnumerable<Booking> GetAll() => _repo.GetAll();
        public IEnumerable<Booking> GetByUser(int userId) => _repo.GetByUser(userId);
        public Booking? GetById(int bookingId) => _repo.GetById(bookingId);
        public void Add(Booking booking) => _repo.Add(booking);
        public void Cancel(int bookingId) => _repo.Cancel(bookingId);
        public IEnumerable<Booking> Search(string keyword) => _repo.Search(keyword);
    }
} 