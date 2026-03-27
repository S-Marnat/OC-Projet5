using ExpressVoitures.Models;

namespace ExpressVoitures.Interfaces
{
    public interface IVoitureService
    {
        Task<List<Voiture>> ObtenirToutesAsync();
        Task<Voiture?> ObtenirParIdAsync(int id);
        Task CreerAsync(Voiture voiture);
        Task CreerAvecImageAsync(Voiture voiture, IFormFile fichier);
        Task MettreAJourAsync(Voiture voiture);
        Task MettreAJourAvecImageAsync(Voiture voiture, IFormFile nouvelleImage);
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
        Task<List<Voiture>> ObtenirParPresenceCodeVinAsync();
        Task<string> TelechargerImageAsync(IFormFile fichier);

        void ValiderImage(IFormFile fichier);
    }
}
