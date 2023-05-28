namespace SalaryCounter.Models;
public class EmployerTask 
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Rate { get; set; }
    public int EmployersId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime ResultTime { get; set; }
}
