using Microsoft.Extensions.Logging;
using SalaryCounter.ViewModels;

namespace SalaryCounter;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddSingleton(new AppRepository("Appdb19.db"));
        builder.Services.AddSingleton<EmployerPage>();
        builder.Services.AddSingleton<EmployerTaskPage>();
        builder.Services.AddSingleton<ReportPage>();
        builder.Services.AddSingleton<CountSalaryTask>();
        builder.Services.AddSingleton<EmployerViewModel>();
        builder.Services.AddSingleton<EmployerTaskViewModel>();
        builder.Services.AddSingleton<ReportViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
