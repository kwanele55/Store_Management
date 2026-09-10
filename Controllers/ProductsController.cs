using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    // ProductsController handles all CRUD operations for Products
    public class ProductsController : Controller
    {
        // EF Core database context for accessing the database
        private readonly StoreManagementDbContext _context;

        // Constructor - injects the database context
        public ProductsController(StoreManagementDbContext context)
        {
            _context = context;
        }

        // GET: Products
        // Uses LINQ Include to load Category, OrderBy to sort by name
        public async Task<IActionResult> Index()
        {
            var products = _context.Products
                                   .Include(p => p.Category)
                                   .OrderBy(p => p.ProductName);
            return View(await products.ToListAsync());
        }

        // GET: Products/Details/5
        // Retrieves a single product with its category using LINQ
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            // LINQ FirstOrDefaultAsync returns null if not found
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: Products/Create
        // Loads the create form with category dropdown
        public IActionResult Create()
        {
            // Populate category dropdown using LINQ OrderBy
            ViewData["CategoryId"] = new SelectList(
                _context.Categories.OrderBy(c => c.CategoryName),
                "CategoryId", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // Saves a new product then redirects with success message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            // Remove validation for navigation properties
            ModelState.Remove("Category");
            ModelState.Remove("OrderItems");

            if (ModelState.IsValid)
            {
                // Add the new product and save to database
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product added successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(
                _context.Categories.OrderBy(c => c.CategoryName),
                "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        // Loads the edit form for an existing product
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            ViewData["CategoryId"] = new SelectList(
                _context.Categories.OrderBy(c => c.CategoryName),
                "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // Updates an existing product then redirects with success message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId)
                return NotFound();

            // Remove validation for navigation properties not in the form
            ModelState.Remove("Category");
            ModelState.Remove("OrderItems");

            if (ModelState.IsValid)
            {
                try
                {
                    // Update product and save changes to database
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Product updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Check if product still exists using helper method
                    if (!ProductExists(product.ProductId))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["CategoryId"] = new SelectList(
                _context.Categories.OrderBy(c => c.CategoryName),
                "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        // Shows delete confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        // Deletes product only if it has no related order items
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // LINQ Any - check if product is linked to any order items
            bool hasOrderItems = _context.OrderItems
                                         .Any(oi => oi.ProductId == id);

            if (hasOrderItems)
            {
                // Block delete and show friendly error message
                TempData["Error"] = "Cannot delete this product because it is linked to existing orders.";
                return RedirectToAction(nameof(Index));
            }

            // Safe to delete - no linked order items found
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper method - checks if a product exists by ID
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}