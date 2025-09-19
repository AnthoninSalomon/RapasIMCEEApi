using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RapasIMCEEApi.Models
{
    [Table("ObjetData")]
    public class ObjetData
    {
        [Key]
        public long Id { get; set; }

        [Column("Object")]
        public long ObjectId { get; set; }         // FK vers Objet(Id)

        // date/time : on peut stocker DateOnly/TimeOnly avec converters (expliqué ci-dessous)
        public DateOnly? DateObservation { get; set; }
        public TimeOnly? UTCHHMNSS { get; set; }

        public double? MJD { get; set; }
        public double? RADegDec { get; set; }
        public double? DecDegDec { get; set; }
        public double? MagnitudeAG { get; set; }
        public double? IncertitudeAG { get; set; }
        public double? MagnitudeBGbp { get; set; }
        public double? IncertitudeBGbp { get; set; }
        public double? MagnitudeCGrp { get; set; }
        public double? IncertitudeCGrp { get; set; }
        public double? IndiceBC { get; set; }
        public double? UpperLimitG { get; set; }

        [Column("Utilisateur")]
        public long? UtilisateurId { get; set; }

        public string? Commentaire { get; set; }
        public double? ChampCouvert { get; set; }

        [ForeignKey("ObjectId")]
        public Objet Objet { get; set; }

        [ForeignKey("UtilisateurId")]
        public Utilisateur Utilisateur { get; set; }
    }
}
