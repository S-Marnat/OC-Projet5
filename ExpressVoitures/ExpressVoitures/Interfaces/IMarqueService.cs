using ExpressVoitures.Models;

namespace ExpressVoitures.Interfaces
{
    public interface IMarqueService
    {
        Task<List<Marque>> ObtenirToutesAsync();
        Task<Marque?> ObtenirParIdAsync(int id);
        Task CreerAsync(Marque marque);
        Task MettreAJourAsync(Marque marque);
        Task SupprimerAsync(int id);

        Task<bool> ExisteAsync(int id);
        Task<bool> NomExisteAsync(string nom, int? idExclu = null);
    }
}
