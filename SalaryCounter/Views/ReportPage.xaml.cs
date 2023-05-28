using Microsoft.Maui.Controls;
using SalaryCounter.ViewModels;
using System.Diagnostics;

namespace SalaryCounter.Views;

public partial class ReportPage : ContentPage
{
    public ReportPage(ReportViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
      
    }
    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        (this.BindingContext as ReportViewModel).OnAppearing();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Rotate();
    }
    async Task Rotate()
    {
        uint duration = 10 * 60 * 1000;
        await Task.WhenAll
        (
          Image.RotateTo(307 * 360, duration),
          Image.RotateXTo(251 * 360, duration),
          Image.RotateYTo(199 * 360, duration)
        );
    }
}