using System.ComponentModel.DataAnnotations;
using TeisterMask.Data;


namespace TeisterMask.DataProcessor.ImportDto
{
    public class EmployeeInputDTO
    {
        [Required]
        [MinLength(GlobalConstants.EMPLOYEE_USERNAME_MINLENGTH)]
        [MaxLength(GlobalConstants.EMPLOYEE_USERNAME_MAXLENGTH)]
        [RegularExpression(@"^([A-Za-z\d])+$")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^([0-9]{3})-([0-9]{3})-([0-9]{4})$")]
        public string Phone { get; set; }

        public int[] Tasks { get; set; }
    }
}
