namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<TeisterMaskProfile>());
            IMapper mapper = new Mapper(config);

            Project[] projects = context.Projects
                .Where(p => p.Tasks.Count > 0)
                .OrderByDescending(p => p.Tasks.Count)
                .ThenBy(p => p.Name)
                .ToArray();

            ProjectOutputDTO[] dtos = mapper.Map<ProjectOutputDTO[]>(projects);

            StringBuilder sb = new StringBuilder();
            XmlRootAttribute root = new XmlRootAttribute("Projects");
            XmlSerializer serializer = new XmlSerializer(typeof(ProjectOutputDTO[]), root);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter(sb);
            serializer.Serialize(writer, dtos, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            Employee[] employees = context.Employees
                .Where(e => e.EmployeesTasks.Select(et => et.Task.OpenDate).Any(d => d >= date))
                .OrderByDescending(e => e.EmployeesTasks.Select(et => et.Task.OpenDate).Where(d => d >= date).Count())
                .ThenBy(e => e.Username)
                .Take(10)
                .ToArray();

            List<EmployeeOutputDTO> dtos = new List<EmployeeOutputDTO>();

            foreach (Employee e in employees)
            {
                Task[] taskMatches = e.EmployeesTasks.Select(et => et.Task).Where(t => t.OpenDate >= date)
                    .OrderByDescending(t => t.DueDate)
                    .ThenBy(t => t.Name)
                    .ToArray();

                List<TaskOutputDTO> taskDtos = new List<TaskOutputDTO>();

                foreach (Task t in taskMatches)
                {
                    TaskOutputDTO taskDto = new TaskOutputDTO()
                    {
                        TaskName = t.Name,
                        OpenDate = t.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = t.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = t.LabelType.ToString(),
                        ExecutionType = t.ExecutionType.ToString()
                    };

                    taskDtos.Add(taskDto);
                }

                EmployeeOutputDTO dto = new EmployeeOutputDTO()
                {
                    Username = e.Username,
                    Tasks = taskDtos
                };

                dtos.Add(dto);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(JsonConvert.SerializeObject(dtos, Formatting.Indented));

            return sb.ToString().Trim();
        }
    }
}