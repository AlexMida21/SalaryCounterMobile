using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;

namespace SalaryCounter.ViewModels;

public partial class EmployerTaskViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private readonly AppRepository _db;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public EmployerTaskViewModel(AppRepository db)
    {
        _db = db;
        EmployersTask = new List<TaskVisible>();
        Commands = new List<string>() {
        "Добавить",
        "Удалить",
        "Изменить",
        "Отметить выполненным"
        };
        ClickedCommand = new Command(async () =>
        {
            await Clicked();
        });
        SearchCommand = new Command(async () =>
        {
            Search();
        });
    }
    public void Initialize()
    {
        Task.Run(GetTasks);
    }

    async Task Clicked()
    {
        if (ChooseCommand == null)
            return;
        if (ChooseCommand.Equals("Добавить"))
        {
            if (AddName == null|| AddRate == null|| AddEmployer == null) //.equals("")
            {
                await AppShell.Current.DisplayAlert("Добавление", "Не все ячейки заполнены", "OK");
                return;
            }
            decimal i = 0;
            string s = AddRate;
            bool result = decimal.TryParse(s, out i);
            if (!result)
            {
                await AppShell.Current.DisplayAlert("Добавление", "Тариф не decimal", "OK");
                return;
            }
            if (AddName.Equals("") || AddRate.Equals("") || AddEmployer.Equals(""))
            {
                await AppShell.Current.DisplayAlert("Добавление", "Не все ячейки заполненны", "OK");
                return;
            }
            int idTask = _db.FindLastEmployerTask() + 1;
            string[] split = { "," };
            string[] employerStr = AddEmployer.Split(split, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in employerStr)
            {
                int i2 = 0;
                string s2 = item;
                bool result2 = int.TryParse(s2, out i2);
                if (!result2)
                {
                    await AppShell.Current.DisplayAlert("Добавление", "Неправильно заполнено поле сотрудников", "OK");
                    return;
                }
            }
            foreach (string str in employerStr)
            {
                var temp = new EmployersList()
                {
                    IdEmployer = Convert.ToInt32(str),
                    IdTask = idTask
                };
                _db.CreateEmployersList(temp);
            }
            var task = new EmployerTask()
            {
                Name = AddName,
                Rate = Convert.ToDecimal(AddRate),
                EmployersId = _db.FindLastEmployerTask() + 1,
                StartTime = StartTime,
                EndTime = EndTime,
            };
            _db.CreateEmployerTask(task);
            GetTasks();
        }
        if (ChooseCommand.Equals("Изменить"))
        {
            {
                if (TaskSelected is null)
                    return;

                var template = TaskSelected as TaskVisible;
                if (template is null)
                    return;
                string[] split = { "," };
                string[] employerStr = AddEmployer.Split(split, StringSplitOptions.RemoveEmptyEntries);
                _db.DeleteEmployersList(template.Id);
                foreach (string str in employerStr)
                {
                    var temp = new EmployersList()
                    {
                        IdEmployer = Convert.ToInt32(str),
                        IdTask = template.Id
                    };
                    _db.CreateEmployersList(temp);
                }
                var task = new EmployerTask()
                {
                    Id = template.Id,
                    Name = AddName,
                    Rate = Convert.ToDecimal(AddRate),
                    EmployersId = template.Id,
                    EndTime = EndTime,
                    StartTime = StartTime,
                    ResultTime = template.ResultTime
                };
                _db.UpdateEmployerTask(task);
            }
            GetTasks();
        }
        if (ChooseCommand.Equals("Удалить"))
        {
            if (TaskSelected is null)
                return;
            var template = TaskSelected as TaskVisible;
            if (template is null)
                return;
            _db.DeleteEmployerTask(template.Id);
            GetTasks();
        }
        if (ChooseCommand.Equals("Отметить выполненным"))
        {
            if (TaskSelected is null)
                return;

            var template = TaskSelected as TaskVisible;
            DateTime temp = DateTime.Now;
            if (template is null)
                return;
            _db.UpdateEmployerTask(template.Id, temp);
            GetTasks();
        }
    }
    private void Search()
    {
        EmployersTask = _db.GetTasks();
        var source = new List<TaskVisible>();
        foreach (var item in EmployersTask)
        {
            if (item is TaskVisible collectionItem)
            {
                if (collectionItem.Name.Contains(SearchEntry))
                    source.Add(collectionItem);
            }
        }
        EmployersTask = source;
    }
    private void GetTasks()
    {
        EmployersTask = Enumerable.Reverse(_db.GetTasks()).ToList();
        foreach (var item in EmployersTask)
        {
            
            if (item is TaskVisible collectionItem)
            {
                collectionItem.StartTime2 = collectionItem.StartTime.ToString("dd.MM.yyyy");
                collectionItem.EndTime2 = collectionItem.EndTime.ToString("dd.MM.yyyy");
                collectionItem.ResultTime2 = collectionItem.ResultTime.ToString("dd.MM.yyyy");
                if (collectionItem.ResultTime == DateTime.MinValue)
                    collectionItem.DR = false;
            }
        }
    }
    private int id;
    private List<string> commands;
    private string name;
    private string rate;
    private string employers;
    private DateTime resultTime;
    private DateTime startTime;
    private DateTime endTime;
    private bool DR = true;
    private List<TaskVisible> employersTask;
    private string chooseCommand;

    private string addEmployer;
    private string addName;
    private string addRate;
    private string searchEntry;

    private TaskVisible taskSelected;
    public int Id
    {
        get { return id; }
        set { id = value; OnPropertyChanged(); }
    }
    public string Name
    {
        get { return name; }
        set { name = value; OnPropertyChanged(); }
    }
    public string Rate
    {
        get { return rate; }
        set { rate = value; OnPropertyChanged(); }
    }
    public string Employers
    {
        get { return employers; }
        set { employers = value; OnPropertyChanged(); }
    }
    public DateTime ResultTime
    {
        get { return resultTime; }
        set { resultTime = value; OnPropertyChanged(); }
    }
    public DateTime StartTime
    {
        get { return startTime; }
        set { startTime = value; OnPropertyChanged(); }
    }
    public DateTime EndTime
    {
        get { return endTime; }
        set { endTime = value; OnPropertyChanged(); }
    }
    public bool dr
    {
        get { return DR; }
        set { DR = value; OnPropertyChanged(); }
    }
    public string AddEmployer
    {
        get { return addEmployer; }
        set { addEmployer = value; OnPropertyChanged(); }
    }
    public string AddName
    {
        get { return addName; }
        set { addName = value; OnPropertyChanged(); }
    }
    public string AddRate
    {
        get { return addRate; }
        set { addRate = value; OnPropertyChanged(); }
    }
    public string SearchEntry
    {
        get { return searchEntry; }
        set { searchEntry = value; OnPropertyChanged(); }
    }
    public List<TaskVisible> EmployersTask
    {
        get { return employersTask; }
        set { employersTask = value; OnPropertyChanged(); }
    }
    public string ChooseCommand
    {
        get { return chooseCommand; }
        set { chooseCommand = value; OnPropertyChanged(); }
    }
    public List<string> Commands
    {
        get { return commands; }
        set { commands = value; OnPropertyChanged(); }
    }
    public TaskVisible TaskSelected
    {
        get { return taskSelected; }
        set { taskSelected = value; OnPropertyChanged(); }
    }
    public ICommand GetEmployerCommand { get; set; }
    public ICommand ClickedCommand { get; set; }
    public ICommand SearchCommand { get; set; }

}
