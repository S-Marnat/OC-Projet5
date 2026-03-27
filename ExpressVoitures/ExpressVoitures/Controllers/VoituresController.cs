using ExpressVoitures.Data;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize]
    public class VoituresController : Controller
    {
        private readonly IVoitureService _voitureService;
        private readonly IMarqueService _marqueService;
        private readonly IModeleService _modeleService;
        private readonly IFinitionService _finitionService;
        private readonly IReparationService _reparationService;
        private readonly IVenteService _venteService;
        private readonly UserManager<IdentityUser> _userManager;

        public VoituresController(IVoitureService voitureService, IMarqueService marqueService, IModeleService modeleService,
            IFinitionService finitionService, IReparationService reparationService, IVenteService venteService, UserManager<IdentityUser> userManager)
        {
            _voitureService = voitureService;
            _marqueService = marqueService;
            _modeleService = modeleService;
            _finitionService = finitionService;
            _reparationService = reparationService;
            _venteService = venteService;
            _userManager = userManager;
        }

        // GET: Voitures
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Index()
        {
            var voitures = await _voitureService.ObtenirToutesAsync();
            return View(voitures);
        }

        // GET: Voitures/Details/5
        [Authorize(Roles = "Administrateur")]
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

        [AllowAnonymous]
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
        [Authorize(Roles = "Administrateur")]
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
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Create([Bind("Id,CodeVin,Annee,Image,TelechargerImage,Description,DateAchat,PrixAchat,PrixAchatString,VoitureReparee,DateMiseEnVente,PrixMiseEnVente,PrixMiseEnVenteString,AnnoncePubliee,VoitureVendue,IdMarque,IdModele,IdFinition")] Voiture voiture)
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

            if (!double.TryParse(voiture.PrixAchatString.Replace(",", "."),
                     NumberStyles.Any,
                     CultureInfo.InvariantCulture,
                     out double prixAchat))
            {
                ModelState.AddModelError("PrixAchatString", "Le prix doit être un nombre positif avec au maximum 2 décimales");
            }
            else
            {
                voiture.PrixMiseEnVente = prixAchat;
            }

            if (!double.TryParse(voiture.PrixMiseEnVenteString.Replace(",", "."),
                     NumberStyles.Any,
                     CultureInfo.InvariantCulture,
                     out double prixVente))
            {
                ModelState.AddModelError("PrixMiseEnVenteString", "Le prix doit être un nombre positif avec au maximum 2 décimales");
            }
            else
            {
                voiture.PrixMiseEnVente = prixVente;
            }

            if (ModelState.IsValid)
            {
                if (!await TentativeTelechargementImageAsync(voiture))
                {
                    return await RetournerVueAvecListes(voiture);
                }
                voiture.IdUtilisateur = _userManager.GetUserId(User);
                await _voitureService.CreerAsync(voiture);
                return View("CreateSucces");
            }

            ListeAnnees();
            await ListesMarquesModelesFinitions();
            return View(voiture);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSimple([Bind("Id,Annee,Image,TelechargerImage,PrixMiseEnVenteString,PrixMiseEnVente,AnnoncePubliee,IdMarque,IdModele,IdFinition")] Voiture voiture)
        {
            if (!double.TryParse(voiture.PrixMiseEnVenteString.Replace(",", "."),
                     NumberStyles.Any,
                     CultureInfo.InvariantCulture,
                     out double prix))
            {
                ModelState.AddModelError("PrixMiseEnVenteString", "Le prix doit être un nombre positif avec au maximum 2 décimales");
            }
            else
            {
                voiture.PrixMiseEnVente = prix;
            }

            if (ModelState.IsValid)
            {
                if (!await TentativeTelechargementImageAsync(voiture))
                {
                    return await RetournerVueAvecListes(voiture);
                }

                if (voiture.TelechargerImage != null)
                {
                    voiture.Image = await _voitureService.TelechargerImageAsync(voiture.TelechargerImage);
                }

                voiture.AnnoncePubliee = true;
                voiture.IdUtilisateur = _userManager.GetUserId(User);
                
                await _voitureService.CreerAsync(voiture);
                return View("CreateSucces");
            }

            ListeAnnees();
            await ListesMarquesModelesFinitions();
            return View(voiture);
        }

        // GET: Voitures/Edit/5
        [Authorize(Roles = "Administrateur")]
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

            if (voiture.PrixAchat.HasValue)
            {
                voiture.PrixAchatString = voiture.PrixAchat.Value.ToString("0.##", new CultureInfo("fr-FR"));
            }
            voiture.PrixMiseEnVenteString = voiture.PrixMiseEnVente.ToString("0.##", new CultureInfo("fr-FR"));

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

            voiture.PrixMiseEnVenteString = voiture.PrixMiseEnVente.ToString("0.##", new CultureInfo("fr-FR"));

            // Vérifier l'autorisation
            if (!User.IsInRole("Administrateur"))
            {
                if (voiture.IdUtilisateur != _userManager.GetUserId(User))
                    return Forbid();
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
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodeVin,Annee,Image,TelechargerImage,Description,DateAchat,PrixAchatString,PrixAchat,VoitureReparee,DateMiseEnVente,PrixMiseEnVenteString,PrixMiseEnVente,AnnoncePubliee,VoitureVendue,IdMarque,IdModele,IdFinition")] Voiture voiture)
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

            if (!double.TryParse(voiture.PrixAchatString.Replace(",", "."),
                     NumberStyles.Any,
                     CultureInfo.InvariantCulture,
                     out double prixAchat))
            {
                ModelState.AddModelError("PrixAchatString", "Le prix doit être un nombre positif avec au maximum 2 décimales");
            }
            else
            {
                voiture.PrixMiseEnVente = prixAchat;
            }

            if (!double.TryParse(voiture.PrixMiseEnVenteString.Replace(",", "."),
                     NumberStyles.Any,
                     CultureInfo.InvariantCulture,
                     out double prixVente))
            {
                ModelState.AddModelError("PrixMiseEnVenteString", "Le prix doit être un nombre positif avec au maximum 2 décimales");
            }
            else
            {
                voiture.PrixMiseEnVente = prixVente;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await TentativeTelechargementImageAsync(voiture))
                    {
                        return await RetournerVueAvecListes(voiture);
                    }
                    if (voiture.TelechargerImage != null)
                    {
                        await _voitureService.MettreAJourAvecImageAsync(voiture, voiture.TelechargerImage);
                    }
                    else
                    {
                        await _voitureService.MettreAJourAsync(voiture);
                    }
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
        public async Task<IActionResult> EditSimple(int id, [Bind("Id,Annee,Image,TelechargerImage,PrixMiseEnVenteString,PrixMiseEnVente,AnnoncePubliee,IdMarque,IdModele,IdFinition")] Voiture voiture)
        {
            if (id != voiture.Id)
            {
                return NotFound();
            }

            if (!double.TryParse(voiture.PrixMiseEnVenteString.Replace(",", "."),
                     NumberStyles.Any,
                     CultureInfo.InvariantCulture,
                     out double prixVente))
            {
                ModelState.AddModelError("PrixMiseEnVenteString", "Le prix doit être un nombre positif avec au maximum 2 décimales");
            }
            else
            {
                voiture.PrixMiseEnVente = prixVente;
            }

            var voitureOriginale = await _voitureService.ObtenirParIdAsync(id);

            if (voitureOriginale == null)
            {
                return NotFound();
            }

            // Vérifier l'autorisation
            if (!User.IsInRole("Administrateur"))
            {
                if (voitureOriginale.IdUtilisateur != _userManager.GetUserId(User))
                {
                    return Forbid();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await TentativeTelechargementImageAsync(voiture))
                    {
                        return await RetournerVueAvecListes(voiture);
                    }
                    voiture.AnnoncePubliee = true;
                    voiture.IdUtilisateur = voitureOriginale.IdUtilisateur;
                    if (voiture.TelechargerImage != null)
                    {
                        await _voitureService.MettreAJourAvecImageAsync(voiture, voiture.TelechargerImage);
                    }
                    else
                    {
                        await _voitureService.MettreAJourAsync(voiture);
                    }
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

            // Vérifier l'autorisation
            if (!User.IsInRole("Administrateur"))
            {
                if (voiture.IdUtilisateur != _userManager.GetUserId(User))
                    return Forbid();
            }

            return View(voiture);
        }

        // POST: Voitures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voiture = await _voitureService.ObtenirParIdAsync(id);

            if (voiture == null)
            {
                return NotFound();
            }

            // Vérifier l'autorisation
            if (!User.IsInRole("Administrateur"))
            {
                if (voiture.IdUtilisateur != _userManager.GetUserId(User))
                    return Forbid();
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
        private async Task<IActionResult> RetournerVueAvecListes(Voiture voiture)
        {
            ListeAnnees();
            await ListesMarquesModelesFinitions();
            return View(voiture);
        }

        private async Task<bool> TentativeTelechargementImageAsync(Voiture voiture)
        {
            if (voiture.TelechargerImage == null)
                return true;

            try
            {
                _voitureService.ValiderImage(voiture.TelechargerImage);
                return true;
            }
            catch (Exception exception)
            {
                if (exception.Message == "EXTENSION_INVALIDE")
                    ModelState.AddModelError("TelechargerImage", "Seuls les fichiers JPG et PNG sont autorisés.");

                if (exception.Message == "TAILLE_INVALIDE")
                    ModelState.AddModelError("TelechargerImage", "La taille du fichier ne doit pas dépasser 2 Mo.");

                if (exception.Message == "MIME_INVALIDE")
                    ModelState.AddModelError("TelechargerImage", "Le fichier n'est pas une image valide.");

                if (exception.Message == "FICHIER_VIDE")
                    ModelState.AddModelError("TelechargerImage", "Aucun fichier n'a été envoyé.");

                return false;
            }
        }
    }
}
