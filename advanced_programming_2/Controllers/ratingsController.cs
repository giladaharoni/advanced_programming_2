using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using advanced_programming_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace advanced_programming_2.Controllers
{
    public class ratingsController : Controller
    {
        private readonly advanced_programming_2Context _context;

        public ratingsController(advanced_programming_2Context context)
        {
            _context = context;
        }

        // GET: ratings
        public async Task<IActionResult> Index()
        {
           
   
              return _context.rating != null ? View(await _context.rating.ToListAsync()) :
                          Problem("Entity set 'advanced_programming_2Context.rating'  is null.");
        }

        // GET: rating/search

        public async Task<IActionResult> Search()
        {


            return View(await _context.rating.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            var q = from rating in _context.rating
                    where rating.feedback.Contains(query) || rating.name.Contains(query)
                    select rating;

            return View(await q.ToListAsync());
        }

        public async Task<IActionResult> Search2(string query)
        {
            var q = _context.rating.Where(rating => rating.name.Contains(query) || rating.feedback.Contains(query));
            if (query == null)
            {
                q = _context.rating;
            }
           

            return PartialView(await q.ToListAsync());
        }

        // GET: ratings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.rating == null)
            {
                return NotFound();
            }

            var rating = await _context.rating
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // GET: ratings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ratings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,feedback,rate,date")] rating rating)
        {
            rating.date = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(rating);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rating);
        }

        // GET: ratings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.rating == null)
            {
                return NotFound();
            }

            var rating = await _context.rating.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }
            return View(rating);
        }

        // POST: ratings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,feedback,rate,date")] rating rating)
        {
            if (id != rating.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ratingExists(rating.Id))
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
            return View(rating);
        }

        // GET: ratings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.rating == null)
            {
                return NotFound();
            }

            var rating = await _context.rating
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // POST: ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.rating == null)
            {
                return Problem("Entity set 'advanced_programming_2Context.rating'  is null.");
            }
            var rating = await _context.rating.FindAsync(id);
            if (rating != null)
            {
                _context.rating.Remove(rating);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ratingExists(int id)
        {
          return (_context.rating?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
