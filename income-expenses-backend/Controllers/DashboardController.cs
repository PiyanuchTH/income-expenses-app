using IncomeExpenses.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncomeExpenses.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetTotalIncome")]
        public async Task<IActionResult> GetTotalIncome(int? month, int? year)
        {
            var currentMonth = month ?? DateTime.Now.Month;
            var currentYear = year ?? DateTime.Now.Year;

            var totalIncome = await _context.Transaction
                .Where(t => t.Type == "Income"
                         && t.Date.Month == currentMonth
                         && t.Date.Year == currentYear)
                .Select(t => t.Amount)
                .SumAsync();

            return Ok(new
            {
                Month = currentMonth,
                Year = currentYear,
                TotalIncome = totalIncome
            });
        }

        [HttpGet("GetTotalExpenses")]
        public async Task<IActionResult> GetTotalExpenses(int? month, int? year)
        {
            var currentMonth = month ?? DateTime.Now.Month;
            var currentYear = year ?? DateTime.Now.Year;

            var totalExpense = await _context.Transaction
                .Where(t => t.Type == "Expense"
                         && t.Date.Month == currentMonth
                         && t.Date.Year == currentYear)
                .Select(t => t.Amount)
                .SumAsync();

            return Ok(new
            {
                Month = currentMonth,
                Year = currentYear,
                TotalExpense = totalExpense
            });
        }
    }
}
