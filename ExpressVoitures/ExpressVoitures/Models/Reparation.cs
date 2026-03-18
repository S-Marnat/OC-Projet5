using System.ComponentModel.DataAnnotations;

namespace ExpressVoitures.Models
{
    public class Reparation
    {
        public int Id { get; set; }

        [Required (ErrorMessage = "Veuillez indiquer la réparation effectuée")]
        public string ReparationEffectuee { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer le coût de la réparation")]
        public double Cout { get; set; }


        public int IdVoiture { get; set; }
        public Voiture? Voiture { get; set; }
    }
}
