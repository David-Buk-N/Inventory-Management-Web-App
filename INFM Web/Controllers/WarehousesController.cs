using INFM_Web.Constants;
using INFM_Web.Data;
using INFM_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INFM_Web.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class WarehousesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WarehousesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Warehouses
        public async Task<IActionResult> Index()
        {
            var warehouses = await _context.Warehouses
                .Include(w => w.Stocks)
                .OrderBy(w => w.WarehouseName)
                .ToListAsync();
            return View(warehouses);
        }

        // GET: Warehouses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .Include(w => w.Stocks)!
                    .ThenInclude(s => s.Product)
                .FirstOrDefaultAsync(w => w.Warehouse_Id == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // GET: Warehouses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Warehouses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WarehouseName,Code,Location,City,Capacity")] Warehouse warehouse)
        {
            if (await _context.Warehouses.AnyAsync(w => w.Code == warehouse.Code))
            {
                ModelState.AddModelError(nameof(Warehouse.Code), "A warehouse with this code already exists.");
            }

            if (ModelState.IsValid)
            {
                warehouse.CreatedDate = DateTime.Now;
                _context.Add(warehouse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(warehouse);
        }

        // GET: Warehouses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            return View(warehouse);
        }

        // POST: Warehouses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Warehouse_Id,WarehouseName,Code,Location,City,Capacity")] Warehouse warehouse)
        {
            if (id != warehouse.Warehouse_Id)
            {
                return NotFound();
            }

            if (await _context.Warehouses.AnyAsync(w => w.Code == warehouse.Code && w.Warehouse_Id != id))
            {
                ModelState.AddModelError(nameof(Warehouse.Code), "A warehouse with this code already exists.");
            }

            if (ModelState.IsValid)
            {
                // Load the tracked entity so the created date and stock rows are preserved.
                var existing = await _context.Warehouses.FindAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                existing.WarehouseName = warehouse.WarehouseName;
                existing.Code = warehouse.Code;
                existing.Location = warehouse.Location;
                existing.City = warehouse.City;
                existing.Capacity = warehouse.Capacity;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(warehouse);
        }

        // GET: Warehouses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .Include(w => w.Stocks)
                .FirstOrDefaultAsync(w => w.Warehouse_Id == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // POST: Warehouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse != null)
            {
                _context.Warehouses.Remove(warehouse);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
