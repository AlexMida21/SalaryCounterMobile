using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;

namespace SalaryCounter.ViewModels;

public partial class EmployerViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private readonly AppRepository _db;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public EmployerViewModel(AppRepository db)
    {
        _db = db;
        Employers = new List<Employer>();
        Commands = new List<string>() { 
        "Добавить",
        "Удалить",
        "Изменить"  
        };
        GetEmployerCommand = new Command(async () =>
        {
            await GetEmployers();
        });
        ClickedCommand = new Command(async () =>
        {
            await Clicked();
        });
        SearchCommand = new Command(async () =>
        {
            await Search();
        });
    }
    public void Initialize()
    {
        Task.Run(GetEmployers);
    }
    async Task GetEmployers()
    {
        Employers = await _db.GetEmployers();
    }
    async Task Search()
    {
        Employers = await  _db.GetEmployers();
        var source = new List<Employer>();
        foreach (var item in Employers)
        {
            if (item is Employer collectionItem)
            {
                if (collectionItem.LastName.Contains(SearchValue))
                    source.Add(collectionItem);
            }
        }
        Employers = source;
    }
    async Task Clicked()
    {
        if (ChooseCommand == null)
            return;
        if (ChooseCommand.Equals("Добавить"))
        {
            if (!FirstName.Equals("") && !LastName.Equals(""))
            {
                var employer = new Employer()
                {
                    FirstName = FirstName,
                    LastName = LastName
                };
                _db.CreateEmployer(employer);
            }
            else await AppShell.Current.DisplayAlert("Ошибка", "Не все поля заполнены", "OK");
           await  GetEmployers();
        }
        if (ChooseCommand.Equals("Изменить"))
        {
            if (EmployerSelected is null)
                return;

            var employerr = EmployerSelected as Employer;
            if (employerr is null)
                return;
            if (!FirstName.Equals("") && !LastName.Equals(""))
            {
                employerr.FirstName = FirstName;
                employerr.LastName = LastName;
                _db.UpdateEmployer(employerr);
            }
            else await AppShell.Current.DisplayAlert("Ошибка", "Не все поля заполнены", "OK");
            await GetEmployers();
        }
        if (ChooseCommand.Equals("Удалить"))
        {
            if (EmployerSelected is null)
                return;

            var employerr = EmployerSelected as Employer;
            if (employerr is null)
                return;
            if (_db.FindEmployers(employerr.Id).Count() < 1)
                _db.DeleteEmployer(employerr);
            else await AppShell.Current.DisplayAlert("Ошибка", "Сначала удалите задачу в которой участвует работник", "OK");
            await GetEmployers();
        }
    }
    private List<Employer> employers;
    private string firstname;
    private string lastname;
    private string chooseCommand;
    private string searchValue;
    private List<string> commands;
    private string pickerValue;
    private int id;
    private Employer employerSelected;

    public List<Employer> Employers
    {
        get { return employers; }
        set { employers = value; OnPropertyChanged(); }
    }
    public Employer EmployerSelected
    {
        get { return employerSelected; }
        set { employerSelected = value; OnPropertyChanged(); }
    }
    public List<string> Commands
    {
        get { return commands; }
        set { commands = value; OnPropertyChanged(); }
    }
    public string FirstName
    {
        get { return firstname; }
        set { firstname = value; OnPropertyChanged(); }
    }
    public string SearchValue
    {
        get { return searchValue; }
        set { searchValue = value; OnPropertyChanged(); }
    }
    public string ChooseCommand
    {
        get { return chooseCommand; }
        set { chooseCommand = value; OnPropertyChanged(); }
    }
    public string LastName
    {
        get { return lastname; }
        set { lastname = value; OnPropertyChanged(); }
    }
    public string PickerValue
    {
        get { return pickerValue; }
        set { pickerValue = value; OnPropertyChanged(); }
    }
    public int Id
    {
        get { return id; }
        set { id = value; OnPropertyChanged(); }
    }
    public ICommand GetEmployerCommand { get; set; }
    public ICommand ClickedCommand { get; set; }
    public ICommand SearchCommand { get; set; }
}