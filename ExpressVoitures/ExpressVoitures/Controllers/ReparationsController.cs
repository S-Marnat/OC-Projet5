using ExpressVoitures.Data;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressVoitures.Controllers
{
    [Authorize(Roles = "Administrateur")]
    public class ReparationsController : Controller
    {
        private readonly IReparationService _reparationService;
        private readonly IVoitureService _voitureService;

        public ReparationsController(IReparationService reparationService, IVoitureService voitureService)
        {
            _reparationService = reparationService;
            _voitureService = voitureService;
        }

        // GET: Reparations
        public async Task<IActionResult> Index()
        {
            var reparations = await _reparationService.ObtenirToutesAsync();
            return View(reparations);
        }

        // GET: Reparations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var reparation = await _reparationService.ObtenirParIdAsync(id.Value);
            if (reparation == null)
                return NotFound();

            return View(reparation);
        }

        // GET: Reparations/Create
        public async Task<IActionResult> Create()
        {
            var voitures = await _voitureService.ObtenirToutesAsync();
            ViewData["IdVoiture"] = new SelectList(voitures, "Id", "NomComplet");
            return View();
        }

        // POST: Reparations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReparationEffectuee,Cout,CoutString,IdVoiture")] Reparation reparation)
        {
            if (!double.TryParse(reparation.CoutString.Replace(",", "."),
                     NumberStyles.Any,
                     CultureInfo.InvariantCulture,
                     out double cout))
            {
                ModelState.AddModelError("CoutString", "Le prix doit être un nombre positif avec au maximum 2 décimales");
            }
            else
            {
                reparation.Cout = cout;
            }

            if (ModelState.IsValid)
            {
                await _reparationService.CreerAsync(reparation);
                return RedirectToAction(nameof(Index));
            }

            var voitures = await _voitureService.ObtenirToutesAsync();
            ViewData["IdVoiture"] = new SelectList(voitures, "Id", "NomComplet", reparation.IdVoiture);
            return View(reparation);
        }

        // GET: Reparations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var reparation = await _reparationService.ObtenirParIdAsync(id.Value);
            if (reparation == null)
                return NotFound();

            reparation.CoutString = reparation.Cout.ToString("0.##", new CultureInfo("fr-FR"));

            var voitures = await _voitureService.ObtenirToutesAsync();
            ViewData["IdVoiture"] = new SelectList(voitures, "Id", "NomComplet", reparation.IdVoiture);
            return View(reparation);
        }

        // POST: Reparations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReparationEffectuee,Cout,CoutString,IdVoiture")] Reparation reparation)
        {
            if (id != reparation.Id)
                return NotFound();

            if (!double.TryParse(
                reparation.CoutString.Replace(".", ","),
                NumberStyles.Any,
                new CultureInfo("fr-FR"),
                out double cout))
            {
                ModelState.AddModelError("CoutString", "La valeur saisie pour le prix doit être un nombre positif, avec au maximum 2 décimales");
            }
            else
            {
                reparation.Cout = cout;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _reparationService.MettreAJourAsync(reparation);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ReparationExists(reparation.Id))
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

            var voitures = await _voitureService.ObtenirToutesAsync();
            ViewData["IdVoiture"] = new SelectList(voitures, "Id", "NomComplet", reparation.IdVoiture);
            return View(reparation);
        }

        // GET: Reparations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var reparation = await _reparationService.ObtenirParIdAsync(id.Value);
            if (reparation == null)
                return NotFound();

            return View(reparation);
        }

        // POST: Reparations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _reparationService.SupprimerAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ReparationExists(int id)
        {
            return await _reparationService.ExisteAsync(id);
        }
    }
}
