using ExpressVoitures.Data;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;

namespace ExpressVoitures.Controllers
{
    [Authorize]
    public class ModelesController : Controller
    {
        private readonly IModeleService _modeleService;
        private readonly IMarqueService _marqueService;
        private readonly IVoitureService _voitureService;

        public ModelesController(IModeleService modeleService, IMarqueService marqueService, IVoitureService voitureService)
        {
            _modeleService = modeleService;
            _marqueService = marqueService;
            _voitureService = voitureService;
        }

        // GET: Modeles
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Index()
        {
            var modeles = await _modeleService.ObtenirTousAsync();
            return View(modeles);
        }

        // GET: Modeles/Details/5
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modele = await _modeleService.ObtenirParIdAsync(id.Value);
            if (modele == null)
            {
                return NotFound();
            }

            return View(modele);
        }

        // GET: Modeles/Create
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Create()
        {
            var marques = await _marqueService.ObtenirToutesAsync();
            ViewData["IdMarque"] = new SelectList(marques, "Id", "Nom");
            return View();
        }

        // POST: Modeles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Create([Bind("Id,Nom,IdMarque")] Modele modele)
        {
            bool modeleExiste;
            modeleExiste = await _modeleService.ExistePourMarqueAsync(modele.Nom, modele.IdMarque);
            if (modeleExiste)
            {
                ModelState.AddModelError("", "Un modèle portant ce nom existe déjà pour cette marque.");
            }

            if (ModelState.IsValid)
            {
                await _modeleService.CreerAsync(modele);
                return RedirectToAction(nameof(Index));
            }
            var marques = await _marqueService.ObtenirToutesAsync();
            ViewData["IdMarque"] = new SelectList(marques, "Id", "Nom", modele.IdMarque);
            return View(modele);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepuisVoiture(Modele modele)
        {
            if (!ModelState.IsValid)
                return View("_CreatePartial", modele);

            await _modeleService.CreerAsync(modele);

            // Redirection en sélectionnant automatiquement le nouveau modèle
            return RedirectToAction("CreateSimple", "Voitures", new { idModeleCree = modele.Id });
        }

        // GET: Modeles/Edit/5
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modele = await _modeleService.ObtenirParIdAsync(id.Value);
            if (modele == null)
            {
                return NotFound();
            }
            var marques = await _marqueService.ObtenirToutesAsync();
            ViewData["IdMarque"] = new SelectList(marques, "Id", "Nom", modele.IdMarque);
            return View(modele);
        }

        // POST: Modeles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,IdMarque")] Modele modele)
        {
            if (id != modele.Id)
            {
                return NotFound();
            }

            bool modeleExiste;
            modeleExiste = await _modeleService.ExistePourMarqueAsync(modele.Nom, modele.IdMarque, modele.Id);
            if (modeleExiste)
            {
                ModelState.AddModelError("", "Un modèle portant ce nom existe déjà pour cette marque.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _modeleService.MettreAJourAsync(modele);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ModeleExists(modele.Id))
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
            var marques = await _marqueService.ObtenirToutesAsync();
            ViewData["IdMarque"] = new SelectList(marques, "Id", "Nom", modele.IdMarque);
            return View(modele);
        }

        // GET: Modeles/Delete/5
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modele = await _modeleService.ObtenirParIdAsync(id.Value);
            if (modele == null)
            {
                return NotFound();
            }

            // Vérifier si une voiture utilise ce modèle
            bool modeleUtilise = await _voitureService.ModeleUtiliseAsync(id.Value);

            if (modeleUtilise)
            {
                return View("DeleteBlocked", modele);
            }

            return View(modele);
        }

        // POST: Modeles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _modeleService.SupprimerAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ModeleExists(int id)
        {
            return await _modeleService.ExisteAsync(id);
        }
    }
}
