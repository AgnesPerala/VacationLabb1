using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VacationLabb1.Models
{
    public class Vacation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VacationId { get; set; }

        [ForeignKey("Employee")]

        public int FkEmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string VacationType { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsHistory { get; set; }
    }
}
