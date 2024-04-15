using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationLabb1.Data;
using VacationLabb1.Models;

namespace VacationLabb1.Controllers
{
    public class VacationByEmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VacationByEmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vacation
        public async Task<IActionResult> Index(DateTime? selectedMonth)
        {
            if (!selectedMonth.HasValue)
            {
                selectedMonth = DateTime.Now;
            }
            var firstDayOfMonth = new DateTime(selectedMonth.Value.Year, selectedMonth.Value.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var vacations = await _context.Vacation
                .Include(v => v.Employee)
                .Where(v => v.StartDate >= firstDayOfMonth && v.EndDate <= lastDayOfMonth)
                .ToListAsync();

            // Calculate total days requested by the employee
            var totalDaysRequested = vacations.Sum(v => (v.EndDate - v.StartDate).Days);

            // Pass both vacations and totalDaysRequested to the view
            var viewModel = new VacationByEmployeeViewModel
            {
                vacations = vacations,
                TotalDaysRequested = totalDaysRequested,
                SelectedMonth = selectedMonth ?? DateTime.Now
            }; 
            return View(viewModel);
        }
    }
}
