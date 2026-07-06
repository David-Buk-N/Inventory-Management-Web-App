using System.Threading.Tasks;
using INFM_Web.Constants;
using INFM_Web.Data;
using INFM_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace INFM_Web.Controllers
{
    // Everyone in the back-office can browse the catalogue; only admins may change it.
    [Authorize(Roles = "Admin,Worker")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Stocks)
                .ToListAsync();
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Stocks)!
                    .ThenInclude(s => s.Warehouse)
                .FirstOrDefaultAsync(m => m.Product_Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> Create()
        {
            await PopulateSelectLists();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> Create([Bind("Product_Name,SKU,Product_Price,ProductDescription,ReorderLevel,Category_Id,Supplier_Id")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateSelectLists(product);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            await PopulateSelectLists(product);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> Edit(int id, [Bind("Product_Id,Product_Name,SKU,Product_Price,ProductDescription,ReorderLevel,Category_Id,Supplier_Id")] Product product)
        {
            if (id != product.Product_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Product_Id))
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
            await PopulateSelectLists(product);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Product_Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Product_Id == id);
        }

        private async Task PopulateSelectLists(Product? product = null)
        {
            ViewBag.Categories = new SelectList(
                await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync(),
                "Category_Id", "CategoryName", product?.Category_Id);
            ViewBag.Suppliers = new SelectList(
                await _context.Suppliers.OrderBy(s => s.SupplierName).ToListAsync(),
                "Supplier_Id", "SupplierName", product?.Supplier_Id);
        }
    }
}
