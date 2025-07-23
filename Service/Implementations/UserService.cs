using BusinessObjects;
using Service.Interfaces;

using DataAccessObjects.Repositories.Interfaces;
using DataAccessObjects.Repositories.Implementations;

namespace Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository? userRepository = null)
        {
            _userRepository = userRepository ?? new UserRepository();
        }

        public User? Authenticate(string email, string password)
        {
            return _userRepository.GetUserByEmailAndPassword(email, password);
        }
    }
} 