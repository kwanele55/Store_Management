using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    // CategoriesController handles all CRUD operations for Categories
    public class CategoriesController : Controller
    {
        // EF Core database context
        private readonly StoreManagementDbContext _context;

        // Constructor - injects the database context
        public CategoriesController(StoreManagementDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        // Uses LINQ Where to get active categories, OrderBy to sort
        public async Task<IActionResult> Index()
        {
            var categories = _context.Categories
                                     .Where(c => c.IsActive == true)
                                     .OrderBy(c => c.CategoryName);
            return View(await categories.ToListAsync());
        }

        // GET: Categories/Details/5
        // Retrieves a single category using LINQ
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // GET: Categories/Create
        // Loads the create form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // Saves new category with auto-set date and active status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            // Remove validation for auto-set fields
            ModelState.Remove("IsActive");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("Products");

            if (ModelState.IsValid)
            {
                // Auto-set these fields - not entered by user
                category.CreatedDate = DateTime.Now;
                category.IsActive = true;

                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        // Loads the edit form for an existing category
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        // Updates existing category and preserves CreatedDate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryId)
                return NotFound();

            // Remove validation for fields not in the form
            ModelState.Remove("Products");
            ModelState.Remove("CreatedDate");

            if (ModelState.IsValid)
            {
                try
                {
                    // Preserve original CreatedDate
                    var original = await _context.Categories
                                                 .AsNoTracking()
                                                 .FirstOrDefaultAsync(c => c.CategoryId == id);

                    category.CreatedDate = original.CreatedDate;
                    category.IsActive = original.IsActive;

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Category updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        // Shows delete confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        // Deletes category and shows success message
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper method - checks if a category exists by ID
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}