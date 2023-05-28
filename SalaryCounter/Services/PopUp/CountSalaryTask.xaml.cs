
namespace SalaryCounter.Views;

public partial class CountSalaryTask : ContentPage
{
    private readonly AppRepository _db;
    public CountSalaryTask(AppRepository db)
	{
        _db = db;
        InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        GetEmployers();
    }
    private void Clicked(object sender, EventArgs e)
    {
        GenerateCsvTask();
    }
    private void GenerateCsvTask()
    {
        decimal salary = 0;
        var task = new EmployerTask();
        task = _db.FindTask(Convert.ToInt32(ReportViewModel.HolderTaskId));
        var filePath = AppRepository.HolderPath;
        var time = task.EndTime.Subtract(task.StartTime).Days;
        if (time == 0 && task.ResultTime != DateTime.MinValue)
            salary = time * 8 * task.Rate;
        if (task.ResultTime > DateTime.MinValue)
        {            
            time = 1;
            salary = time * 8 * task.Rate;

        }
        else if (task.ResultTime == DateTime.MinValue)
        {
            AppShell.Current.DisplayAlert("Генерация", "Задача не является выполненной", "OK");
            Shell.Current.SendBackButtonPressed();
            return;
        }
        FileStream fileStream = File.Open(filePath, FileMode.Open);
        fileStream.SetLength(0);
        fileStream.Close();
        File.AppendAllText(filePath, "Имя,Фамилия,Зарплата\n", Encoding.UTF8);
        foreach (var item in collectionView.ItemsSource)
        {
            if (item is SalaryEmployer collectionItem)
            {
                 string str = $"{collectionItem.FirstName},{collectionItem.LastName},{salary * Convert.ToInt32(collectionItem.EntrySalary)}\n";
                 File.AppendAllText(filePath, str, Encoding.UTF8);
                 Console.WriteLine();

                string entryText = collectionItem.EntrySalary;
            }
        }

        string pythonScriptPath = @"D:\BackUp\SalaryCounter\ChartCreate\ChartCreate.py";

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "python";
        startInfo.Arguments = pythonScriptPath;

        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.CreateNoWindow = true;

        using (Process process = new Process())
        {
            process.StartInfo = startInfo;
            process.Start();

            // Получить вывод скрипта Python
            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            Console.WriteLine("Вывод скрипта Python:");
            Console.WriteLine(output);
        }
        AppShell.Current.DisplayAlert("Генерация", "Файл csv обновлен", "OK");
        Shell.Current.SendBackButtonPressed();
    }
    private void GetEmployers()
    {
        var collection = new List<SalaryEmployer>();
        foreach (var item in _db.FindEmployerSalary(ReportViewModel.HolderTaskId))
        {
            var temp = new SalaryEmployer();
            temp.FirstName = item.FirstName;
            temp.LastName = item.LastName;
            temp.EntrySalary = "";
            collection.Add(temp);
        }
        collectionView.ItemsSource = collection;
    }
}