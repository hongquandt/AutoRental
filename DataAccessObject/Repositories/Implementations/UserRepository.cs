using BusinessObjects;
using DataAccessObjects.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataAccessObjects.Repositories.Implementations
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

        public IEnumerable<User> GetAll()
        {
            return _context.Users.Include(u => u.Role).ToList();
        }

        public User? GetById(int userId)
        {
            return _context.Users.Include(u => u.Role).FirstOrDefault(u => u.UserId == userId);
        }

        public bool Add(User user)
        {
            try
            {
                // Kiểm tra email đã tồn tại chưa
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    throw new InvalidOperationException($"Email '{user.Email}' đã tồn tại trong hệ thống!");
                }

                // Kiểm tra username đã tồn tại chưa
                if (_context.Users.Any(u => u.Username == user.Username))
                {
                    throw new InvalidOperationException($"Username '{user.Username}' đã tồn tại trong hệ thống!");
                }

                _context.Users.Add(user);
                int result = _context.SaveChanges();
                
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    throw new InvalidOperationException("Không thể lưu user vào database!");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                System.Diagnostics.Debug.WriteLine($"Lỗi khi thêm user: {ex.Message}");
                throw; // Re-throw để ViewModel có thể hiển thị lỗi
            }
        }

        public bool Update(User user)
        {
            try
            {
                // Kiểm tra email đã tồn tại chưa (trừ user hiện tại)
                if (_context.Users.Any(u => u.Email == user.Email && u.UserId != user.UserId))
                {
                    throw new InvalidOperationException($"Email '{user.Email}' đã tồn tại trong hệ thống!");
                }

                _context.Users.Update(user);
                int result = _context.SaveChanges();
                
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    throw new InvalidOperationException("Không thể cập nhật user trong database!");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật user: {ex.Message}");
                throw; // Re-throw để ViewModel có thể hiển thị lỗi
            }
        }

        public bool Delete(int userId)
        {
            try
            {
                var user = _context.Users.Find(userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    int result = _context.SaveChanges();
                    
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        throw new InvalidOperationException("Không thể xóa user khỏi database!");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Không tìm thấy user với ID: {userId}");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                System.Diagnostics.Debug.WriteLine($"Lỗi khi xóa user: {ex.Message}");
                throw; // Re-throw để ViewModel có thể hiển thị lỗi
            }
        }
    }
}