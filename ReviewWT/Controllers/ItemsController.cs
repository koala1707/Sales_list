using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReviewWT.Data;
using ReviewWT.Models;

namespace ReviewWT.Controllers
{
    public class ItemsController : Controller
    {
        private readonly AmazonOrdersContext _context;

        public ItemsController(AmazonOrdersContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index(string searchText, int? year)
        {
            //3-5-1: flag to show search record count
            ViewBag.Count = !string.IsNullOrWhiteSpace(searchText) || year.HasValue;
            //3-5-3: Retaining the word a user input in searching space
            ViewBag.SearchText = searchText;
            
            #region yearQuery
            var year_dropdown = await _context.CustomerOrders
                .Select(p => p.OrderDate.Year)
                .Distinct()
                .OrderByDescending(p=>p)
                .ToListAsync();
            
            ViewBag.Year = new SelectList(year_dropdown);
            #endregion


            #region itemQuery
            //3-2-1 Query that selects all items in order of itemName
            var query = _context.ItemsInOrders
                .Select(io => io);

            query = query.Include(i => i.Item).Include(c => c.OrderNumberNavigation);
            //3-2-2 Optional WHERE clause if searchText is not empty
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p => p.Item.ItemName == searchText);
            }
            //3-2-3 Include the year (to display year)
            if (year.HasValue)
            {
                query = query.Where(p => p.OrderNumberNavigation.OrderDate.Year == year);
            }
            #endregion
            //3-2-4 Execute the query with ToListAsync() and pass data to View
            //return View(await query.ToListAsync());
            return View(query.ToListAsync());

            //Original
            //var amazonOrdersContext = _context.Items.Include(i => i.Category);
            //return View(await amazonOrdersContext.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.ItemCategories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,ItemName,ItemDescription,ItemCost,ItemImage,CategoryId")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.ItemCategories, "CategoryId", "CategoryName", item.CategoryId);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.ItemCategories, "CategoryId", "CategoryName", item.CategoryId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,ItemName,ItemDescription,ItemCost,ItemImage,CategoryId")] Item item)
        {
            if (id != item.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ItemId))
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
            ViewData["CategoryId"] = new SelectList(_context.ItemCategories, "CategoryId", "CategoryName", item.CategoryId);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.FindAsync(id);
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.ItemId == id);
        }
    }
}
