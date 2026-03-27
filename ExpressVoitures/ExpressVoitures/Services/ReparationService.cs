using ExpressVoitures.Data;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Services
{
    public class ReparationService : IReparationService
    {
        private readonly ApplicationDbContext _context;

        public ReparationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreerAsync(Reparation reparation)
        {
            _context.Reparations.Add(reparation);
            await _context.SaveChangesAsync();

            // Mise à jour automatique de la voiture
            var voiture = await _context.Voitures.FindAsync(reparation.IdVoiture);
            if (voiture != null)
            {
                voiture.VoitureReparee = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Reparations.AnyAsync(f => f.Id == id);
        }

        public async Task MettreAJourAsync(Reparation reparation)
        {
            var reparationExiste = await _context.Reparations.FindAsync(reparation.Id);

            if (reparationExiste == null)
                throw new Exception("Réparation introuvable");

            reparationExiste.ReparationEffectuee = reparation.ReparationEffectuee;
            reparationExiste.Cout = reparation.Cout;
            reparationExiste.IdVoiture = reparation.IdVoiture;

            await _context.SaveChangesAsync();
        }

        public async Task<Reparation?> ObtenirParIdAsync(int id)
        {
            return await _context.Reparations
                .AsNoTracking()
                .Include(r => r.Voiture)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Reparation>> ObtenirParVoitureAsync(int idVoiture)
        {
            return await _context.Reparations
                .Where(r => r.IdVoiture ==  idVoiture)
                .ToListAsync();
        }

        public async Task<List<Reparation>> ObtenirToutesAsync()
        {
            return await _context.Reparations
                .Include(r => r.Voiture).ThenInclude(v => v.Marque)
                .Include(r => r.Voiture).ThenInclude(v => v.Modele)
                .Include(r => r.Voiture).ThenInclude(v => v.Finition)
                .ToListAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var reparation = await _context.Reparations.FirstOrDefaultAsync(r => r.Id == id);
            if (reparation != null)
            {
                int idVoiture = reparation.IdVoiture;

                _context.Reparations.Remove(reparation);
                await _context.SaveChangesAsync();

                // Vérifier s'il reste des réparations pour cette voiture
                bool encoreDesReparations = await _context.Reparations.AnyAsync(r => r.IdVoiture == idVoiture);

                var voiture = await _context.Voitures.FindAsync(idVoiture);
                if (voiture != null)
                {
                    voiture.VoitureReparee = encoreDesReparations;
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
