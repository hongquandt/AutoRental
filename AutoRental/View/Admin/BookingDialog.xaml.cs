using System.Windows;
using AutoRental.ViewModels.Admin;

namespace AutoRental.View.Admin
{
    public partial class BookingDialog : Window
    {
        public BookingDialog(BookingDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            this.Owner = Application.Current.MainWindow;
            viewModel.SetDialogWindow(this);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BookingDialogViewModel viewModel)
            {
                viewModel.SaveCommand.Execute(null);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnRecalculate_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BookingDialogViewModel viewModel)
            {
                viewModel.RecalculateTotalAmount();
            }
        }
    }
} 