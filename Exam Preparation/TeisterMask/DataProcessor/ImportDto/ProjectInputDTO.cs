using System.ComponentModel.DataAnnotations;

using System.Xml.Serialization;
using TeisterMask.Data;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Project")]
    public class ProjectInputDTO
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(GlobalConstants.PROJECT_NAME_MINLENGTH)]
        [MaxLength(GlobalConstants.PROJECT_NAME_MAXLENGTH)]
        public string Name { get; set; }

        [XmlElement("OpenDate")]
        [Required]
        public string OpenDate { get; set; }

        [XmlElement("DueDate")]
        public string DueDate { get; set; }

        [XmlArray("Tasks")]
        public TaskInputDTO[] Tasks { get; set; }
    }
}
