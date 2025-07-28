using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using BusinessObjects;
using System.Linq; // Added missing import for FirstOrDefault

namespace AutoRental
{
    public class CarImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Car car)
            {
                // Tìm ảnh chính của xe
                var mainImage = car.CarImages?.FirstOrDefault(img => img.IsMain);
                if (mainImage != null && !string.IsNullOrEmpty(mainImage.ImageUrl))
                {
                    try
                    {
                        // Xử lý đường dẫn ảnh
                        string imagePath = mainImage.ImageUrl;
                        
                        // Nếu đường dẫn bắt đầu bằng /images/, chuyển thành đường dẫn tuyệt đối
                        if (imagePath.StartsWith("/images/"))
                        {
                            // Lấy đường dẫn thư mục hiện tại của ứng dụng
                            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                            string imagesDirectory = Path.Combine(appDirectory, "images");
                            imagePath = Path.Combine(imagesDirectory, imagePath.Substring(8)); // Bỏ "/images/"
                            
                            System.Diagnostics.Debug.WriteLine($"App Directory: {appDirectory}");
                            System.Diagnostics.Debug.WriteLine($"Images Directory: {imagesDirectory}");
                            System.Diagnostics.Debug.WriteLine($"Final Image Path: {imagePath}");
                        }
                        
                        // Kiểm tra file có tồn tại không
                        if (File.Exists(imagePath))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.UriSource = new Uri(imagePath);
                            bitmap.EndInit();
                            return bitmap;
                        }
                        else
                        {
                            // Nếu file không tồn tại, trả về null để hiển thị placeholder
                            System.Diagnostics.Debug.WriteLine($"Image file not found: {imagePath}");
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                        return null;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"No main image found for car: {car?.CarModel}");
                }
            }
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
} 