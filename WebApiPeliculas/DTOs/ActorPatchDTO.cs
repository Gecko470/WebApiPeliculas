using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.DTOs
{
    public class ActorPatchDTO
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}
