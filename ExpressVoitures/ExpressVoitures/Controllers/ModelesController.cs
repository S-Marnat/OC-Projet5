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
    public class ModelesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModelesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Modeles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Modeles.Include(m => m.Marque);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Modeles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modele = await _context.Modeles
                .Include(m => m.Marque)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modele == null)
            {
                return NotFound();
            }

            return View(modele);
        }

        // GET: Modeles/Create
        public IActionResult Create()
        {
            ViewData["IdMarque"] = new SelectList(_context.Marques, "Id", "Nom");
            return View();
        }

        // POST: Modeles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,IdMarque")] Modele modele)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modele);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdMarque"] = new SelectList(_context.Marques, "Id", "Nom", modele.IdMarque);
            return View(modele);
        }

        // GET: Modeles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modele = await _context.Modeles.FindAsync(id);
            if (modele == null)
            {
                return NotFound();
            }
            ViewData["IdMarque"] = new SelectList(_context.Marques, "Id", "Nom", modele.IdMarque);
            return View(modele);
        }

        // POST: Modeles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,IdMarque")] Modele modele)
        {
            if (id != modele.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modele);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModeleExists(modele.Id))
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
            ViewData["IdMarque"] = new SelectList(_context.Marques, "Id", "Nom", modele.IdMarque);
            return View(modele);
        }

        // GET: Modeles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modele = await _context.Modeles
                .Include(m => m.Marque)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modele == null)
            {
                return NotFound();
            }

            return View(modele);
        }

        // POST: Modeles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modele = await _context.Modeles.FindAsync(id);
            if (modele != null)
            {
                _context.Modeles.Remove(modele);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModeleExists(int id)
        {
            return _context.Modeles.Any(e => e.Id == id);
        }
    }
}
