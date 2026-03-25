using ExpressVoitures.Data;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Models;
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
    [Authorize(Roles = "Administrateur")]
    public class VentesController : Controller
    {
        private readonly IVenteService _venteService;
        private readonly IVoitureService _voitureService;

        public VentesController(IVenteService venteService, IVoitureService voitureService)
        {
            _venteService = venteService;
            _voitureService = voitureService;
        }

        // GET: Ventes
        public async Task<IActionResult> Index()
        {
            var ventes = await _venteService.ObtenirToutesAsync();
            return View(ventes);
        }

        // GET: Ventes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vente = await _venteService.ObtenirParIdAsync(id.Value);
            if (vente == null)
            {
                return NotFound();
            }

            return View(vente);
        }

        // GET: Ventes/Create
        public async Task<IActionResult> Create()
        {
            var voitures = await _voitureService.ObtenirToutesAsync();
            ViewData["IdVoiture"] = new SelectList(await _voitureService.ObtenirParPresenceCodeVinAsync(), "Id", "CodeVin");
            return View();
        }

        // POST: Ventes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,IdVoiture")] Vente vente)
        {
            bool venteExiste;
            venteExiste = await _venteService.ExistePourVoitureAsync(vente.IdVoiture.Value);
            if (venteExiste)
            {
                ModelState.AddModelError("", "Une vente a déjà été ajoutée pour cette voiture.");
            }

            var voitureVendue = await _voitureService.ObtenirParIdAsync(vente.IdVoiture.Value);
            if (voitureVendue != null)
            {
                if (vente.Date < voitureVendue.DateAchat)
                {
                    ModelState.AddModelError("", "La date de vente ne peut pas être antérieure à la date d'achat de la voiture.");
                }
            }

            if (ModelState.IsValid)
            {
                await _venteService.CreerAsync(vente);
                return RedirectToAction(nameof(Index));
            }
            var voitures = await _voitureService.ObtenirToutesAsync();
            ViewData["IdVoiture"] = new SelectList(await _voitureService.ObtenirParPresenceCodeVinAsync(), "Id", "CodeVin", vente.IdVoiture);
            return View(vente);
        }

        // GET: Ventes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vente = await _venteService.ObtenirParIdAsync(id.Value);
            if (vente == null)
            {
                return NotFound();
            }
            var voitures = await _voitureService.ObtenirToutesAsync();
            ViewData["IdVoiture"] = new SelectList(await _voitureService.ObtenirParPresenceCodeVinAsync(), "Id", "CodeVin", vente.IdVoiture);
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

            bool venteExiste;
            venteExiste = await _venteService.ExistePourVoitureAsync(vente.IdVoiture.Value, vente.Id);
            if (venteExiste)
            {
                ModelState.AddModelError("", "Une vente a déjà été ajoutée pour cette voiture.");
            }

            var voitureVendue = await _voitureService.ObtenirParIdAsync(vente.IdVoiture.Value);
            if (voitureVendue != null)
            {
                if (vente.Date < voitureVendue.DateAchat)
                {
                    ModelState.AddModelError("", "La date de vente ne peut pas être antérieure à la date d'achat de la voiture.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _venteService.MettreAJourAsync(vente);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await VenteExists(vente.Id))
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
            ViewData["IdVoiture"] = new SelectList(await _voitureService.ObtenirParPresenceCodeVinAsync(), "Id", "CodeVin", vente.IdVoiture);
            return View(vente);
        }

        // GET: Ventes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vente = await _venteService.ObtenirParIdAsync(id.Value);
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
            await _venteService.SupprimerAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> VenteExists(int id)
        {
            return await _venteService.ExisteAsync(id);
        }
    }
}
