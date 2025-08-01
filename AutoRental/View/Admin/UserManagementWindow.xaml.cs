﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AutoRental.ViewModels.Admin;
using Service.Implementations;
using Service.Interfaces;
using DataAccessObjects.Repositories.Implementations;
using DataAccessObjects.Repositories.Interfaces;

namespace AutoRental.View.Admin
{
    /// <summary>
    /// Interaction logic for UserManagementWindow.xaml
    /// </summary>
    public partial class UserManagementWindow : Window
    {
        public UserManagementWindow()
        {
            InitializeComponent();
            
            // Setup Services (Dependency Injection)
            IUserRepository userRepo = new UserRepository();
            IUserService userService = new UserService(userRepo);
            
            // Create and set ViewModel
            var viewModel = new UserManagementViewModel(userService);
            this.DataContext = viewModel;
        }

        private void userDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // Ensure auto-generation is off, in case it's being overridden or re-enabled
            userDataGrid.AutoGenerateColumns = false;
            
            // Clear any auto-generated columns that might have been added
            var autoGeneratedColumns = userDataGrid.Columns.Where(c => !c.IsReadOnly).ToList();
            foreach (var column in autoGeneratedColumns)
            {
                if (column.Header.ToString().Contains("User") || 
                    column.Header.ToString().Contains("Password") ||
                    column.Header.ToString().Contains("Role") ||
                    column.Header.ToString().Contains("Created"))
                {
                    userDataGrid.Columns.Remove(column);
                }
            }
        }
    }
}
