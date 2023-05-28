using SalaryCounter.Views;

namespace SalaryCounter;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("EmployerPage", typeof(EmployerPage));
        Routing.RegisterRoute("EmployerTaskPage", typeof(EmployerTaskPage));
        Routing.RegisterRoute("ReportPage", typeof(ReportPage));
        Routing.RegisterRoute("CountSalaryTask", typeof(CountSalaryTask));
    }
}
