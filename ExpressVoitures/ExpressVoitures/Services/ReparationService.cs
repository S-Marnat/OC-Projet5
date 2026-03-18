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
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Reparations.AnyAsync(f => f.Id == id);
        }

        public async Task MettreAJourAsync(Reparation reparation)
        {
            _context.Reparations.Update(reparation);
            await _context.SaveChangesAsync();
        }

        public async Task<Reparation?> ObtenirParIdAsync(int id)
        {
            return await _context.Reparations
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
                .Include(r => r.Voiture)
                .ToListAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var reparation = await _context.Reparations.FirstOrDefaultAsync(r => r.Id == id);
            if (reparation != null)
            {
                _context.Reparations.Remove(reparation);
                await _context.SaveChangesAsync();
            }
        }
    }
}
