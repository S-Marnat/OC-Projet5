using ExpressVoitures.Data;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Services
{
    public class MarqueService : IMarqueService
    {
        private readonly ApplicationDbContext _context;

        public MarqueService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreerAsync(Marque marque)
        {
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Marques.AnyAsync(m => m.Id == id);
        }

        public async Task MettreAJourAsync(Marque marque)
        {
            _context.Marques.Update(marque);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> NomExisteAsync(string nom, int? idExclu = null)
        {
            return await _context.Marques.AnyAsync(m =>
                m.Nom == nom &&
                (idExclu == null || m.Id != idExclu));
        }

        public async Task<Marque?> ObtenirParIdAsync(int id)
        {
            return await _context.Marques.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Marque>> ObtenirToutesAsync()
        {
            return await _context.Marques.ToListAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var marque = await _context.Marques.FirstOrDefaultAsync(m => m.Id == id);
            if (marque != null)
            {
                _context.Marques.Remove(marque);
                await _context.SaveChangesAsync();
            }
        }
    }
}
