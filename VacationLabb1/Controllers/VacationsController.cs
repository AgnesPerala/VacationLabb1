using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VacationLabb1.Data;
using VacationLabb1.Models;

namespace VacationLabb1.Controllers
{
    public class VacationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VacationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vacations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Vacation.Include(v => v.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vacations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacation = await _context.Vacation
                .Include(v => v.Employee)
                .FirstOrDefaultAsync(m => m.VacationId == id);
            if (vacation == null)
            {
                return NotFound();
            }

            return View(vacation);
        }

        // GET: Vacations/Create
        public IActionResult Create()
        {
            ViewData["FkEmployeeId"] = new SelectList(_context.Employee, "EmployeeId", "EmployeeName");
            return View();
        }

        // POST: Vacations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VacationId,FkEmployeeId,StartDate,EndDate,VacationType,RegistrationDate,IsHistory")] Vacation vacation)
        {
            if (ModelState.IsValid)
            {
                vacation.RegistrationDate = DateTime.Now;

                // Bestäm IsHistory baserat på datum
                vacation.IsHistory = vacation.EndDate < DateTime.Now;

                _context.Add(vacation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vacation);
        }

        // GET: Vacations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacation = await _context.Vacation.FindAsync(id);
            if (vacation == null)
            {
                return NotFound();
            }
            ViewData["FkEmployeeId"] = new SelectList(_context.Employee, "EmployeeId", "EmployeeName", vacation.FkEmployeeId);
            return View(vacation);
        }

        // POST: Vacations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VacationId,FkEmployeeId,StartDate,EndDate,VacationType,RegistrationDate,IsHistory")] Vacation vacation)
        {
            if (id != vacation.VacationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    vacation.RegistrationDate = DateTime.Now;
                    vacation.IsHistory = vacation.EndDate < DateTime.Now;
                    _context.Update(vacation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacationExists(vacation.VacationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkEmployeeId"] = new SelectList(_context.Employee, "EmployeeId", "EmployeeName", vacation.FkEmployeeId);
            return View(vacation);
        }

        // GET: Vacations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacation = await _context.Vacation
                .Include(v => v.Employee)
                .FirstOrDefaultAsync(m => m.VacationId == id);
            if (vacation == null)
            {
                return NotFound();
            }

            return View(vacation);
        }

        // POST: Vacations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vacation = await _context.Vacation.FindAsync(id);
            if (vacation != null)
            {
                _context.Vacation.Remove(vacation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Vacations/Search
        public async Task<IActionResult> Search(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return View();
            }

            var vacations = await _context.Vacation
                .Include(v => v.Employee)
                .Where(v => v.Employee.EmployeeName.Contains(searchString))
                .ToListAsync();

            ViewBag.SearchString = searchString;
            return View("Index", vacations);
        }

        // GET: Vacations/ByMonth
        public async Task<IActionResult> ByMonth(DateTime? month)
        {
            if (!month.HasValue)
            {
                return BadRequest("Ange en månad.");
            }

            var startDate = new DateTime(month.Value.Year, month.Value.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var vacationApplications = await _context.Vacation
                .Where(v => v.RegistrationDate >= startDate && v.RegistrationDate <= endDate)
                .GroupBy(v => new { v.FkEmployeeId, v.Employee.EmployeeName })
                .Select(g => new VacationByEmployeeViewModel
                {
                    EmployeeId = g.Key.FkEmployeeId,
                    EmployeeName = g.Key.EmployeeName,
                    TotalDaysRequested = g.Sum(v => (v.EndDate - v.StartDate).Days),
                    ApplicationDates = g.Select(v => v.RegistrationDate).ToList()
                })
                .ToListAsync();

            return View(vacationApplications);
        }


        private bool VacationExists(int id)
        {
            return _context.Vacation.Any(e => e.VacationId == id);
        }
    }
}
