using ExpressVoitures.Data;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressVoitures.Controllers
{
    public class VoituresController : Controller
    {
        private readonly IVoitureService _voitureService;
        private readonly IMarqueService _marqueService;
        private readonly IModeleService _modeleService;
        private readonly IFinitionService _finitionService;
        private readonly IReparationService _reparationService;
        private readonly IVenteService _venteService;

        public VoituresController(IVoitureService voitureService, IMarqueService marqueService, IModeleService modeleService,
            IFinitionService finitionService, IReparationService reparationService, IVenteService venteService)
        {
            _voitureService = voitureService;
            _marqueService = marqueService;
            _modeleService = modeleService;
            _finitionService = finitionService;
            _reparationService = reparationService;
            _venteService = venteService;
        }

        // GET: Voitures
        public async Task<IActionResult> Index()
        {
            var voitures = await _voitureService.ObtenirToutesAsync();
            return View(voitures);
        }

        // GET: Voitures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voiture = await _voitureService.ObtenirParIdAsync(id.Value);
            if (voiture == null)
            {
                return NotFound();
            }

            return View(voiture);
        }

        // GET: Voitures/Create
        public async Task<IActionResult> Create(int? idMarque, int? idModele)
        {
            ListeAnnees();
            await ListesMarquesModelesFinitions();
            return View();
        }

        // POST: Voitures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CodeVin,Annee,Image,TelechargerImage,Description,DateAchat,PrixAchat,VoitureReparee,DateMiseEnVente,PrixMiseEnVente,AnnoncePubliee,VoitureVendue,IdMarque,IdModele,IdFinition")] Voiture voiture)
        {
            bool voitureExiste;
            voitureExiste = await _voitureService.VinExisteAsync(voiture.CodeVin);
            if (voitureExiste)
            {
                ModelState.AddModelError("", "Une voiture portant ce code VIN existe déjà.");
            }

            if (voiture.DateMiseEnVente < voiture.DateAchat)
            {
                ModelState.AddModelError("", "La date de mise en vente ne peut pas être antérieure à la date d'achat de la voiture.");
            }

            if (ModelState.IsValid)
            {
                if (voiture.TelechargerImage != null)
                {
                    // Génération d'un nom de fichier unique
                    var nomImage = Guid.NewGuid() + Path.GetExtension(voiture.TelechargerImage.FileName);

                    // Vérifier l'extension du fichier
                    var extensionsAutorisees = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(voiture.TelechargerImage.FileName).ToLower();

                    if (!extensionsAutorisees.Contains(extension))
                    {
                        ModelState.AddModelError("TelechargerImage", "Seuls les fichiers JPG et PNG sont autorisés.");
                        ListeAnnees();
                        await ListesMarquesModelesFinitions();
                        return View(voiture);
                    }

                    // Vérifier la taille du fichier
                    long tailleMax = 2 * 1024 * 1024;

                    if (voiture.TelechargerImage.Length > tailleMax)
                    {
                        ModelState.AddModelError("TelechargerImage", "La taille du fichier ne doit pas dépasser 2 Mo.");
                        ListeAnnees();
                        await ListesMarquesModelesFinitions();
                        return View(voiture);
                    }

                    // Construction du chemin de l'image
                    var nomChemin = Path.Combine("wwwroot/images/voitures", nomImage);

                    // Création du fichier physique dans le projet
                    using (var stream = new FileStream(nomChemin, FileMode.Create))
                    {
                        await voiture.TelechargerImage.CopyToAsync(stream);
                    }

                    voiture.Image = nomImage;
                }
                await _voitureService.CreerAsync(voiture);
                return RedirectToAction(nameof(Index));
            }

            ListeAnnees();
            await ListesMarquesModelesFinitions();
            return View(voiture);
        }

        // GET: Voitures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voiture = await _voitureService.ObtenirParIdAsync(id.Value);
            if (voiture == null)
            {
                return NotFound();
            }

            ListeAnnees();
            await ListesMarquesModelesFinitions();
            return View(voiture);
        }

        // POST: Voitures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodeVin,Annee,Image,TelechargerImage,Description,DateAchat,PrixAchat,VoitureReparee,DateMiseEnVente,PrixMiseEnVente,AnnoncePubliee,VoitureVendue,IdMarque,IdModele,IdFinition")] Voiture voiture)
        {
            if (id != voiture.Id)
            {
                return NotFound();
            }

            bool voitureExiste;
            voitureExiste = await _voitureService.VinExisteAsync(voiture.CodeVin, voiture.Id);
            if (voitureExiste)
            {
                ModelState.AddModelError("", "Une voiture portant ce code VIN existe déjà.");
            }

            if (voiture.DateMiseEnVente < voiture.DateAchat)
            {
                ModelState.AddModelError("", "La date de mise en vente ne peut pas être antérieure à la date d'achat de la voiture.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (voiture.TelechargerImage != null)
                    {
                        // Vérifier l'extension du fichier
                        var extensionsAutorisees = new[] { ".jpg", ".jpeg", ".png" };
                        var extension = Path.GetExtension(voiture.TelechargerImage.FileName).ToLower();

                        if (!extensionsAutorisees.Contains(extension))
                        {
                            ModelState.AddModelError("TelechargerImage", "Seuls les fichiers JPG et PNG sont autorisés.");
                            ListeAnnees();
                            await ListesMarquesModelesFinitions();
                            return View(voiture);
                        }

                        // Vérifier la taille du fichier
                        long tailleMax = 2 * 1024 * 1024;

                        if (voiture.TelechargerImage.Length > tailleMax)
                        {
                            ModelState.AddModelError("TelechargerImage", "La taille du fichier ne doit pas dépasser 2 Mo.");
                            ListeAnnees();
                            await ListesMarquesModelesFinitions();
                            return View(voiture);
                        }

                        // Supprimer l'ancienne image si elle existe
                        if (!string.IsNullOrEmpty(voiture.Image))
                        {
                            var ancienChemin = Path.Combine("wwwroot/images/voitures", voiture.Image);
                            if (System.IO.File.Exists(ancienChemin))
                            {
                                System.IO.File.Delete(ancienChemin);
                            }
                        }

                        // Génération d'un nom de fichier unique
                        var nomImage = Guid.NewGuid() + Path.GetExtension(voiture.TelechargerImage.FileName);

                        // Construction du chemin de l'image
                        var nomChemin = Path.Combine("wwwroot/images/voitures", nomImage);

                        // Création du fichier physique dans le projet
                        using (var stream = new FileStream(nomChemin, FileMode.Create))
                        {
                            await voiture.TelechargerImage.CopyToAsync(stream);
                        }

                        voiture.Image = nomImage;
                    }
                    await _voitureService.MettreAJourAsync(voiture);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await VoitureExists(voiture.Id))
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

            ListeAnnees();
            await ListesMarquesModelesFinitions();
            return View(voiture);
        }

        // GET: Voitures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voiture = await _voitureService.ObtenirParIdAsync(id.Value);
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
            // Supprimer l'image de l'application si présente
            var voiture = await _voitureService.ObtenirParIdAsync(id);
            if (!string.IsNullOrEmpty(voiture.Image))
            {
                var cheminImage = Path.Combine("wwwroot/images/voitures", voiture.Image);
                if (System.IO.File.Exists(cheminImage))
                {
                    System.IO.File.Delete(cheminImage);
                }
            }

            await _voitureService.SupprimerAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> VoitureExists(int id)
        {
            return await _voitureService.ExisteAsync(id);
        }

        private void ListeAnnees()
        {
            var anneeLaPlusAncienne = 1990;
            var anneeActuelle = DateTime.Now.Year;

            var annees = Enumerable
                .Range(anneeLaPlusAncienne, anneeActuelle - anneeLaPlusAncienne + 1)
                .Reverse()
                .ToList();

            ViewData["Annees"] = new SelectList(annees);
        }

        private void prixVenteFinal()
        {

        }

        private async Task ListesMarquesModelesFinitions()
        {
            // Charger toutes les marques
            var marques = await _marqueService.ObtenirToutesAsync();
            ViewBag.Marques = new SelectList(marques, "Id", "Nom");

            // Construire le dictionnaire : Modèles par marque
            var modelesParMarque = new Dictionary<int, List<SelectListItem>>();

            foreach (var marque in marques)
            {
                var modeles = await _modeleService.ObtenirParMarqueAsync(marque.Id);

                modelesParMarque[marque.Id] = modeles
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Nom
                    })
                    .ToList();
            }

            ViewBag.ModelesParMarque = modelesParMarque;

            // Construire le dictionnaire : Finitions par modèle
            var finitionsParModele = new Dictionary<int, List<SelectListItem>>();

            var tousLesModeles = await _modeleService.ObtenirTousAsync();
            foreach (var modele in tousLesModeles)
            {
                var finitions = await _finitionService.ObtenirParModeleAsync(modele.Id);

                finitionsParModele[modele.Id] = finitions
                    .Select(f => new SelectListItem
                    {
                        Value = f.Id.ToString(),
                        Text = f.Nom
                    })
                    .ToList();
            }

            ViewBag.FinitionsParModele = finitionsParModele;
        }
    }
}
