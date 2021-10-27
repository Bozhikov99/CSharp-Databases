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
            string result = GetLatestProjects(context);
            Console.WriteLine(result);
        }
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            context
                .Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p=>p.Name)
                .ToList()
                .ForEach(p => 
                {
                    DateTime date = p.StartDate;
                    sb.AppendLine(p.Name);
                    sb.AppendLine(p.Description);
                    sb.AppendLine($"{date.ToString("M/d/yyyy h:mm:ss")} {(date.Hour < 12 ? "AM":"PM")}");
        });
        
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