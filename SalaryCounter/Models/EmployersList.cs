using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCounter.Models;

public class EmployersList
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int IdEmployer { get; set; }
    public int IdTask { get; set; }
}
