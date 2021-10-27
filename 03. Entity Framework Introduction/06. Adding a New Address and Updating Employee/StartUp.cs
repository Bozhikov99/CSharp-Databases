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
            string result = AddNewAddressToEmployee(context);
            Console.WriteLine(result);
        }
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            context
                .Employees
                .First(e => e.LastName == "Nakov")
                .Address = new Address()
                {
                    AddressText = "Vitoshka 15",
                    TownId = 4
                };

            context.SaveChanges();

            context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Select(e => new
                {
                    address = e.Address.AddressText
                })
                .Take(10)
                .ToList()
                .ForEach(e => sb.AppendLine(e.address));

            return sb
                .ToString()
                .TrimEnd();
        }
    }
}
