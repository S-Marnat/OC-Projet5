using ExpressVoitures.Models;

namespace ExpressVoitures.Interfaces
{
    public interface IFinitionService
    {
        Task<List<Finition>> ObtenirToutesAsync();
        Task<Finition?> ObtenirParIdAsync(int id);
        Task CreerAsync(Finition finition);
        Task MettreAJourAsync(Finition finition);
        Task SupprimerAsync(int id);

        Task<List<Finition>> ObtenirParModeleAsync(int idModele);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExistePourModeleAsync(string nom, int idModele, int? idExclu = null);
    }
}
