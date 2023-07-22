using System.ComponentModel.DataAnnotations;
using WebApiPeliculas.Validations;

namespace WebApiPeliculas.DTOs
{
    public class PeliculaPatchDTO
    {
        [Required]
        [StringLength(50)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
    }
}
