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

        public async Task<IActionResult> DetailsSimple(int? id)
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
        public async Task<IActionResult> Create()
        {
            ListeAnnees();
            await ListesMarquesModelesFinitions();
            return View();
        }

        public async Task<IActionResult> CreateSimple(int? idMarqueCree, int? idModeleCree, int? idFinitionCree)
        {
            ListeAnnees();
            await ListesMarquesModelesFinitions();

            ViewBag.MarqueCree = idMarqueCree;
            ViewBag.ModeleCree = idModeleCree;
            ViewBag.FinitionCree = idFinitionCree;

            return View();
        }

        // POST: Voitures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CodeVin,Annee,Image,TelechargerImage,Description,DateAchat,PrixAchat,VoitureReparee,DateMiseEnVente,PrixMiseEnVente,AnnoncePubliee,VoitureVendue,IdMarque,IdModele,IdFinition")] Voiture voiture)
        {
            if (voiture.CodeVin != null)
            {
                bool voitureExiste;
                voitureExiste = await _voitureService.VinExisteAsync(voiture.CodeVin);
                if (voitureExiste)
                {
                    ModelState.AddModelError("", "Une voiture portant ce code VIN existe déjà.");
                }
            }

            if (voiture.DateAchat != null && voiture.DateMiseEnVente < voiture.DateAchat)
            {
                ModelState.AddModelError("", "La date de mise en vente ne peut pas être antérieure à la date d'achat de la voiture.");
            }

            if (ModelState.IsValid)
            {
                if (voiture.TelechargerImage != null)
                {
                    try
                    {
                        voiture.Image = await TelechargerImageAsync(voiture);
                    }
                    catch (Exception exception)
                    {
                        if (exception.Message == "EXTENSION_INVALIDE")
                            ModelState.AddModelError("TelechargerImage", "Seuls les fichiers JPG et PNG sont autorisés.");

                        if (exception.Message == "TAILLE_INVALIDE")
                            ModelState.AddModelError("TelechargerImage", "La taille du fichier ne doit pas dépasser 2 Mo.");

                        ListeAnnees();
                        await ListesMarquesModelesFinitions();
                        return View(voiture);
                    }
                }
                await _voitureService.CreerAsync(voiture);
                return View("CreateSucces");
            }

            ListeAnnees();
            await ListesMarquesModelesFinitions();
            return View(voiture);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSimple([Bind("Id,Annee,Image,TelechargerImage,PrixMiseEnVente,AnnoncePubliee,IdMarque,IdModele,IdFinition")] Voiture voiture)
        {
            if (ModelState.IsValid)
            {
                if (voiture.TelechargerImage != null)
                {
                    try
                    {
                        voiture.Image = await TelechargerImageAsync(voiture);
                    }
                    catch (Exception exception)
                    {
                        if (exception.Message == "EXTENSION_INVALIDE")
                            ModelState.AddModelError("TelechargerImage", "Seuls les fichiers JPG et PNG sont autorisés.");

                        if (exception.Message == "TAILLE_INVALIDE")
                            ModelState.AddModelError("TelechargerImage", "La taille du fichier ne doit pas dépasser 2 Mo.");

                        ListeAnnees();
                        await ListesMarquesModelesFinitions();
                        return View(voiture);
                    }
                }
                voiture.AnnoncePubliee = true;
                await _voitureService.CreerAsync(voiture);
                return View("CreateSucces");
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

        public async Task<IActionResult> EditSimple(int? id)
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

            if (voiture.CodeVin != null)
            {
                bool voitureExiste;
                voitureExiste = await _voitureService.VinExisteAsync(voiture.CodeVin, voiture.Id);
                if (voitureExiste)
                {
                    ModelState.AddModelError("", "Une voiture portant ce code VIN existe déjà.");
                }
            }

            if (voiture.DateAchat != null && voiture.DateMiseEnVente < voiture.DateAchat)
            {
                ModelState.AddModelError("", "La date de mise en vente ne peut pas être antérieure à la date d'achat de la voiture.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (voiture.TelechargerImage != null)
                    {
                        try
                        {
                            voiture.Image = await TelechargerImageAsync(voiture);
                        }
                        catch (Exception exception)
                        {
                            if (exception.Message == "EXTENSION_INVALIDE")
                                ModelState.AddModelError("TelechargerImage", "Seuls les fichiers JPG et PNG sont autorisés.");

                            if (exception.Message == "TAILLE_INVALIDE")
                                ModelState.AddModelError("TelechargerImage", "La taille du fichier ne doit pas dépasser 2 Mo.");

                            ListeAnnees();
                            await ListesMarquesModelesFinitions();
                            return View(voiture);
                        }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSimple(int id, [Bind("Id,Annee,Image,TelechargerImage,PrixMiseEnVente,AnnoncePubliee,IdMarque,IdModele,IdFinition")] Voiture voiture)
        {
            if (id != voiture.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (voiture.TelechargerImage != null)
                    {
                        try
                        {
                            voiture.Image = await TelechargerImageAsync(voiture);
                        }
                        catch (Exception exception)
                        {
                            if (exception.Message == "EXTENSION_INVALIDE")
                                ModelState.AddModelError("TelechargerImage", "Seuls les fichiers JPG et PNG sont autorisés.");

                            if (exception.Message == "TAILLE_INVALIDE")
                                ModelState.AddModelError("TelechargerImage", "La taille du fichier ne doit pas dépasser 2 Mo.");

                            ListeAnnees();
                            await ListesMarquesModelesFinitions();
                            return View(voiture);
                        }
                    }
                    voiture.AnnoncePubliee = true;
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
                return RedirectToAction("DetailsSimple", new { id = voiture.Id });
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

            // Stockage des informations de la voiture avant de la supprimer
            var voitureSupprimee = new Voiture
            {
                Id = voiture.Id,
                Annee = voiture.Annee,
                Marque = voiture.Marque,
                Modele = voiture.Modele,
                Finition = voiture.Finition
            };

            await _voitureService.SupprimerAsync(id);
            return View("DeleteSucces", voitureSupprimee);
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

        private async Task<string?> TelechargerImageAsync(Voiture voiture)
        {
            // Génération d'un nom de fichier unique
            var nomImage = Guid.NewGuid() + Path.GetExtension(voiture.TelechargerImage.FileName);

            // Vérifier l'extension du fichier
            var extensionsAutorisees = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(voiture.TelechargerImage.FileName).ToLower();

            if (!extensionsAutorisees.Contains(extension))
                throw new Exception("EXTENSION_INVALIDE");

            // Vérifier la taille du fichier
            long tailleMax = 2 * 1024 * 1024;

            if (voiture.TelechargerImage.Length > tailleMax)
                throw new Exception("TAILLE_INVALIDE");

            // Construction du chemin de l'image
            var nomChemin = Path.Combine("wwwroot/images/voitures", nomImage);

            // Création du fichier physique dans le projet
            using (var stream = new FileStream(nomChemin, FileMode.Create))
            {
                await voiture.TelechargerImage.CopyToAsync(stream);
            }

            return nomImage;
        }
    }
}
