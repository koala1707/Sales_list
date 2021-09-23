using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReviewWT.Data;
using ReviewWT.Models;
using ReviewWT.ViewModels;

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
            ItemSearchViewModel newView = new ItemSearchViewModel();
            newView.year = year;
            newView.searchText = searchText;
            //3-5-1: flag to show search record count
            //ViewBag.Count = !string.IsNullOrWhiteSpace(searchText) || year.HasValue;
            //3-5-3: Retaining the word a user input in searching space
            //ViewBag.SearchText = searchText;

            //.Select(p => new {p.OrderNumber, p.OrderDate.Year})

            #region yearQuery
            var year_dropdown = _context.CustomerOrders
                .Select(p => p.OrderDate.Year)
                .Distinct()
                .OrderByDescending(p => p)
                .ToList();

            //ViewBag.Year = new SelectList(year_dropdown);
            //newView has to have Model.customerOrders
            newView.years = new SelectList(year_dropdown, "OrderDate");
            #endregion

            #region itemQuery
            var query = _context.ItemsInOrders
                .Where(iio => iio.Item.ItemName.Contains(searchText))
                .Select(iio => iio);
            
            if (year.HasValue)
            {
                query = query.Where(iio => iio.OrderNumberNavigation.OrderDate.Year == year);
            }
            var display_customer_order = query
                .GroupBy(iio => iio.ItemId)
                .OrderBy(iio => iio.Key)
                .Select(iio => new ItemDetails
                {
                    itemId = iio.Key,
                    unitsSold = iio.Sum(iio => iio.NumberOf),
                    customerEffect = iio.Select(co => co.OrderNumberNavigation.CustomerId).Distinct().Count()
                });

            var customer_order_summary = display_customer_order
                .Select(i => new ItemDetails
                {
                    itemId=i.itemId,
                    customerEffect=i.customerEffect,
                    unitsSold=i.unitsSold,
                    item = _context.Items
                        .Include(c => c.Category)
                        .Where(c => c.ItemId == i.itemId).FirstOrDefault()
                });
            #endregion

            //newView has to have Model.item
            //and the Model.item has to have propaties
            //which are included below e.g. ItemsInOrder
            //Otherwise, newView.items does not work!
            newView.items = await customer_order_summary.ToListAsync();

            newView.itemList = _context.Items.Select(i => i.ItemName).ToList();

            return View(newView);
            
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int id, int? purchasedYear)// id == itemId
        {
            CustomerSearchViewModel detailsView = new CustomerSearchViewModel();
            detailsView.id = id;
            detailsView.purchasedYear = purchasedYear;

            //#region detailQuery
            //var detailQuery = _context.CustomerOrders
            //    .Include(i => i.ItemsInOrders)
            //    .Where(i => i.ItemsInOrders.)
            //    .Select(co => co);

            //#endregion

            #region detailQuery
            var detailQuery = _context.ItemsInOrders
                .Where(iio => iio.ItemId == id)
                .Select(iio => iio);

            Console.WriteLine("YEAR: "+purchasedYear.HasValue);
            if (purchasedYear.HasValue)
            {
                detailQuery = detailQuery.Where(iio => iio.OrderNumberNavigation.OrderDate.Year == purchasedYear);
            }
            var get_details = detailQuery
                .GroupBy(iio => iio.OrderNumberNavigation.CustomerId)
                .OrderBy(iio => iio.Key)
                .Select(cd => new CustomerDetails
                {
                    customerId = cd.Key,
                    totalCost = cd.Sum(cd => cd.TotalItemCost),
                    customerUnits = cd.Sum(cd => cd.NumberOf),
                    
                });

            var details_summary = get_details
                .Select(p => new CustomerDetails
                {
                    customerId = p.customerId,
                    customerUnits = p.customerUnits,
                    totalCost = p.totalCost,
                    customerDetails = _context.CustomerOrders
                        .Include(c => c.Customer)
                        .ThenInclude(a => a.Address)
                        .Where(c => c.CustomerId == p.customerId).FirstOrDefault()
                }) ;
            #endregion

            #region itemDetails
            var itemDetails = _context.Items
                .Where(i => i.ItemId == id)
                .Select(i => new CustomerSearchViewModel
                {
                    purchasedItemName = i.ItemName,
                    purchasedItemImage = i.ItemImage
                });

            var display = itemDetails
                .Select(id => new CustomerSearchViewModel
                {
                    purchasedItemImage = id.purchasedItemImage,
                    purchasedItemName = id.purchasedItemName
                }).ToList();


            #endregion

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var item = await _context.Items
            //    .Include(i => i.Category)
            //    .FirstOrDefaultAsync(m => m.ItemId == id);
            //if (item == null)
            //{
            //    return NotFound();
            //}

            detailsView.customers = await details_summary.ToListAsync();
            //detailsView.display = await itemDetails.ToListAsync();

            //return View(item);
            return View(detailsView);
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
