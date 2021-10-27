using SoftUni.Data;
using SoftUni.Models;
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
            string result = GetEmployee147(context);
            Console.WriteLine(result);
        }
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            const int employeeId = 147;

            var employee = context
                .Employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects
                        .Select(ep => ep.Project.Name)
                        .OrderBy(p => p)
                        .ToList()

                })
                .FirstOrDefault(e => e.EmployeeId == employeeId);

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var p in employee.Projects)
            {
                sb.AppendLine(p);
            }

            //StringBuilder sb = new StringBuilder();

            // context
            //    .Addresses
            //    .GroupBy(a => new
            //    {
            //        a.AddressId,
            //        a.AddressText,
            //        a.Town.Name
            //    },
            //        (key, group) => new
            //        {
            //            key.AddressText,
            //            Town = key.Name,
            //            Count = group.Sum(a => a.Employees.Count)
            //        })
            //    .OrderByDescending(a => a.Count)
            //    .ThenBy(a => a.Town)
            //    .ThenBy(a => a.AddressText)
            //    .Take(10)
            //    .ToList()
            //  .ForEach(a => sb.AppendLine($"{a.AddressText}, {a.Town} - {a.Count} employees"));



            return sb
                .ToString()
                .TrimEnd();

        }
    }
}