using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    // OrdersController handles all CRUD operations for Orders
    public class OrdersController : Controller
    {
        // EF Core database context
        private readonly StoreManagementDbContext _context;

        // Constructor - injects the database context
        public OrdersController(StoreManagementDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        // Uses LINQ Include to load Customer, OrderByDescending to sort
        public async Task<IActionResult> Index()
        {
            var orders = _context.Orders
                                 .Include(o => o.Customer)
                                 .OrderByDescending(o => o.OrderDate);
            return View(await orders.ToListAsync());
        }

        // GET: Orders/Details/5
        // Retrieves single order with customer and order items
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: Orders/Create
        // Loads create form with customer dropdown
        public IActionResult Create()
        {
            // Populate customer dropdown using LINQ OrderBy
            ViewData["CustomerId"] = new SelectList(
                _context.Customers.OrderBy(c => c.LastName),
                "CustomerId", "FirstName");
            return View();
        }

        // POST: Orders/Create
        // Saves new order with auto-set date
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            // Remove validation for navigation properties
            ModelState.Remove("Customer");
            ModelState.Remove("OrderItems");
            ModelState.Remove("OrderDate");

            if (ModelState.IsValid)
            {
                // Auto-set order date to current date and time
                order.OrderDate = DateTime.Now;
                _context.Add(order);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Order added successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(
                _context.Customers.OrderBy(c => c.LastName),
                "CustomerId", "FirstName", order.CustomerId);
            return View(order);
        }

        // GET: Orders/Edit/5
        // Loads edit form for existing order
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            ViewData["CustomerId"] = new SelectList(
                _context.Customers.OrderBy(c => c.LastName),
                "CustomerId", "FirstName", order.CustomerId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // Updates existing order and preserves original OrderDate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.OrderId)
                return NotFound();

            // Remove validation for fields not in the form
            ModelState.Remove("Customer");
            ModelState.Remove("OrderItems");
            ModelState.Remove("OrderDate");

            if (ModelState.IsValid)
            {
                try
                {
                    // Get original order to preserve the OrderDate
                    var original = await _context.Orders
                                                 .AsNoTracking()
                                                 .FirstOrDefaultAsync(o => o.OrderId == id);

                    // Preserve original date - not editable by user
                    order.OrderDate = original.OrderDate;

                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Order updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["CustomerId"] = new SelectList(
                _context.Customers.OrderBy(c => c.LastName),
                "CustomerId", "FirstName", order.CustomerId);
            return View(order);
        }

        // GET: Orders/Delete/5
        // Shows delete confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: Orders/Delete/5
        // Deletes order and shows success message
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Order deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper method - checks if an order exists by ID
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}