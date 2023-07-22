using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.Models
{
    public class Genero : IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; }
        public List<PeliculasGeneros> PeliculasGeneros { get; set; }
    }
}
