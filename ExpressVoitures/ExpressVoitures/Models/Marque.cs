using System.ComponentModel.DataAnnotations;

namespace ExpressVoitures.Models
{
    public class Marque
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer le nom de la marque")]
        [StringLength(50)]
        public string Nom { get; set; }


        public List<Voiture> Voitures { get; set; } = new();
        public List<Modele> Modeles { get; set; } = new();
    }
}
