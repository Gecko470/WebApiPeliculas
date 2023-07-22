using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPeliculas.DTOs;
using WebApiPeliculas.Helpers;
using WebApiPeliculas.Migrations;
using WebApiPeliculas.Models;
using WebApiPeliculas.Services;
using System.Linq.Dynamic.Core;

namespace WebApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController : CustomBaseController
    {
        private readonly AppDBContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ILogger<PeliculasController> logger;
        private readonly string contenedor = "peliculas";

        public PeliculasController(AppDBContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos, ILogger<PeliculasController> logger) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PeliculasIndexDTO>> Get()
        {
            var top = 5;
            var hoy = DateTime.Now;

            var proximosEstrenos = await context.Peliculas.Where(x => x.FechaEstreno > hoy).OrderBy(x => x.FechaEstreno).Take(top).ToListAsync();

            var enCines = await context.Peliculas.Where(x => x.EnCines).Take(5).ToListAsync();

            var resultado = new PeliculasIndexDTO();

            resultado.FuturosEstrenos = mapper.Map<List<PeliculaDTO>>(proximosEstrenos);
            resultado.EnCines = mapper.Map<List<PeliculaDTO>>(enCines);

            return resultado;
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDTO>>> FiltroPeliculas([FromQuery] FiltroPeliculasDTO filtroPeliculasDTO)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(filtroPeliculasDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculasDTO.Titulo));
            }

            if (filtroPeliculasDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);
            }

            if (filtroPeliculasDTO.ProximosEstrenos)
            {
                var hoy = DateTime.Now;
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > hoy);
            }

            if (filtroPeliculasDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.PeliculasGeneros.Select(x => x.GeneroId).Contains(filtroPeliculasDTO.GeneroId));
            }

            if (!string.IsNullOrEmpty(filtroPeliculasDTO.CampoOrdenar))
            {
                //PODEMOS HACERLO ASI

                //if (filtroPeliculasDTO.CampoOrdenar == "titulo")
                //{
                //    if (filtroPeliculasDTO.OrdenASC)
                //    {
                //        peliculasQueryable = peliculasQueryable.OrderBy(x => x.Titulo);
                //    }
                //    else
                //    {
                //        peliculasQueryable = peliculasQueryable.OrderByDescending(x => x.Titulo);
                //    }
                //}

                //if (filtroPeliculasDTO.CampoOrdenar == "titulo")
                //{
                //    if (filtroPeliculasDTO.OrdenASC)
                //    {
                //        peliculasQueryable = peliculasQueryable.OrderBy(x => x.Titulo);
                //    }
                //    else
                //    {
                //        peliculasQueryable = peliculasQueryable.OrderByDescending(x => x.Titulo);
                //    }
                //}

                //LO MISMO UTILIZANDO EL PAQUETE SYSTEM.LINQ.DYNAMIC.CORE

                var tipoOrden = filtroPeliculasDTO.OrdenASC ? "ascending" : "descending";

                try
                {
                    peliculasQueryable = peliculasQueryable.OrderBy($"{filtroPeliculasDTO.CampoOrdenar} {tipoOrden}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                }
            }

            await HttpContext.InsertarParametrosPaginacion(peliculasQueryable, filtroPeliculasDTO.CantidadRegistrosPorPagina);

            var peliculas = await peliculasQueryable.Paginar(filtroPeliculasDTO.Paginacion).ToListAsync();

            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpGet("{id:int}", Name = "PeliculaGetById")]
        public async Task<ActionResult<PeliculaDetallesDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas.Include(x => x.PeliculasGeneros).ThenInclude(x => x.Genero).Include(x => x.PeliculasActores).ThenInclude(x => x.Actor).FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula == null)
            {
                return NotFound();
            }

            pelicula.PeliculasActores = pelicula.PeliculasActores.OrderBy(x => x.Orden).ToList();

            return mapper.Map<PeliculaDetallesDTO>(pelicula);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var ms = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(ms);
                    var contenido = ms.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);
                    pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, peliculaCreacionDTO.Poster.ContentType);
                }
            }

            OrdenActores(pelicula);

            context.Add(pelicula);
            await context.SaveChangesAsync();

            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);

            return CreatedAtRoute("PeliculaGetById", new { id = pelicula.Id }, peliculaDTO);
        }

        private void OrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculasActores != null)
            {
                for (int i = 0; i < pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromForm] PeliculaCreacionDTO peliculaCreacionDTO, int id)
        {
            var pelicula = await context.Peliculas.Include(x => x.PeliculasGeneros).Include(x => x.PeliculasActores).FirstOrDefaultAsync(x => x.Id == id);


            if (pelicula == null)
            {
                return NotFound();
            }

            pelicula = mapper.Map(peliculaCreacionDTO, pelicula);

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var ms = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(ms);
                    var contenido = ms.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);
                    pelicula.Poster = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, peliculaCreacionDTO.Poster.ContentType, pelicula.Poster);
                }
            }

            OrdenActores(pelicula);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch([FromBody] JsonPatchDocument<PeliculaPatchDTO> patchDocument, int id)
        {
            //if (patchDocument == null)
            //{
            //    return BadRequest();
            //}

            //var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            //if (pelicula == null)
            //{
            //    return NotFound();
            //}

            //var peliculaPatchDTO = mapper.Map<PeliculaPatchDTO>(pelicula);

            //patchDocument.ApplyTo(peliculaPatchDTO, ModelState);

            //var esValido = TryValidateModel(peliculaPatchDTO);

            //if (!esValido)
            //{
            //    return BadRequest(ModelState);
            //}

            //mapper.Map(peliculaPatchDTO, pelicula);

            //await context.SaveChangesAsync();

            //return NoContent();


            //HEREDANDO DE CUSTOMBASECONTROLLER
            return await Patch<Pelicula, PeliculaPatchDTO>(patchDocument, id);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            //var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            //if (pelicula == null)
            //{
            //    return NotFound();
            //}

            //context.Peliculas.Remove(pelicula);
            //await context.SaveChangesAsync();

            //return NoContent();


            //HEREDANDO DE CUSTOMBASECONTROLLER
            return await Delete<Pelicula>(id);
        }
    }
}
