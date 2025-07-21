using BusinessObjects;
using Repositories.Interfaces;
using System.Linq;

namespace Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AutoRentalPrnContext _context;

        public UserRepository(AutoRentalPrnContext? context = null)
        {
            _context = context ?? new AutoRentalPrnContext();
        }

        public User? GetUserByEmailAndPassword(string email, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public User? GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
    }
} 