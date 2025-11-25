using LinenManagementAPI.Data;
using System;
using BC = BCrypt.Net.BCrypt;



public interface IHashPassword
{
    public void HashPasswords();
}

public class HashPassword : IHashPassword
{
    private readonly ApplicationDbContext _context;

    public HashPassword(ApplicationDbContext context)
    {
        _context = context;
    }
    public void HashPasswords()
    {
        string password = "Test@123";
        string hash = BC.HashPassword(password);
        var employeeList = _context.Employees.ToList();
        var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == 2);
        if (employee != null)
        {
            employee.Password = hash;
            _context.SaveChanges();
        }
    }
}