using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApiPeliculas.Helpers;
using WebApiPeliculas.Validations;

namespace WebApiPeliculas.DTOs
{
    public class PeliculaCreacionDTO
    {
        [Required]
        [StringLength(50)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        [PesoArchivoValidacion(4)]
        [TipoArchivoValidacion(GrupoTiposArchivo.Imagen)]
        public IFormFile Poster { get; set; }
        [ModelBinder(binderType: typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIds { get; set; }
        [ModelBinder(binderType: typeof(TypeBinder<List<ActorPeliculasCreacionDTO>>))]
        public List<ActorPeliculasCreacionDTO> Actores { get; set; }
    }
}
