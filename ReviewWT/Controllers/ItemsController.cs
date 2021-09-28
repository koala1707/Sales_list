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

            #region yearQuery
            var year_dropdown = _context.CustomerOrders
                .Select(p => p.OrderDate.Year)
                .Distinct()
                .OrderByDescending(p => p)
                .ToList();

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
        public async Task<IActionResult> Details(int id, int year)
        {
            ItemSearchViewModel detailsView = new ItemSearchViewModel();
            detailsView.id = id;
            detailsView.year = year;

            #region customerDetails
            var detailQuery = _context.ItemsInOrders
                .Where(iio => iio.ItemId == id)
                //.Where(iio => iio.OrderNumberNavigation.OrderDate.Year == purchasedYear)
                .Select(iio => iio);

            //Console.WriteLine("YEAR: "+purchasedYear.HasValue);
            //if (year.HasValue)
            //{
            //    detailQuery = detailQuery.Where(iio => iio.OrderNumberNavigation.OrderDate.Year == year);
            //}

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
                .Select(i => new PurchasedItemDetails
                {
                    purchasedItemName = i.ItemName,
                    purchasedItemImage = i.ItemImage,
                    purchasedItemCost = i.ItemCost,
                    purchasedItemDescription = i.ItemDescription
                });
            #endregion

            detailsView.customers = await details_summary.ToListAsync();
            detailsView.itemDetails = await itemDetails.ToListAsync();

            //return View(item);
            return View(detailsView);
        }





        public async Task<IActionResult> Report(int? year)
        {
            ItemSearchViewModel newView = new ItemSearchViewModel();
            newView.year = year;

            #region yearQuery
            var year_dropdown = _context.CustomerOrders
                .Select(p => p.OrderDate.Year)
                .Distinct()
                .OrderByDescending(p => p)
                .ToList();

            newView.years = new SelectList(year_dropdown);
            #endregion

            return View(newView);
        }


        [Produces("application/json")]
        public IActionResult AnnualSalesReportData(int? year)
        {
            if (year.HasValue)
            {
                var orderSummary = _context.ItemsInOrders
                   .Include(p => p.OrderNumberNavigation)
                   .GroupBy(p => new
                   {
                       p.OrderNumberNavigation.OrderDate.Year,
                       p.Item.ItemName
                   })
                   .Select(p => new
                   {
                       year = p.Key.Year,
                       itemName = p.Key.ItemName,
                       totalItems = p.Sum(x => x.NumberOf),
                       totalSales = p.Sum(x => x.TotalItemCost)
                   })
                   .Where(p => p.year == year)
                   .OrderByDescending(p => p.itemName)
                   //.OrderBy(p => p.year)
                   //.ThenBy(p => p.itemName)
                   .ToList();

                var summary = orderSummary
                    .Select(p => new
                    {
                        p.year,
                        p.itemName,
                        //monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(p.monthNo),
                        p.totalItems,
                        p.totalSales
                    });
                    //.OrderBy(p => p.itemName);
                return Json(summary);
            }
            else
            {
                return BadRequest();
            }
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
