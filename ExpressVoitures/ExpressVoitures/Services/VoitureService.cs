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

        public async Task<bool> VinExisteAsync(string vin, int? idExclu = null)
        {
            return await _context.Voitures.AnyAsync(v =>
                v.CodeVin == vin &&
                (idExclu == null || v.Id != idExclu));
        }
    }
}
