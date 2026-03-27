using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpressVoitures.Models
{
    public class Voiture : IValidatableObject
    {
        public int Id { get; set; }

        [StringLength(17, ErrorMessage = "Le code VIN doit contenir 17 caractères")]
        public string? CodeVin { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer l'année de la voiture")]
        public int Annee { get; set; }

        public string? Image { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Veuillez ajouter une image")]
        public IFormFile TelechargerImage { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateAchat { get; set; }

        [NotMapped]
        [RegularExpression(@"^\d+([.,]\d{1,2})?$", ErrorMessage = "Le prix doit être un nombre positif avec au maximum 2 décimales")]
        public string? PrixAchatString { get; set; }
        public double? PrixAchat { get; set; }

        public bool VoitureReparee { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateMiseEnVente { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Veuillez indiquer le prix de vente")]
        [RegularExpression(@"^\d+([.,]\d{1,2})?$", ErrorMessage = "Le prix doit être un nombre positif avec au maximum 2 décimales")]
        public string PrixMiseEnVenteString { get; set; }
        public double PrixMiseEnVente { get; set; }

        public bool AnnoncePubliee { get; set; }
        public bool VoitureVendue { get; set; }



        [Required(ErrorMessage = "Veuillez sélectionner une marque")]
        public int IdMarque { get; set; }
        public Marque? Marque { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un modèle")]
        public int IdModele { get; set; }
        public Modele? Modele { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner une finition")]
        public int IdFinition { get; set; }
        public Finition? Finition { get; set; }

        public Vente? Vente { get; set; }

        public List<Reparation> Reparations { get; set; } = new();
        public string? IdUtilisateur { get; set; }

        [NotMapped]
        public string NomComplet => $"{Annee} {Marque?.Nom} {Modele?.Nom} {Finition?.Nom}";


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
