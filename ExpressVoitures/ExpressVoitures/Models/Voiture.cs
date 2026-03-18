using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpressVoitures.Models
{
    public class Voiture : IValidatableObject
    {
        public int Id { get; set; }

        [Required (ErrorMessage = "Veuillez indiquer le code VIN de la voiture")]
        [StringLength(17, ErrorMessage = "Le code VIN doit contenir 17 caractères")]
        public string CodeVin { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer l'année de la voiture")]
        public int Annee { get; set; }

        public string? Image { get; set; }

        [NotMapped]
        public IFormFile? TelechargerImage { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer la date d'achat de la voiture")]
        [DataType(DataType.Date)]
        public DateTime DateAchat { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer le prix d'achat de la voiture")]
        public double PrixAchat { get; set; }

        public bool VoitureReparee { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateMiseEnVente { get; set; }

        public double? PrixMiseEnVente { get; set; }

        public bool AnnoncePubliee { get; set; }
        public bool VoitureVendue { get; set; }



        public int IdMarque { get; set; }
        public Marque? Marque { get; set; }

        public int IdModele { get; set; }
        public Modele? Modele { get; set; }

        public int IdFinition { get; set; }
        public Finition? Finition { get; set; }

        public Vente? Vente { get; set; }

        public List<Reparation> Reparations { get; set; } = new();


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            int min = 1990;
            int max = DateTime.Now.Year;

            if (Annee < min || Annee > max)
            {
                yield return new ValidationResult(
                    $"Veuillez indiquer une année entre {min} et {max}.",
                    new[] { nameof(Annee) }
                );
            }
        }
    }
}
