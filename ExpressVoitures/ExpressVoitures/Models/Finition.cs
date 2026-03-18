using System.ComponentModel.DataAnnotations;

namespace ExpressVoitures.Models
{
    public class Finition
    {
        public int Id { get; set; }

        [Required (ErrorMessage = "Veuillez indiquer le nom de la finition")]
        [StringLength(50)]
        public string Nom { get; set; }


        public int IdModele { get; set; }
        public Modele? Modele { get; set; }

        public List<Voiture> Voitures { get; set; } = new();
    }
}
