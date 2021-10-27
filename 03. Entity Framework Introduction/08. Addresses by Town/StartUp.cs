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
            string result = GetAddressesByTown(context);
            Console.WriteLine(result);
        }
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            

            context
               .Addresses
               .Select(a => new
               {
                   a.AddressText,
                   Town = a.Town.Name,
                   EmployeeCount = a.Employees.Count
               })
               .OrderByDescending(a => a.EmployeeCount)
               .ThenBy(a => a.Town)
               .ThenBy(a => a.AddressText)
               .Take(10)
               .ToList()
             .ForEach(a => sb.AppendLine($"{a.AddressText}, {a.Town} - {a.EmployeeCount} employees"));



            return sb
                .ToString();

        }
    }
}