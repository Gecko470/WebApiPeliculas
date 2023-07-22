using System.ComponentModel.DataAnnotations;
using WebApiPeliculas.Validations;

namespace WebApiPeliculas.DTOs
{
    public class ActorCreacionDTO
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }

        [PesoArchivoValidacion(4)]
        [TipoArchivoValidacion(GrupoTiposArchivo.Imagen)]
        public IFormFile Foto { get; set; }
    }
}
