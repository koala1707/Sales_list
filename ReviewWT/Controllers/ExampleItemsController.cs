using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReviewWT.Data;
using ReviewWT.Models.CodeFirst;

namespace ReviewWT.Controllers
{
    public class ExampleItemsController : Controller
    {
        private readonly ReviewWTContext _context;

        public ExampleItemsController(ReviewWTContext context)
        {
            _context = context;
        }

        // GET: ExampleItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.ExampleItem.ToListAsync());
        }

        // GET: ExampleItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exampleItem = await _context.ExampleItem
                .FirstOrDefaultAsync(m => m.itemID == id);
            if (exampleItem == null)
            {
                return NotFound();
            }

            return View(exampleItem);
        }

        // GET: ExampleItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ExampleItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("itemID,itemName,itemPrice")] ExampleItem exampleItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exampleItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exampleItem);
        }

        // GET: ExampleItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exampleItem = await _context.ExampleItem.FindAsync(id);
            if (exampleItem == null)
            {
                return NotFound();
            }
            return View(exampleItem);
        }

        // POST: ExampleItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("itemID,itemName,itemPrice")] ExampleItem exampleItem)
        {
            if (id != exampleItem.itemID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exampleItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExampleItemExists(exampleItem.itemID))
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
            return View(exampleItem);
        }

        // GET: ExampleItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exampleItem = await _context.ExampleItem
                .FirstOrDefaultAsync(m => m.itemID == id);
            if (exampleItem == null)
            {
                return NotFound();
            }

            return View(exampleItem);
        }

        // POST: ExampleItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exampleItem = await _context.ExampleItem.FindAsync(id);
            _context.ExampleItem.Remove(exampleItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExampleItemExists(int id)
        {
            return _context.ExampleItem.Any(e => e.itemID == id);
        }
    }
}
