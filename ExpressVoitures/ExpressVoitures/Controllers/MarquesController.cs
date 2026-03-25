using ExpressVoitures.Data;
using ExpressVoitures.Models;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ExpressVoitures.Controllers
{
    [Authorize]
    public class MarquesController : Controller
    {
        private readonly IMarqueService _marqueService;
        private readonly IVoitureService _voitureService;

        public MarquesController(IMarqueService marqueService, IVoitureService voitureService)
        {
            _marqueService = marqueService;
            _voitureService = voitureService;
        }

        // GET: Marques
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Index()
        {
            var marques = await _marqueService.ObtenirToutesAsync();
            return View(marques);
        }

        // GET: Marques/Details/5
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marque = await _marqueService.ObtenirParIdAsync(id.Value);
            if (marque == null)
            {
                return NotFound();
            }

            return View(marque);
        }

        // GET: Marques/Create
        [Authorize(Roles = "Administrateur")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Marques/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Create([Bind("Id,Nom")] Marque marque)
        {
            bool marqueExiste;
            marqueExiste = await _marqueService.NomExisteAsync(marque.Nom);
            if (marqueExiste)
            {
                ModelState.AddModelError("", "Une marque portant ce nom existe déjà.");
            }

            if (ModelState.IsValid)
            {
                await _marqueService.CreerAsync(marque);
                return RedirectToAction(nameof(Index));
            }
            return View(marque);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepuisVoiture(Marque marque)
        {
            if (!ModelState.IsValid)
                return View("_CreatePartial", marque);

            await _marqueService.CreerAsync(marque);

            // Redirection en sélectionnant automatiquement la nouvelle marque
            return RedirectToAction("CreateSimple", "Voitures", new { idMarqueCree = marque.Id });
        }

        // GET: Marques/Edit/5
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marque = await _marqueService.ObtenirParIdAsync(id.Value);
            if (marque == null)
            {
                return NotFound();
            }
            return View(marque);
        }

        // POST: Marques/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom")] Marque marque)
        {
            if (id != marque.Id)
            {
                return NotFound();
            }

            bool marqueExiste;
            marqueExiste = await _marqueService.NomExisteAsync(marque.Nom, marque.Id);
            if (marqueExiste)
            {
                ModelState.AddModelError("", "Une marque portant ce nom existe déjà.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _marqueService.MettreAJourAsync(marque);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await MarqueExists(marque.Id))
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
            return View(marque);
        }

        // GET: Marques/Delete/5
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marque = await _marqueService.ObtenirParIdAsync(id.Value);
            if (marque == null)
            {
                return NotFound();
            }

            // Vérifier si une voiture utilise cette marque
            bool marqueUtilisee = await _voitureService.MarqueUtiliseeAsync(id.Value);

            if (marqueUtilisee)
            {
                return View("DeleteBlocked", marque);
            }

            return View(marque);
        }

        // POST: Marques/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _marqueService.SupprimerAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MarqueExists(int id)
        {
            return await _marqueService.ExisteAsync(id);
        }
    }
}
