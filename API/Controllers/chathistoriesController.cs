using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using advanced_programming_2.Models;
using Repository;

namespace advanced_programming_2.Controllers
{
    public class chathistoriesController : Controller
    {
        private readonly advanced_programming_2Context _context;

        public chathistoriesController(advanced_programming_2Context context)
        {
            _context = context;
        }

        // GET: chathistories
        public async Task<IActionResult> Index()
        {
              return _context.chathistory != null ? 
                          View(await _context.chathistory.ToListAsync()) :
                          Problem("Entity set 'advanced_programming_2Context.chathistory'  is null.");
        }

        // GET: chathistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.chathistory == null)
            {
                return NotFound();
            }

            var chathistory = await _context.chathistory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chathistory == null)
            {
                return NotFound();
            }

            return View(chathistory);
        }

        // GET: chathistories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: chathistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] chathistory chathistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chathistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chathistory);
        }

        // GET: chathistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.chathistory == null)
            {
                return NotFound();
            }

            var chathistory = await _context.chathistory.FindAsync(id);
            if (chathistory == null)
            {
                return NotFound();
            }
            return View(chathistory);
        }

        // POST: chathistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] chathistory chathistory)
        {
            if (id != chathistory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chathistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!chathistoryExists(chathistory.Id))
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
            return View(chathistory);
        }

        // GET: chathistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.chathistory == null)
            {
                return NotFound();
            }

            var chathistory = await _context.chathistory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chathistory == null)
            {
                return NotFound();
            }

            return View(chathistory);
        }

        // POST: chathistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.chathistory == null)
            {
                return Problem("Entity set 'advanced_programming_2Context.chathistory'  is null.");
            }
            var chathistory = await _context.chathistory.FindAsync(id);
            if (chathistory != null)
            {
                _context.chathistory.Remove(chathistory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool chathistoryExists(int id)
        {
          return (_context.chathistory?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
