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
    public class VoituresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VoituresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Voitures
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Voitures.Include(v => v.Finition).Include(v => v.Marque).Include(v => v.Modele);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Voitures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voiture = await _context.Voitures
                .Include(v => v.Finition)
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (voiture == null)
            {
                return NotFound();
            }

            return View(voiture);
        }

        // GET: Voitures/Create
        public IActionResult Create()
        {
            ViewData["IdFinition"] = new SelectList(_context.Finitions, "Id", "Nom");
            ViewData["IdMarque"] = new SelectList(_context.Marques, "Id", "Nom");
            ViewData["IdModele"] = new SelectList(_context.Modeles, "Id", "Nom");
            return View();
        }

        // POST: Voitures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CodeVin,Annee,Image,Description,DateAchat,PrixAchat,VoitureReparee,DateMiseEnVente,PrixMiseEnVente,AnnoncePubliee,VoitureVendue,IdMarque,IdModele,IdFinition")] Voiture voiture)
        {
            if (ModelState.IsValid)
            {
                _context.Add(voiture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdFinition"] = new SelectList(_context.Finitions, "Id", "Nom", voiture.IdFinition);
            ViewData["IdMarque"] = new SelectList(_context.Marques, "Id", "Nom", voiture.IdMarque);
            ViewData["IdModele"] = new SelectList(_context.Modeles, "Id", "Nom", voiture.IdModele);
            return View(voiture);
        }

        // GET: Voitures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voiture = await _context.Voitures.FindAsync(id);
            if (voiture == null)
            {
                return NotFound();
            }
            ViewData["IdFinition"] = new SelectList(_context.Finitions, "Id", "Nom", voiture.IdFinition);
            ViewData["IdMarque"] = new SelectList(_context.Marques, "Id", "Nom", voiture.IdMarque);
            ViewData["IdModele"] = new SelectList(_context.Modeles, "Id", "Nom", voiture.IdModele);
            return View(voiture);
        }

        // POST: Voitures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodeVin,Annee,Image,Description,DateAchat,PrixAchat,VoitureReparee,DateMiseEnVente,PrixMiseEnVente,AnnoncePubliee,VoitureVendue,IdMarque,IdModele,IdFinition")] Voiture voiture)
        {
            if (id != voiture.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voiture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoitureExists(voiture.Id))
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
            ViewData["IdFinition"] = new SelectList(_context.Finitions, "Id", "Nom", voiture.IdFinition);
            ViewData["IdMarque"] = new SelectList(_context.Marques, "Id", "Nom", voiture.IdMarque);
            ViewData["IdModele"] = new SelectList(_context.Modeles, "Id", "Nom", voiture.IdModele);
            return View(voiture);
        }

        // GET: Voitures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voiture = await _context.Voitures
                .Include(v => v.Finition)
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (voiture == null)
            {
                return NotFound();
            }

            return View(voiture);
        }

        // POST: Voitures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voiture = await _context.Voitures.FindAsync(id);
            if (voiture != null)
            {
                _context.Voitures.Remove(voiture);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoitureExists(int id)
        {
            return _context.Voitures.Any(e => e.Id == id);
        }
    }
}
