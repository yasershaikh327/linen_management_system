namespace LinenManagementAPI.Models;

public class Employee
{
    public int EmployeeId { get; set; }
    public string? Email { get; set; }  // allow null
    public string? Name { get; set; }   // allow null
    public string? Password { get; set; }
    public string? RefreshToken { get; set; }
}