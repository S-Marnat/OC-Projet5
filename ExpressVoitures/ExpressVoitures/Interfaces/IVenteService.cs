using ExpressVoitures.Models;

namespace ExpressVoitures.Interfaces
{
    public interface IVenteService
    {
        Task<List<Vente>> ObtenirToutesAsync();
        Task<Vente?> ObtenirParIdAsync(int id);
        Task CreerAsync(Vente vente);
        Task MettreAJourAsync(Vente vente);
        Task SupprimerAsync(int id);

        Task<Vente?> ObtenirParVoitureAsync(int idVoiture);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExistePourVoitureAsync(int idVoiture, int? idExclu = null);
    }
}
