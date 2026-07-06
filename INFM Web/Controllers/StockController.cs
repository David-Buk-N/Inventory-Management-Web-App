using INFM_Web.Constants;
using INFM_Web.Data;
using INFM_Web.Models;
using INFM_Web.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace INFM_Web.Controllers
{
    // Both admins and warehouse workers manage stock levels.
    [Authorize(Roles = "Admin,Worker")]
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Stock  (optionally filtered by warehouse / product / low-stock)
        public async Task<IActionResult> Index(int? warehouseId, int? productId, bool lowStockOnly = false)
        {
            var query = _context.Stocks
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(s => s.Warehouse_Id == warehouseId);
            }
            if (productId.HasValue)
            {
                query = query.Where(s => s.Product_Id == productId);
            }
            if (lowStockOnly)
            {
                query = query.Where(s => s.Quantity <= s.Product!.ReorderLevel);
            }

            var stocks = await query
                .OrderBy(s => s.Warehouse!.WarehouseName)
                .ThenBy(s => s.Product!.Product_Name)
                .ToListAsync();

            ViewBag.Warehouses = new SelectList(
                await _context.Warehouses.OrderBy(w => w.WarehouseName).ToListAsync(),
                "Warehouse_Id", "WarehouseName", warehouseId);
            ViewBag.Products = new SelectList(
                await _context.Products.OrderBy(p => p.Product_Name).ToListAsync(),
                "Product_Id", "Product_Name", productId);
            ViewBag.LowStockOnly = lowStockOnly;

            return View(stocks);
        }

        // GET: Stock/Create  (assign a product to a warehouse)
        public async Task<IActionResult> Create()
        {
            await PopulateSelectLists();
            return View(new Stock());
        }

        // POST: Stock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Product_Id,Warehouse_Id,Quantity")] Stock stock)
        {
            var duplicate = await _context.Stocks
                .AnyAsync(s => s.Product_Id == stock.Product_Id && s.Warehouse_Id == stock.Warehouse_Id);
            if (duplicate)
            {
                ModelState.AddModelError(string.Empty,
                    "This product already has stock in that warehouse. Use Adjust to change its quantity.");
            }

            if (ModelState.IsValid)
            {
                stock.UpdatedDate = DateTime.Now;
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateSelectLists(stock);
            return View(stock);
        }

        // GET: Stock/Adjust/5  (receive or issue units)
        public async Task<IActionResult> Adjust(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .FirstOrDefaultAsync(s => s.Stock_Id == id);
            if (stock == null)
            {
                return NotFound();
            }

            var model = new StockAdjustViewModel
            {
                Stock_Id = stock.Stock_Id,
                ProductName = stock.Product?.Product_Name ?? "",
                WarehouseName = stock.Warehouse?.WarehouseName ?? "",
                CurrentQuantity = stock.Quantity
            };
            return View(model);
        }

        // POST: Stock/Adjust/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adjust(StockAdjustViewModel model)
        {
            var stock = await _context.Stocks
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .FirstOrDefaultAsync(s => s.Stock_Id == model.Stock_Id);
            if (stock == null)
            {
                return NotFound();
            }

            var newQuantity = stock.Quantity + model.Change;
            if (newQuantity < 0)
            {
                ModelState.AddModelError(nameof(model.Change),
                    $"Cannot issue {Math.Abs(model.Change)} units; only {stock.Quantity} on hand.");
            }

            if (ModelState.IsValid)
            {
                stock.Quantity = newQuantity;
                stock.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate display fields for the view.
            model.ProductName = stock.Product?.Product_Name ?? "";
            model.WarehouseName = stock.Warehouse?.WarehouseName ?? "";
            model.CurrentQuantity = stock.Quantity;
            return View(model);
        }

        // GET: Stock/Delete/5  (admin only)
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .FirstOrDefaultAsync(s => s.Stock_Id == id);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // POST: Stock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock != null)
            {
                _context.Stocks.Remove(stock);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateSelectLists(Stock? stock = null)
        {
            ViewBag.Products = new SelectList(
                await _context.Products.OrderBy(p => p.Product_Name).ToListAsync(),
                "Product_Id", "Product_Name", stock?.Product_Id);
            ViewBag.Warehouses = new SelectList(
                await _context.Warehouses.OrderBy(w => w.WarehouseName).ToListAsync(),
                "Warehouse_Id", "WarehouseName", stock?.Warehouse_Id);
        }
    }
}
