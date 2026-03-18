using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpressVoitures.Data;
using ExpressVoitures.Models;

namespace ExpressVoitures.Controllers
{
    public class FinitionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FinitionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Finitions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Finitions.Include(f => f.Modele);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Finitions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finition = await _context.Finitions
                .Include(f => f.Modele)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (finition == null)
            {
                return NotFound();
            }

            return View(finition);
        }

        // GET: Finitions/Create
        public IActionResult Create()
        {
            ViewData["IdModele"] = new SelectList(_context.Modeles, "Id", "Nom");
            return View();
        }

        // POST: Finitions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,IdModele")] Finition finition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(finition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdModele"] = new SelectList(_context.Modeles, "Id", "Nom", finition.IdModele);
            return View(finition);
        }

        // GET: Finitions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finition = await _context.Finitions.FindAsync(id);
            if (finition == null)
            {
                return NotFound();
            }
            ViewData["IdModele"] = new SelectList(_context.Modeles, "Id", "Nom", finition.IdModele);
            return View(finition);
        }

        // POST: Finitions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,IdModele")] Finition finition)
        {
            if (id != finition.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(finition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinitionExists(finition.Id))
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
            ViewData["IdModele"] = new SelectList(_context.Modeles, "Id", "Nom", finition.IdModele);
            return View(finition);
        }

        // GET: Finitions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finition = await _context.Finitions
                .Include(f => f.Modele)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (finition == null)
            {
                return NotFound();
            }

            return View(finition);
        }

        // POST: Finitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var finition = await _context.Finitions.FindAsync(id);
            if (finition != null)
            {
                _context.Finitions.Remove(finition);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FinitionExists(int id)
        {
            return _context.Finitions.Any(e => e.Id == id);
        }
    }
}
