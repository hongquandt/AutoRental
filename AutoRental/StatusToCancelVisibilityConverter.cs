using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BusinessObjects;

namespace AutoRental
{
    public class StatusToCancelVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Booking booking)
            {
                return booking.Status == "Confirmed";
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 