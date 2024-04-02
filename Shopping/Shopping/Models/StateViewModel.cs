using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class StateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Departamento/Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorío.")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener maximo {1} caractéres.")]
        public string Name { get; set; }

        public int CountryId { get; set; }
    }
}
