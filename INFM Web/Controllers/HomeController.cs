using INFM_Web.Data;
using INFM_Web.Models;
using INFM_Web.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace INFM_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        // Back-office inventory dashboard.
        [Authorize(Roles = "Admin,Worker")]
        public async Task<IActionResult> Dash()
        {
            var model = new DashboardViewModel
            {
                ProductCount = await _context.Products.CountAsync(),
                SupplierCount = await _context.Suppliers.CountAsync(),
                WarehouseCount = await _context.Warehouses.CountAsync(),
                TotalUnitsOnHand = await _context.Stocks.SumAsync(s => (int?)s.Quantity) ?? 0,
                LowStockCount = await _context.Products
                    .Where(p => (p.Stocks.Sum(s => (int?)s.Quantity) ?? 0) <= p.ReorderLevel)
                    .CountAsync()
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
