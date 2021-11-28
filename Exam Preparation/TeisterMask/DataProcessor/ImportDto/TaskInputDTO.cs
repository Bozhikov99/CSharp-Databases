using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using TeisterMask.Data;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Task")]
    public class TaskInputDTO
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(GlobalConstants.TASK_NAME_MINLENGTH)]
        [MaxLength(GlobalConstants.TASK_NAME_MAXLENGTH)]
        public string Name { get; set; }

        [XmlElement("OpenDate")]
        [Required]
        public string TaskOpenDate { get; set; }

        [XmlElement("DueDate")]
        [Required]
        public string TaskDueDate { get; set; }

        [XmlElement("ExecutionType")]
        [Range(GlobalConstants.TASK_LABEL_RANGE_MIN, GlobalConstants.TASK_LABEL_RANGE_MAX)]
        public int ExecutionType { get; set; }

        [XmlElement("LabelType")]
        [Range(GlobalConstants.TASK_LABEL_RANGE_MIN, GlobalConstants.TASK_LABEL_RANGE_MAX)]
        public int LabelType { get; set; }


    }
}
