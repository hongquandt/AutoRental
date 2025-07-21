using System.Configuration;
using System.Data;
using System.Windows;

namespace AutoRental
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Khởi động ứng dụng với LoginWindow
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
