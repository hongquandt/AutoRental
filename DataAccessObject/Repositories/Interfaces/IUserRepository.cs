using BusinessObjects;

namespace DataAccessObjects.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User? GetUserByEmailAndPassword(string email, string password);
        User? GetUserByEmail(string email);
        IEnumerable<User> GetAll();
        User? GetById(int userId);
        bool Add(User user);
        bool Update(User user);
        bool Delete(int userId);
    }
} 