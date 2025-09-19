using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RapasIMCEEApi.Models
{
    [Table("Utilisateur")]
    public class Utilisateur
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Libelle { get; set; }

        [Required]
        public string Password { get; set; } // le contenu est chiffré (base64)
    }
}
