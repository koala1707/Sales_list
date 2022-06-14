using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReviewWT.Data;
using ReviewWT.Models;

namespace ReviewWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsWebAPIController : ControllerBase
    {
        private readonly AmazonOrdersContext _context;

        public ItemsWebAPIController(AmazonOrdersContext context)
        {
            _context = context;
        }

        // GET: api/ItemsWebAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems(string searchText, int? year)
        {
            var query = _context.ItemsInOrders
                .Select(i => i);
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(i => i.Item.ItemName.Contains(searchText));
            }
            if (year.HasValue)
            {
                query = query.Where(i => i.OrderNumberNavigation.OrderDate.Year==year);
            }
            return await _context.Items.ToListAsync();
        }

        // GET: api/ItemsWebAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        // PUT: api/ItemsWebAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, Item item)
        {
            if (id != item.ItemId)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ItemsWebAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItem", new { id = item.ItemId }, item);
        }

        // DELETE: api/ItemsWebAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.ItemId == id);
        }
    }
}
