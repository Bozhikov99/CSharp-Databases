using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();
            string result = IncreaseSalaries(context);
            Console.WriteLine(result);
        }
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            string[] departmentNames = { "Engineering", "Tool Design", "Marketing", "Information Services" };

            IQueryable<Employee> employees = context
                  .Employees
                  .Where(e => departmentNames.Contains(e.Department.Name))
                  .Select(e => e);

            foreach (Employee e in employees)
            {
                e.Salary *= 1.12m;
            }

            context.SaveChanges();

            List<Employee> employeesList = employees
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (Employee e in employeesList)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:F2})");
            }

            //var departments = context
            //    .Departments
            //    .Where(d => d.Employees.Count > 5)
            //    .Select(d => new
            //    {
            //        d.Name,
            //        firstName = d.Manager
            //        .FirstName,
            //        lastName = d.Manager
            //        .LastName,
            //        d.Manager,
            //        d.Employees
            //    })
            //    .ToList();

            //foreach (var d in departments)
            //{
            //    sb.AppendLine($"{d.Name} - {d.firstName} {d.lastName}");

            //    foreach (var e in d.Employees
            //        .OrderBy(em=>em.FirstName)
            //        .ThenBy(em=>em.LastName))
            //    {
            //        sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
            //    }
            //}

            return sb
                .ToString();

        }
    }
}