using System.ComponentModel.DataAnnotations;

namespace VacationLabb1.Models
{
    public class VacationByEmployeeViewModel
    {
        public IEnumerable<Vacation> vacations { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int TotalDaysRequested { get; set; }
        public DateTime SelectedMonth { get; set; }
        public List<DateTime> ApplicationDates { get; set; }
    }
}
