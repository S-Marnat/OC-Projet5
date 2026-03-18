using ExpressVoitures.Models;

namespace ExpressVoitures.Interfaces
{
    public interface IVoitureService
    {
        Task<List<Voiture>> ObtenirToutesAsync();
        Task<Voiture?> ObtenirParIdAsync(int id);
        Task CreerAsync(Voiture voiture);
        Task MettreAJourAsync(Voiture voiture);
        Task SupprimerAsync(int id);

        Task<bool> ExisteAsync(int id);
        Task<bool> VinExisteAsync(string vin, int? idExclu = null);
        Task<bool> MarqueUtiliseeAsync(int idMarque);
        Task<bool> ModeleUtiliseAsync(int idModele);
        Task<bool> FinitionUtiliseeAsync(int idFinition);
        Task<List<Voiture>> ObtenirParDisponibiliteAsync();
        Task<List<Voiture>> ObtenirParMarqueAsync(int idMarque);
        Task<List<Voiture>> ObtenirParModeleAsync(int idModele);
        Task<List<Voiture>> ObtenirParFinitionAsync(int idFinition);
    }
}
