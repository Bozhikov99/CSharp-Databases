namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using Task = Data.Models.Task;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute root = new XmlRootAttribute("Projects");
            XmlSerializer serializer = new XmlSerializer(typeof(ProjectInputDTO[]), root);
            using StringReader reader = new StringReader(xmlString);

            ProjectInputDTO[] dTOs = (ProjectInputDTO[])serializer.Deserialize(reader);
            List<Project> projects = new List<Project>();


            foreach (ProjectInputDTO d in dTOs)
            {
                if (!IsValid(d))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isOpenDateValid = DateTime.TryParseExact(d.OpenDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime projectOpenDate);

                if (!isOpenDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? projectDueDate = null;

                if (!string.IsNullOrWhiteSpace(d.DueDate))
                {
                    bool isDueDateValid = DateTime.TryParseExact(d.DueDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDateResult);

                    if (isDueDateValid)
                    {
                        projectDueDate = dueDateResult;
                    }
                }

                Project current = new Project()
                {
                    Name = d.Name,
                    OpenDate = projectOpenDate,
                    DueDate = projectDueDate
                };

                ICollection<Task> tasksForCurrentProject = new List<Task>();

                foreach (var t in d.Tasks.Distinct())
                {

                    if (!IsValid(t))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isTaskOpenDateValid = DateTime.TryParseExact(t.TaskOpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime taskOpenDate);
                    bool isTaskDueDateValid = DateTime.TryParseExact(t.TaskDueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime taskDueDate);

                    if (!isTaskDueDateValid || !isTaskOpenDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (taskOpenDate < projectOpenDate ||
                        taskDueDate > projectDueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Task currentTask = new Task()
                    {
                        Name = t.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)t.ExecutionType,
                        LabelType = (LabelType)t.LabelType
                    };

                    tasksForCurrentProject.Add(currentTask);
                }

                current.Tasks = tasksForCurrentProject;
                projects.Add(current);
                sb.AppendLine(string.Format(SuccessfullyImportedProject, current.Name, tasksForCurrentProject.Count));
            }

            context.Projects.AddRange(projects);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            EmployeeInputDTO[] dTOs = JsonConvert.DeserializeObject<EmployeeInputDTO[]>(jsonString);
            ICollection<Employee> employees = new List<Employee>();

            foreach (EmployeeInputDTO dTO in dTOs)
            {
                if (!IsValid(dTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Employee currentEmployee = new Employee()
                {
                    Username = dTO.Username,
                    Email = dTO.Email,
                    Phone = dTO.Phone
                };

                ICollection<EmployeeTask> employeeTasks = new List<EmployeeTask>();

                foreach (var taskId in dTO.Tasks.Distinct())
                {
                    Task currentTask = context
                        .Tasks
                        .Find(taskId);

                    if (currentTask == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    EmployeeTask currentEmployeeTask = new EmployeeTask()
                    {
                        Employee = currentEmployee,
                        Task = currentTask
                    };

                    employeeTasks.Add(currentEmployeeTask);
                }

                currentEmployee.EmployeesTasks = employeeTasks;
                employees.Add(currentEmployee);
                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, currentEmployee.Username, employeeTasks.Count));
            }

            context.Employees.AddRange(employees);
            context.SaveChanges();


            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}