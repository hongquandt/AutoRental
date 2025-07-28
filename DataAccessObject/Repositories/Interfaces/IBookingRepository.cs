using BusinessObjects;
using System.Collections.Generic;

namespace DataAccessObjects.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        IEnumerable<Booking> GetAll();
        IEnumerable<Booking> GetByUser(int userId);
        Booking? GetById(int bookingId);
        bool Add(Booking booking);
        bool Update(Booking booking);
        bool Delete(int bookingId);
        void Cancel(int bookingId);
        IEnumerable<Booking> Search(string keyword);
        string GenerateBookingCode();
    }
}