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
            string result = DeleteProjectById(context);
            Console.WriteLine(result);
        }
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            const int id = 2;

            EmployeeProject[] employeeProjects = context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == id)
                .ToArray();

            context
                .EmployeesProjects
                .RemoveRange(employeeProjects);

            Project project = context
                .Projects
                .FirstOrDefault(p => p.ProjectId == id);

            context
                .Projects
                .Remove(project);

            context.SaveChanges();

            context
                .Projects
                .Take(10)
                .ToList()
                .ForEach(p => sb.AppendLine(p.Name));

            return sb
                .ToString();

        }
    }
}
