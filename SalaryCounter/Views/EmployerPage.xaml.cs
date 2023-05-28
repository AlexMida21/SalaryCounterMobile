using Microsoft.Maui;
using SalaryCounter.ViewModels;

namespace SalaryCounter.Views;

public partial class EmployerPage : ContentPage
{
    public EmployerPage(EmployerViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        (this.BindingContext as EmployerViewModel).Initialize();
    }
}
