using BusinessObjects;

namespace Service.Interfaces
{
    public interface IUserService
    {
        User? Authenticate(string email, string password);
        IEnumerable<User> GetAll();
        User? GetById(int userId);
        bool Add(User user);
        bool Update(User user);
        bool Delete(int userId);
    }
} 