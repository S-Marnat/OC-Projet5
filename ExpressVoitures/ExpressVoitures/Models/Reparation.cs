using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpressVoitures.Models
{
    public class Reparation
    {
        public int Id { get; set; }

        [Required (ErrorMessage = "Veuillez indiquer la réparation effectuée")]
        public string ReparationEffectuee { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Veuillez indiquer le coût de la réparation")]
        [RegularExpression(@"^\d+([.,]\d{1,2})?$", ErrorMessage = "Le prix doit être un nombre positif avec au maximum 2 décimales")]
        public string CoutString { get; set; }
        public double Cout { get; set; }


        public int IdVoiture { get; set; }
        public Voiture? Voiture { get; set; }
    }
}
