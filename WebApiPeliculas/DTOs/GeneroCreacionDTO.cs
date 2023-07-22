using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.DTOs
{
    public class GeneroCreacionDTO
    {
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; }
    }
}
