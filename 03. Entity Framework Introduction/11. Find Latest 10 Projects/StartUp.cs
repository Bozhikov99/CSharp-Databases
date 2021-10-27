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
        
            return sb
                .ToString();

        }
    }
}
