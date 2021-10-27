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
            string result = GetEmployeesByFirstNameStartingWithSa(context);
            Console.WriteLine(result);
        }
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            context
                .Employees
                .Where(e => e.FirstName
                    .ToLower()
                    .Substring(0, 2) == "sa")
                    .OrderBy(e=>e.FirstName)
                    .ThenBy(e=>e.LastName)
                    .ToList()
                    .ForEach(e=>sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})"));

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