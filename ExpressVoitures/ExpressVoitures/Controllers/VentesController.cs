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
    public class VentesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VentesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ventes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ventes.Include(v => v.Voiture);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Ventes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vente = await _context.Ventes
                .Include(v => v.Voiture)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vente == null)
            {
                return NotFound();
            }

            return View(vente);
        }

        // GET: Ventes/Create
        public IActionResult Create()
        {
            ViewData["IdVoiture"] = new SelectList(_context.Voitures, "Id", "CodeVin");
            return View();
        }

        // POST: Ventes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,IdVoiture")] Vente vente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdVoiture"] = new SelectList(_context.Voitures, "Id", "CodeVin", vente.IdVoiture);
            return View(vente);
        }

        // GET: Ventes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vente = await _context.Ventes.FindAsync(id);
            if (vente == null)
            {
                return NotFound();
            }
            ViewData["IdVoiture"] = new SelectList(_context.Voitures, "Id", "CodeVin", vente.IdVoiture);
            return View(vente);
        }

        // POST: Ventes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,IdVoiture")] Vente vente)
        {
            if (id != vente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenteExists(vente.Id))
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
            ViewData["IdVoiture"] = new SelectList(_context.Voitures, "Id", "CodeVin", vente.IdVoiture);
            return View(vente);
        }

        // GET: Ventes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vente = await _context.Ventes
                .Include(v => v.Voiture)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vente == null)
            {
                return NotFound();
            }

            return View(vente);
        }

        // POST: Ventes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vente = await _context.Ventes.FindAsync(id);
            if (vente != null)
            {
                _context.Ventes.Remove(vente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VenteExists(int id)
        {
            return _context.Ventes.Any(e => e.Id == id);
        }
    }
}
