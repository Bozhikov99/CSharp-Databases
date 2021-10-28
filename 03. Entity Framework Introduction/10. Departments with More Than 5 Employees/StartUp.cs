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
            string result = GetDepartmentsWithMoreThan5Employees(context);
            Console.WriteLine(result);
        }
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departments = context
                .Departments
                .Where(d => d.Employees.Count > 5)
                .Select(d => new
                {
                    d.Name,
                    firstName = d.Manager
                    .FirstName,
                    lastName = d.Manager
                    .LastName,
                    d.Manager,
                    d.Employees
                })
                .ToList();

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.Name} - {d.firstName} {d.lastName}");

                foreach (var e in d.Employees
                    .OrderBy(em => em.FirstName)
                    .ThenBy(em => em.LastName)
                    .ToList())
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb
                .ToString();

        }
    }
}