using ExpressVoitures.Models;

namespace ExpressVoitures.Interfaces
{
    public interface IHomeService
    {
        Task<List<Voiture>> ObtenirVoituresPublieesAsync();
    }
}
