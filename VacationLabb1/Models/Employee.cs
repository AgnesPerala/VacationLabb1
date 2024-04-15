using System.ComponentModel.DataAnnotations;

namespace VacationLabb1.Models
{
    public class Employee
    {
        [Key] public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee måste ha ett namn")]
        [StringLength(60, ErrorMessage = "Namnet får inte vara längre än 60 tecken")]
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
    }
}
