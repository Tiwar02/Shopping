using Mono.TextTemplating;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shopping.Data.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "El campo {0} es obligatorío.")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener maximo {1} caractéres.")]
        public string Name { get; set; }

        [JsonIgnore]
        public State State { get; set; }

        public ICollection<User> Users { get; set; }

    }
}
