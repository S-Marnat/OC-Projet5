using System.ComponentModel.DataAnnotations;

namespace ExpressVoitures.Models
{
    public class Modele
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer le nom du modèle")]
        [StringLength(50)]
        public string Nom { get; set; }


        public int IdMarque {  get; set; }
        public Marque? Marque { get; set; }

        public List<Voiture> Voitures { get; set; } = new();
        public List<Finition> Finitions { get; set; } = new();
    }
}
