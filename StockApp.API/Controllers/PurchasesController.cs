using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard-purchases")]
        public async Task<IActionResult> GetDashboardPurchasesData()
        {
            var dashboardData = new PurchasesDashboardDTO
            {
                TotalPurchases = await _context.Purchases.CountAsync(),
                TotalSpent = await _context.Purchases.SumAsync(p => p.Quantity * p.Price),
                TopSuppliers = await _context.Suppliers.OrderByDescending(s => s.Purchases.Sum(p => p.Quantity)).Take(5).Select(s => new PurchasesSupplierDTO
                {
                    SupplierName = s.Name,
                    TotalPurchases = s.Purchases.Sum(p => p.Quantity)
                }).ToListAsync()
            };
            return Ok(dashboardData);
        }
    }
}
