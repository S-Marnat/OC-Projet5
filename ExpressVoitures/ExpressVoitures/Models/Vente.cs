using System.ComponentModel.DataAnnotations;

namespace ExpressVoitures.Models
{
    public class Vente
    {
        public int Id { get; set; }

        [Required (ErrorMessage = "Veuillez indiquer une date de vente")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }


        public int? IdVoiture { get; set; }
        public Voiture? Voiture { get; set; }
    }
}
