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
            string result = GetEmployeesFromResearchAndDevelopment(context);
            Console.WriteLine(result);
        }
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary,
                    DepartmentName = e.Department.Name
                })
                .ToList()
                .ForEach(e => sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:F2}"));

            return sb
                .ToString()
                .TrimEnd();
        }
    }
}
