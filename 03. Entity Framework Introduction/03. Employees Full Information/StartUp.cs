using SoftUni.Data;
using System;
using System.Linq;
using System.Text;

namespace SoftUni
{
   public class StartUp
    {
        public static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();
            string result = GetEmployeesFullInformation(context);
            Console.WriteLine(result);
        }
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
                var employees = context
                    .Employees
                    .Select(e => new
                    {
                        e.EmployeeId,
                        e.FirstName,
                        e.MiddleName,
                        e.LastName,
                        e.JobTitle,
                        e.Salary
                    })
                    .OrderBy(e=>e.EmployeeId)
                    .ToList();

                StringBuilder sb = new StringBuilder();

                foreach (var e in employees)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:F2}");
                }

                return sb
                    .ToString()
                    .TrimEnd();
        }
    }
}
