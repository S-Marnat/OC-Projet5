using ExpressVoitures.Data;
using ExpressVoitures.Interfaces;
using ExpressVoitures.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Services
{
    public class ModeleService : IModeleService
    {
        private readonly ApplicationDbContext _context;

        public ModeleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreerAsync(Modele modele)
        {
            _context.Modeles.Add(modele);
            await _context.SaveChangesAsync();

        }
        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Modeles.AnyAsync(m => m.Id == id);
        }

        public async Task<bool> ExistePourMarqueAsync(string nom, int idMarque, int? idExclu = null)
        {
            return await _context.Modeles.AnyAsync(m =>
                m.Nom == nom &&
                m.IdMarque == idMarque &&
                (idExclu == null || m.Id != idExclu));
        }

        public async Task MettreAJourAsync(Modele modele)
        {
            _context.Modeles.Update(modele);
            await _context.SaveChangesAsync();
        }

        public async Task<Modele?> ObtenirParIdAsync(int id)
        {
            return await _context.Modeles
                .Include(m => m.Marque)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Modele>> ObtenirParMarqueAsync(int idMarque)
        {
            return await _context.Modeles
                .Where(m => m.IdMarque == idMarque)
                .ToListAsync();
        }

        public async Task<List<Modele>> ObtenirTousAsync()
        {
            return await _context.Modeles
                .Include(m => m.Marque)
                .ToListAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var modele = await _context.Modeles.FirstOrDefaultAsync(m => m.Id == id);
            if (modele !=  null)
            {
                _context.Modeles.Remove(modele);
                await _context.SaveChangesAsync();
            }
        }
    }
}
