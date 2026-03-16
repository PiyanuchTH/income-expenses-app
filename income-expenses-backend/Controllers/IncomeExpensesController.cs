using IncomeExpenses.Data;
using IncomeExpenses.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncomeExpenses.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncomeExpensesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IncomeExpensesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetIncomeExpensesInformation")]
        public async Task<IActionResult> GetAll()
        {
            var transaction = await _context.Transaction
                .OrderByDescending(t => t.Date)
                .ToListAsync();
            return Ok(transaction);
        }

        [HttpPost("CreateIncomeExpensesInformation")]
        public async Task<IActionResult> Create([FromBody] Income model)
        {
            model.CreateDate = DateTime.Now;
            model.UpdateDate = null ;

            _context.Transaction.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        [HttpDelete("DeleteIncomeExpensesInformation/{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var data = await _context.Transaction.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            _context.Transaction.Remove(data);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Delete success" });
        }

        [HttpPut("UpdateIncomeExpensesInformation/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Income transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest();
            }

            var data = await _context.Transaction.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            data.Type = transaction.Type;
            data.Amount = transaction.Amount;
            data.Date = transaction.Date;
            data.Catagory = transaction.Catagory;
            data.Description = transaction.Description;
            data.UpdateDate = DateTime.Now;
            data.CreateDate = transaction.CreateDate;

            await _context.SaveChangesAsync();

            return Ok(data);
        }

        [HttpGet("GetFilterIncomeExpenses")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? type,
            [FromQuery] DateTime? specificDate,
            [FromQuery] int? month,
            [FromQuery] int? year)
            {
                var query = _context.Transaction.AsQueryable();

                if (!string.IsNullOrWhiteSpace(type))
                {
                    query = query.Where(x => x.Type == type);
                }

                if (specificDate.HasValue)
                {
                    var dateOnly = specificDate.Value.Date;
                    query = query.Where(x => x.Date.Date == dateOnly);
                }

                if (month.HasValue && year.HasValue)
                {
                    query = query.Where(x => x.Date.Month == month.Value && x.Date.Year == year.Value);
                }

                //if (month.HasValue)
                //{
                //    query = query.Where(x => x.Date.Month == month.Value);
                //}

                //if (year.HasValue)
                //{
                //    query = query.Where(x => x.Date.Year == year.Value);
                //}

            var transaction = await query
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();

                return Ok(transaction);
            }
    }
}
