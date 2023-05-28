using System.Diagnostics.Metrics;
using System.Reflection;

namespace SalaryCounter.Models;

public class AppRepository : IDisposable
{
    public readonly SQLiteConnection database;
//@"C:\Users\alexm\Desktop\otchot.csv";//
    public readonly static string HolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "otchot.csv");
    public AppRepository(string dbName)
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbName);
        //var dbPath = "C:\\Users\\alexm\\OneDrive\\Рабочий стол";
        database = new SQLiteConnection(dbPath);
        database.CreateTable<Employer>();
        database.CreateTable<EmployerTask>();
        database.CreateTable<EmployersList>();
        if (!File.Exists(HolderPath))
        {
            File.Create(HolderPath);
        }
    }
    //Задачи на экране
    public List<TaskVisible> GetTasks()
    {
        var tasks = new List<TaskVisible>();
        var itemTask = database.Table<EmployerTask>().ToList();
        foreach (var item in itemTask)
        {
            string temp = "";
            var employer = database.Table<EmployersList>().Where(x => x.IdTask == item.EmployersId).ToList();
            foreach (var item2 in employer)
            {
                string tempstr = database.Table<Employer>().Where(x => x.Id == item2.IdEmployer).First().LastName;
                temp = temp + tempstr + ",";
            }
            var task = new TaskVisible
            {
                Id = item.Id,
                Name = item.Name,
                Rate = item.Rate,
                StartTime = item.StartTime,
                EndTime = item.EndTime,
                ResultTime = item.ResultTime,
                Employers = temp
            };
            tasks.Add(task);
        }
        return tasks;
    }

    public int UpdateEmployerTask(int id,DateTime temp)
    {
        var findTask = FindTask(id);
        findTask.ResultTime = temp;
        return database.Update(findTask);
    }
    //Поиски
    public int FindLastEmployerTask()
    {
        if(database.Table<EmployerTask>().FirstOrDefault()!=null)
            return database.Table<EmployerTask>().Last().Id;
        return 0;
    }
    public EmployerTask FindTask(int id)
    {
        return database.Table<EmployerTask>().Where(x => x.Id == id).FirstOrDefault();
    }
    public List<Employer> FindEmployerSalary(int id)
    {
        var result = new List<Employer>();
        var employers = database.Table<EmployersList>().Where(x => x.IdTask == id).ToList();
        foreach (var employer in employers)
        {
            var temp = database.Table<Employer>().Where(x => x.Id == employer.IdEmployer).FirstOrDefault();
            result.Add(temp);
        }
        return result;
    }
    public List<Employer> FindEmployers(int id)
    {
        var result = new List<Employer>();
     //   var result2 = database.Query<Employer>("SELECT employer.* from Employer employer INNER JOIN EmployersList el on el.IdEmployer = employer.Id WHERE el.IdEmployer = ?", id);
         var employers = database.Table<EmployersList>().Where(x => x.IdEmployer == id).ToList();
         foreach (var employer in employers)
         { 
             var temp = database.Table<Employer>().Where(x => x.Id == employer.IdEmployer).FirstOrDefault();
            result.Add(temp);
         }
        return result;
    }
    public Employer FindEmployer(int id)
    {
        var result = database.Table<Employer>().Where(x => x.Id == id).FirstOrDefault();
        return result;
    }
    public int FindEmployerId(string name)
    {
        var result = database.Table<Employer>().Where(x => x.LastName.Equals(name)).FirstOrDefault().Id;
        return result;
    }
    public string FindEmployerEfficiency(int id)
    {
        int time = 0;
        int counter = 0;
        string result = "";
        var employers = database.Table<EmployersList>().Where(x => x.IdEmployer == id).ToList();
        foreach (var employer in employers)
        {
            var task = database.Table<EmployerTask>().Where(x => x.Id == employer.IdTask).FirstOrDefault();
            if (task != null)
            {
                if (task.ResultTime > DateTime.MinValue)
                {
                    counter++;
                    time = time + task.ResultTime.Subtract(task.StartTime).Days;
                    if (task.ResultTime.Subtract(task.StartTime).Days == 0)
                        time++;
                }
            }
        }
        if (time != 0)
            result = result + Math.Round(((decimal)counter / (decimal)time * 100), 2);
        else result = "0";
        return result;
    }
    public List<string> FindTerms(int id)
    {
        List<string> resultList = new();
        int termsOk = 0;
        int termsCancel = 0;
        var employers = database.Table<EmployersList>().Where(x => x.IdEmployer == id).ToList();
        foreach (var employer in employers)
        {
            var task = database.Table<EmployerTask>().Where(x => x.Id == employer.IdTask).FirstOrDefault();
            if (task != null && task.ResultTime!=DateTime.MinValue)
            {
                if (task.ResultTime.Year > task.EndTime.Year && task.ResultTime.Month > task.EndTime.Month && task.ResultTime.Day > task.EndTime.Day)
                {
                    termsCancel++;
                } else
                if (task.ResultTime.Year == task.EndTime.Year && task.ResultTime.Month == task.EndTime.Month && task.ResultTime.Day == task.EndTime.Day)
                {
                    termsOk++;
                } else
                if (task.ResultTime < task.EndTime)
                {
                    termsOk++;
                }
                resultList.Add(termsOk.ToString());
                resultList.Add(termsCancel.ToString());
            }
        }

        return resultList;
    }
    public List<string> FindEmployerLaborCost()
    {
        var result = new List<string>();
        var tasks = GetEmployerTasks();
        foreach (var task in tasks)
        {
            var time = task.EndTime.Subtract(task.StartTime).Days;
            if (task.EndTime != DateTime.MinValue && task.StartTime != DateTime.MinValue & time == 0)
                time = 1;
            var employers = database.Table<EmployersList>().Where(x => x.IdTask == task.EmployersId).ToList();
            var count = employers.Count();
            var laborCost = (decimal)time/ (decimal)count;
            result.Add($"{task.Id},{task.Name},{Math.Round(laborCost,2)}\n");
        }
        return result;
    }
    public List<string> FindEmployerLaborCostTask(int id)
    {
        var result = new List<string>();
        var task = FindTask(id);
            var time = task.EndTime.Subtract(task.StartTime).Days;
            if (task.EndTime != DateTime.MinValue && task.StartTime != DateTime.MinValue & time == 0)
                time = 1;
            var employers = database.Table<EmployersList>().Where(x => x.IdTask == task.EmployersId).ToList();
            var count = employers.Count();
            var laborCost = (decimal)time / (decimal)count;
            result.Add($"{task.Id},{task.Name},{Math.Round(laborCost, 2)}\n");
        return result;
    }

    //Задачи в бд
    public List<EmployerTask> GetEmployerTasks()
    {
        return database.Table<EmployerTask>().ToList();
    }
    public int CreateEmployerTask(EmployerTask employer)
    {
        return database.Insert(employer);
    }
    public int UpdateEmployerTask(EmployerTask employer)
    {
        return database.Update(employer);
    }
    
    public int DeleteEmployerTask(int id)
    {
        var findTask = FindTask(id);
        var tasks = database.Table<EmployersList>().Where(x => x.IdTask == id).ToList();
        foreach (var task in tasks)
        {
            database.Delete(task);
        }
        return database.Delete(findTask);
    }
    // Работники в задачах 
    public List<EmployersList> GetEmployersList()
    {
        return database.Table<EmployersList>().ToList();
    }
    public int CreateEmployersList(EmployersList employer)
    {
        return database.Insert(employer);
    }
    public int UpdateEmployersList(EmployersList employer)
    {
        return database.Update(employer);
    }
    public void DeleteEmployersList(int id)
    {
        var findTask = FindTask(id);
        var tasks = database.Table<EmployersList>().Where(x => x.IdTask == id).ToList();
        foreach (var task in tasks)
        {
            database.Delete(task);
        }
    }
    //Работники
    public async Task<List<Employer>> GetEmployers()
    {
        var result = database.Table<Employer>().ToList();
        return result;
    }
    public List<Employer> GetEmployerss()
    {
        var result = database.Table<Employer>().ToList();
        return result;
    }
    public int CreateEmployer(Employer employer)
    {
        return database.Insert(employer);
    }
    public int UpdateEmployer(Employer employer)
    {
        return database.Update(employer);
    }
    public int DeleteEmployer(Employer employer)
    {
        return database.Delete(employer);
    }
    public void Dispose()
    {
        database.Dispose();
    }
}
