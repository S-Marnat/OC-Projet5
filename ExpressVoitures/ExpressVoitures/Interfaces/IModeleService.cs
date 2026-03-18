using ExpressVoitures.Models;

namespace ExpressVoitures.Interfaces
{
    public interface IModeleService
    {
        Task<List<Modele>> ObtenirTousAsync();
        Task<Modele?> ObtenirParIdAsync(int id);
        Task CreerAsync(Modele modele);
        Task MettreAJourAsync(Modele modele);
        Task SupprimerAsync(int id);

        Task<List<Modele>> ObtenirParMarqueAsync(int idMarque);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExistePourMarqueAsync(string nom, int idMarque, int? idExclu = null);

    }
}
