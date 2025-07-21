using BusinessObjects;

namespace Service.Interfaces
{
    public interface IUserService
    {
        User? Authenticate(string email, string password);
    }
} 