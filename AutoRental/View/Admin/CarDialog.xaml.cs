using System.Windows;
using AutoRental.ViewModels.Admin;

namespace AutoRental.View.Admin
{
    public partial class CarDialog : Window
    {
        public CarDialog(CarDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            this.Owner = Application.Current.MainWindow;
            viewModel.SetDialogWindow(this);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CarDialogViewModel viewModel)
            {
                viewModel.SaveCommand.Execute(null);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
} 