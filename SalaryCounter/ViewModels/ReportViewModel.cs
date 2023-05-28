using MailKit.Security;
using MimeKit;
using SalaryCounter.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

//CollectionView on ReportPage
//Создание файла в директорию
//HolderPath
//отправка на почту

namespace SalaryCounter.ViewModels;

public partial class ReportViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private readonly AppRepository _db;
    public static int HolderTaskId;
    public ReportViewModel(AppRepository db)
    {
        _db = db;
        Commands = new List<string>(){
        "Подсчёт эффективности работника",
        "Подсчёт всех работников",
        "Генерация зарплаты по задаче",
        "Генерация предположительных трудозатрат",
        "Генерация предположительных трудозатрат по задаче",
        "Оценка сотрудника"
        };
        ClickedCommand = new Command(async () =>
        {
            await Clicked();
        });
    }

    private void Clear()
    {
        chooseEmployer = null;
        chooseTask = null;
    }
    public  void OnAppearing()
    {        
        var pickerEmp = _db.GetEmployerss();
        List<string> Emp = new();
        List<string> Tas = new();
        foreach (var item in pickerEmp)
        {
            Emp.Add(item.LastName);
            //pickerEmployer.Items.Add(item.LastName);
        }
        var pickerTas = _db.GetEmployerTasks();
        foreach (var item in pickerTas)
        {
            Tas.Add(item.Id.ToString());
            //pickerTask.Items.Add(item.Id.ToString());
        }
        PickerTask = Tas;
        PickerEmployer = Emp;
    }
    async Task Clicked()
    {
        var sm = new SendMail();
        if (chooseCommand.Equals("Подсчёт эффективности работника"))
        {
            if (chooseEmployer != null)
            {
                int idEmployer = _db.FindEmployerId(chooseEmployer.ToString());
                var Efficiency = _db.FindEmployerEfficiency(idEmployer);
                var Employer = _db.FindEmployer(idEmployer);
                var filePath = AppRepository.HolderPath;
                FileStream fileStream = File.Open(filePath, FileMode.Open);
                fileStream.SetLength(0);
                fileStream.Close();
                File.AppendAllText(filePath, "Имя,Фамилия,Эффективность\n", Encoding.UTF8);
                string str = $"{Employer.FirstName},{Employer.LastName},{Efficiency}\n";
                File.AppendAllText(filePath, str, Encoding.UTF8);
                await AppShell.Current.DisplayAlert("Генерация", "Файл csv отправлен", "OK");
                OnAppearing();
                sm.SendFileMail(chooseCommand);
            }
            else
            {
                await AppShell.Current.DisplayAlert("Генерация", "Выберите сотрудника", "OK");
            }
            Clear();
        }
        if (chooseCommand.Equals("Генерация зарплаты по задаче"))
        {
            if (chooseTask != null)
            {
                HolderTaskId = Convert.ToInt32(chooseTask);
                await Shell.Current.GoToAsync($"CountSalaryTask");
            }
            else
            {
                await AppShell.Current.DisplayAlert("Генерация", "Выберите задачу", "OK");
            }
            Clear();
        }
        if (chooseCommand.Equals("Подсчёт всех работников"))
        {
            var filePath = AppRepository.HolderPath;
            FileStream fileStream = File.Open(filePath, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close();
            File.AppendAllText(filePath, "Имя,Фамилия,Эффективность\n", Encoding.UTF8);

            var Employers = _db.GetEmployerss();
            foreach (var employer in Employers)
            {
                var Efficiency = _db.FindEmployerEfficiency(employer.Id);
                var Employer = _db.FindEmployer(employer.Id);

                string str = $"{Employer.FirstName},{Employer.LastName},{Efficiency}\n";
                File.AppendAllText(filePath, str, Encoding.UTF8);
            }
            
            await AppShell.Current.DisplayAlert("Генерация", "Файл csv отправлен", "OK");
            OnAppearing();
            sm.SendFileMail(chooseCommand);

            Clear();
        }
        if (chooseCommand.Equals("Генерация предположительных трудозатрат"))
        {
            var Result = _db.FindEmployerLaborCost();

            var filePath = AppRepository.HolderPath;
            FileStream fileStream = File.Open(filePath, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close();
            File.AppendAllText(filePath, "id Задачи,Название,ПредположительныеТрудозатраты\n", Encoding.UTF8);

            foreach (var result in Result)
            {
                File.AppendAllText(filePath, result, Encoding.UTF8);
            }
           
            await AppShell.Current.DisplayAlert("Генерация", "Файл csv отправлен", "OK");
            OnAppearing();
            sm.SendFileMail(chooseCommand);
            Clear();
        }
        if (chooseCommand.Equals("Генерация предположительных трудозатрат по задаче"))
        {
            if (chooseTask == null)
            {
                await AppShell.Current.DisplayAlert("Генерация", "Выберите задачу", "OK");
                return;
            }
            var Result = _db.FindEmployerLaborCostTask(Convert.ToInt32(chooseTask));

            var filePath = AppRepository.HolderPath;
            FileStream fileStream = File.Open(filePath, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close();
            File.AppendAllText(filePath, "id Задачи,Название,ПредположительныеТрудозатраты\n", Encoding.UTF8);

            foreach (var result in Result)
            {
                File.AppendAllText(filePath, result, Encoding.UTF8);
            }
            
            await AppShell.Current.DisplayAlert("Генерация", "Файл csv отправлен", "OK");
            OnAppearing();
            sm.SendFileMail(chooseCommand);
            Clear();
        }
        if (chooseCommand.Equals("Оценка сотрудника"))
        {
            if (chooseEmployer == null)
            {
                await AppShell.Current.DisplayAlert("Генерация", "Выберите сотрудника", "OK");
                return;
            }
            int idEmployer = _db.FindEmployerId(chooseEmployer.ToString());
            var Efficiency = _db.FindTerms(idEmployer);
            var Employer = _db.FindEmployer(idEmployer);
            var filePath = AppRepository.HolderPath;
            FileStream fileStream = File.Open(filePath, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close();
            File.AppendAllText(filePath, "Имя;Фамилия;Вовремя;Просрочено\n", Encoding.UTF8);
            string str = $"{Employer.FirstName};{Employer.LastName};{Efficiency[1]};{Efficiency[2]}\n";
            File.AppendAllText(filePath, str, Encoding.UTF8);
            await AppShell.Current.DisplayAlert("Генерация", "Файл csv отправлен", "OK");
            sm.SendFileMail(chooseCommand);
            Clear();
        }
    }

    private List<string> pickerEmployer;
    private List<string> pickerCommand;
    private List<string> pickerTask;
    private List<string> commands;
    private string chooseEmployer;
    private string chooseCommand;
    private string chooseTask;
    public string ChooseTask
    {
        get { return chooseTask; }
        set { chooseTask = value; OnPropertyChanged(); }
    }
    public string ChooseCommand
    {
        get { return chooseCommand; }
        set { chooseCommand = value; OnPropertyChanged(); }
    }
    public string ChooseEmployer
    {
        get { return chooseEmployer; }
        set { chooseEmployer = value; OnPropertyChanged(); }
    }
    public List<string> PickerEmployer
    {
        get { return pickerEmployer; }
        set { pickerEmployer = value; OnPropertyChanged(); }
    }
    public List<string> PickerCommand
    {
        get { return pickerCommand; }
        set { pickerCommand = value; OnPropertyChanged(); }
    }
    public List<string> PickerTask
    {
        get { return pickerTask; }
        set { pickerTask = value; OnPropertyChanged(); }
    }
    public List<string> Commands
    {
        get { return commands; }
        set { commands = value; OnPropertyChanged(); }
    }
    public ICommand GetEmployerCommand { get; set; }
    public ICommand ClickedCommand { get; set; }
    public ICommand SearchCommand { get; set; }
}

