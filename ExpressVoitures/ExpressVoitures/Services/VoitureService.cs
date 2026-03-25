using ExpressVoitures.Data;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Services
{
    public class VoitureService : IVoitureService
    {
        private readonly ApplicationDbContext _context;

        public VoitureService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreerAsync(Voiture voiture)
        {
            _context.Voitures.Add(voiture);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Voitures.AnyAsync(v => v.Id == id);
        }

        public async Task<bool> FinitionUtiliseeAsync(int idFinition)
        {
            return await _context.Voitures.AnyAsync(v => v.IdFinition == idFinition);
        }

        public async Task<bool> MarqueUtiliseeAsync(int idMarque)
        {
            return await _context.Voitures.AnyAsync(v => v.IdMarque == idMarque);
        }

        public async Task MettreAJourAsync(Voiture voiture)
        {
            _context.Voitures.Update(voiture);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ModeleUtiliseAsync(int idModele)
        {
            return await _context.Voitures.AnyAsync(v => v.IdModele == idModele);
        }

        public async Task<List<Voiture>> ObtenirParDisponibiliteAsync()
        {
            return await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .Include(v => v.Finition)
                .Include(v => v.Vente)
                .Include(v => v.Reparations)
                .Where(v => v.AnnoncePubliee == true)
                .ToListAsync();
        }

        public async Task<List<Voiture>> ObtenirParFinitionAsync(int idFinition)
        {
            return await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .Include(v => v.Finition)
                .Include(v => v.Vente)
                .Include(v => v.Reparations)
                .Where(v => v.IdFinition == idFinition)
                .ToListAsync();
        }

        public async Task<Voiture?> ObtenirParIdAsync(int id)
        {
            return await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .Include(v => v.Finition)
                .Include(v => v.Vente)
                .Include(v => v.Reparations)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<Voiture>> ObtenirParMarqueAsync(int idMarque)
        {
            return await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .Include(v => v.Finition)
                .Include(v => v.Vente)
                .Include(v => v.Reparations)
                .Where(v => v.IdMarque == idMarque)
                .ToListAsync();
        }

        public async Task<List<Voiture>> ObtenirParModeleAsync(int idModele)
        {
            return await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .Include(v => v.Finition)
                .Include(v => v.Vente)
                .Include(v => v.Reparations)
                .Where(v => v.IdModele == idModele)
                .ToListAsync();
        }

        public async Task<List<Voiture>> ObtenirParPresenceCodeVinAsync()
        {
            return await _context.Voitures
                .Where(v => v.CodeVin != null)
                .ToListAsync();
        }

        public async Task<List<Voiture>> ObtenirToutesAsync()
        {
            return await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .Include(v => v.Finition)
                .Include(v => v.Vente)
                .Include(v => v.Reparations)
                .ToListAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var voiture = await _context.Voitures.FirstOrDefaultAsync(v => v.Id == id);
            if (voiture != null)
            {
                _context.Voitures.Remove(voiture);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> TelechargerImageAsync(IFormFile fichier)
        {
            // Vérifier que le fichier n'est pas vide
            if (fichier == null || fichier.Length == 0)
                throw new Exception("FICHIER_VIDE");

            // Vérifier l'extension du fichier
            var extensionsAutorisees = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(fichier.FileName).ToLower();
            if (!extensionsAutorisees.Contains(extension))
                throw new Exception("EXTENSION_INVALIDE");

            // Vérifier la taille du fichier
            long tailleMax = 2 * 1024 * 1024;
            if (fichier.Length > tailleMax)
                throw new Exception("TAILLE_INVALIDE");

            // Vérification MIME
            if (!fichier.ContentType.StartsWith("image/"))
                throw new Exception("MIME_INVALIDE");

            // Génération d'un nom de fichier unique
            var nomImage = Guid.NewGuid() + Path.GetExtension(fichier.FileName);

            // Construction du chemin de l'image
            var nomChemin = Path.Combine("wwwroot/images/voitures", nomImage);

            // Création du fichier physique dans le projet
            using (var stream = new FileStream(nomChemin, FileMode.Create))
            {
                await fichier.CopyToAsync(stream);
            }

            return nomImage;
        }

        public async Task<bool> VinExisteAsync(string vin, int? idExclu = null)
        {
            return await _context.Voitures.AnyAsync(v =>
                v.CodeVin == vin &&
                (idExclu == null || v.Id != idExclu));
        }
    }
}
