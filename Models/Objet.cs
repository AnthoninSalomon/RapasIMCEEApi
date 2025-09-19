using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RapasIMCEEApi.Models
{
    [Table("Objet")]
    public class Objet
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Libelle { get; set; }

        public ICollection<ObjetData> ObjetDatas { get; set; }
    }
}
