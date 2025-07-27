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

        public IEnumerable<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        public User? GetById(int userId)
        {
            return _userRepository.GetById(userId);
        }

        public bool Add(User user)
        {
            try
            {
                return _userRepository.Add(user);
            }
            catch (Exception ex)
            {
                // Log lỗi và re-throw để ViewModel có thể hiển thị
                System.Diagnostics.Debug.WriteLine($"UserService.Add error: {ex.Message}");
                throw;
            }
        }

        public bool Update(User user)
        {
            try
            {
                return _userRepository.Update(user);
            }
            catch (Exception ex)
            {
                // Log lỗi và re-throw để ViewModel có thể hiển thị
                System.Diagnostics.Debug.WriteLine($"UserService.Update error: {ex.Message}");
                throw;
            }
        }

        public bool Delete(int userId)
        {
            try
            {
                return _userRepository.Delete(userId);
            }
            catch (Exception ex)
            {
                // Log lỗi và re-throw để ViewModel có thể hiển thị
                System.Diagnostics.Debug.WriteLine($"UserService.Delete error: {ex.Message}");
                throw;
            }
        }
    }
} 