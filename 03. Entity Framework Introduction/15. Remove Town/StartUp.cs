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
            string result = RemoveTown(context);
            Console.WriteLine(result);
        }
        public static string RemoveTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            const string townName = "Seattle";

            var addresses = context
                 .Addresses
                 .Where(a=>a.Town.Name==townName)
                 .ToArray();

            context
                .Addresses
                .RemoveRange(addresses);

            context
                .Towns
                .Remove(context.Towns.FirstOrDefault(t => t.Name == townName));

            sb.AppendLine($"{addresses.Length} addresses in {townName} were deleted");

            context.SaveChanges();

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