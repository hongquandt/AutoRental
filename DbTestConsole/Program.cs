using System;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Đang kiểm tra kết nối database...");
        bool ok = TestConnection();
        Console.WriteLine(ok ? "Kết nối thành công!" : "Kết nối thất bại!");
        Console.WriteLine("Nhấn phím bất kỳ để thoát...");
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
}
