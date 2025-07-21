using BusinessObjects;
using System.Collections.Generic;

namespace Service.Interfaces
{
    public interface IBookingService
    {
        IEnumerable<Booking> GetAll();
        IEnumerable<Booking> GetByUser(int userId);
        Booking? GetById(int bookingId);
        void Add(Booking booking);
        void Cancel(int bookingId);
        IEnumerable<Booking> Search(string keyword);
    }
} 