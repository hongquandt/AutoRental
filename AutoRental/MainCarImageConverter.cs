using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using BusinessObjects;

namespace AutoRental
{
    public class MainCarImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var images = value as System.Collections.Generic.ICollection<CarImage>;
            if (images != null && images.Count > 0)
            {
                var mainImg = images.FirstOrDefault(i => i.IsMain) ?? images.FirstOrDefault();
                if (mainImg != null && !string.IsNullOrEmpty(mainImg.ImageUrl))
                {
                    // Nếu ImageUrl đã chứa 'images/', chỉ lấy tên file
                    var fileName = mainImg.ImageUrl.Contains("/") ? System.IO.Path.GetFileName(mainImg.ImageUrl) : mainImg.ImageUrl;
                    return $"pack://siteoforigin:,,,/images/{fileName}";
                }
            }
            // Đường dẫn ảnh mặc định nếu không có ảnh
            return "pack://siteoforigin:,,,/images/default-avatar.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 