using System;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.IO;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DataAccessObjects.Context;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Đang kiểm tra kết nối database...");
        bool ok = TestConnection();
        Console.WriteLine(ok ? "Kết nối thành công!" : "Kết nối thất bại!");
        
        if (ok)
        {
            Console.Write("Nhập email người dùng để lấy thông tin: ");
            string email = Console.ReadLine();
            var user = GetUserProfile(email);
            
            if (user != null)
            {
                Console.WriteLine("\n--- THÔNG TIN NGƯỜI DÙNG ---");
                Console.WriteLine($"ID: {user.UserId}");
                Console.WriteLine($"Tên đăng nhập: {user.Username}");
                Console.WriteLine($"Họ tên: {user.FullName}");
                Console.WriteLine($"Email: {user.Email}");
                Console.WriteLine($"Số điện thoại: {user.PhoneNumber ?? "Không có"}");
                Console.WriteLine($"Vai trò: {user.Role.RoleName}");
                Console.WriteLine($"Ngày tạo: {user.CreatedDate:dd/MM/yyyy HH:mm:ss}");
            }
            else
            {
                Console.WriteLine("Không tìm thấy người dùng với email này!");
            }
        }
        
        Console.WriteLine("\nNhấn phím bất kỳ để thoát...");
        Console.ReadKey();
    }

    static bool TestConnection()
    {
        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var connectionString = config.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi: {ex.Message}");
            return false;
        }
    }
    
    static User GetUserProfile(string email)
    {
        try
        {
            var context = new AutoRentalPrnContext();
            return context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi lấy thông tin người dùng: {ex.Message}");
            return null;
        }
    }
}
