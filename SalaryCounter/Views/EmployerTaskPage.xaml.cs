using System.Text.RegularExpressions;
using SalaryCounter.ViewModels;

namespace SalaryCounter.Views;

public partial class EmployerTaskPage : ContentPage
{
    public EmployerTaskPage(EmployerTaskViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        (this.BindingContext as EmployerTaskViewModel).Initialize();
    }
}