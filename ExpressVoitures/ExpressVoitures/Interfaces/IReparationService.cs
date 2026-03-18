using ExpressVoitures.Models;

namespace ExpressVoitures.Interfaces
{
    public interface IReparationService
    {
        Task<List<Reparation>> ObtenirToutesAsync();
        Task<Reparation?> ObtenirParIdAsync(int id);
        Task CreerAsync(Reparation reparation);
        Task MettreAJourAsync(Reparation reparation);
        Task SupprimerAsync(int id);

        Task<List<Reparation>> ObtenirParVoitureAsync(int idVoiture);
        Task<bool> ExisteAsync(int id);
    }
}
