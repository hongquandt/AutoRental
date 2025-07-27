using System.Windows;
using AutoRental.ViewModels.Admin;
using Service.Implementations;
using Service.Interfaces;
using DataAccessObjects.Repositories.Implementations;
using DataAccessObjects.Repositories.Interfaces;
using BusinessObjects;

namespace AutoRental
{
    public partial class AdminWindow : Window
    {
        public AdminWindow(User admin)
        {
            InitializeComponent();
            
            // Setup Services (Dependency Injection)
            IUserRepository userRepo = new UserRepository();
            IUserService userService = new UserService(userRepo);
            IBookingService bookingService = new BookingService();
            
            // Create and set ViewModel - truyền reference của window này
            var viewModel = new AdminViewModel(admin, userService, bookingService, this);
            this.DataContext = viewModel;
        }
    }
} 