using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpressVoitures.Data;
using ExpressVoitures.Models;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Services;

namespace ExpressVoitures.Controllers
{
    public class FinitionsController : Controller
    {
        private readonly IFinitionService _finitionService;
        private readonly IModeleService _modeleService;
        private readonly IVoitureService _voitureService;

        public FinitionsController(IFinitionService finitionService, IModeleService modeleService, IVoitureService voitureService)
        {
            _finitionService = finitionService;
            _modeleService = modeleService;
            _voitureService = voitureService;
        }

        // GET: Finitions
        public async Task<IActionResult> Index()
        {
            var finitions = await _finitionService.ObtenirToutesAsync();
            return View(finitions);
        }

        // GET: Finitions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finition = await _finitionService.ObtenirParIdAsync(id.Value);
            if (finition == null)
            {
                return NotFound();
            }

            return View(finition);
        }

        // GET: Finitions/Create
        public async Task<IActionResult> Create()
        {
            var modeles = await _modeleService.ObtenirTousAsync();
            ViewData["IdModele"] = new SelectList(modeles, "Id", "Nom");
            return View();
        }

        // POST: Finitions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,IdModele")] Finition finition)
        {
            bool finitionExiste;
            finitionExiste = await _finitionService.ExistePourModeleAsync(finition.Nom, finition.IdModele);
            if (finitionExiste)
            {
                ModelState.AddModelError("", "Une finition portant ce nom existe déjà pour ce modèle.");
            }

            if (ModelState.IsValid)
            {
                await _finitionService.CreerAsync(finition);
                return RedirectToAction(nameof(Index));
            }
            var modeles = await _modeleService.ObtenirTousAsync();
            ViewData["IdModele"] = new SelectList(modeles, "Id", "Nom", finition.IdModele);
            return View(finition);
        }

        // GET: Finitions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finition = await _finitionService.ObtenirParIdAsync(id.Value);
            if (finition == null)
            {
                return NotFound();
            }
            var modeles = await _modeleService.ObtenirTousAsync();
            ViewData["IdModele"] = new SelectList(modeles, "Id", "Nom", finition.IdModele);
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

            bool finitionExiste;
            finitionExiste = await _finitionService.ExistePourModeleAsync(finition.Nom, finition.IdModele, finition.Id);
            if (finitionExiste)
            {
                ModelState.AddModelError("", "Une finition portant ce nom existe déjà pour ce modèle.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _finitionService.MettreAJourAsync(finition);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await FinitionExists(finition.Id))
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
            var modeles = await _modeleService.ObtenirTousAsync();
            ViewData["IdModele"] = new SelectList(modeles, "Id", "Nom");
            return View(finition);
        }

        // GET: Finitions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finition = await _finitionService.ObtenirParIdAsync(id.Value);
            if (finition == null)
            {
                return NotFound();
            }

            // Vérifier si une voiture utilise cette finition
            bool finitionUtilisee = await _voitureService.FinitionUtiliseeAsync(id.Value);

            if (finitionUtilisee)
            {
                return View("DeleteBlocked", finition);
            }

            return View(finition);
        }

        // POST: Finitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _finitionService.SupprimerAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> FinitionExists(int id)
        {
            return await _finitionService.ExisteAsync(id);
        }
    }
}
