using ExpressVoitures.Data;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Services
{
    public class FinitionService : IFinitionService
    {
        private readonly ApplicationDbContext _context;

        public FinitionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreerAsync(Finition finition)
        {
            _context.Finitions.Add(finition);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Finitions.AnyAsync(f => f.Id == id);
        }

        public async Task<bool> ExistePourModeleAsync(string nom, int idModele, int? idExclu = null)
        {
            return await _context.Finitions.AnyAsync(f =>
                f.Nom == nom &&
                f.IdModele == idModele &&
                (idExclu == null || f.Id != idExclu));
        }

        public async Task MettreAJourAsync(Finition finition)
        {
            _context.Finitions.Update(finition);
            await _context.SaveChangesAsync();
        }

        public async Task<Finition?> ObtenirParIdAsync(int id)
        {
            return await _context.Finitions
                .Include(f => f.Modele)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<Finition>> ObtenirParModeleAsync(int idModele)
        {
            return await _context.Finitions
                .Where(f => f.IdModele == idModele)
                .ToListAsync();
        }

        public async Task<List<Finition>> ObtenirToutesAsync()
        {
            return await _context.Finitions
                .Include(f => f.Modele)
                .ToListAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var finition = await _context.Finitions.FirstOrDefaultAsync(f => f.Id == id);
            if (finition != null)
            {
                _context.Finitions.Remove(finition);
                await _context.SaveChangesAsync();
            }
        }
    }
}
