using BusinessObjects;

namespace DataAccessObjects.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User? GetUserByEmailAndPassword(string email, string password);
        User? GetUserByEmail(string email);
    }
} 