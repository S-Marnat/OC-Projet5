using ExpressVoitures.Data;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Services
{
    public class VenteService : IVenteService
    {
        private readonly ApplicationDbContext _context;

        public VenteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreerAsync(Vente vente)
        {
            _context.Ventes.Add(vente);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Ventes.AnyAsync(f => f.Id == id);
        }

        public async Task<bool> ExistePourVoitureAsync(int idVoiture, int? idExclu = null)
        {
            return await _context.Ventes.AnyAsync(v =>
                v.IdVoiture == idVoiture &&
                (idExclu == null || v.Id != idExclu));
        }

        public async Task MettreAJourAsync(Vente vente)
        {
            _context.Ventes.Update(vente);
            await _context.SaveChangesAsync();
        }

        public async Task<Vente?> ObtenirParIdAsync(int id)
        {
            return await _context.Ventes
                .Include(v => v.Voiture)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vente?> ObtenirParVoitureAsync(int idVoiture)
        {
            return await _context.Ventes
                .Include(v => v.Voiture)
                .FirstOrDefaultAsync(v => v.IdVoiture == idVoiture);
        }

        public async Task<List<Vente>> ObtenirToutesAsync()
        {
            return await _context.Ventes
                .Include(v =>v.Voiture)
                .ToListAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var vente = await _context.Ventes.FirstOrDefaultAsync(v => v.Id == id);
            if (vente != null)
            {
                _context.Ventes.Remove(vente);
                await _context.SaveChangesAsync();
            }
        }
    }
}
