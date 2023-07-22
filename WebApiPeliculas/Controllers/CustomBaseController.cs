using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.XPath;
using WebApiPeliculas.DTOs;
using WebApiPeliculas.Helpers;
using WebApiPeliculas.Models;
using WebApiPeliculas.Services;

namespace WebApiPeliculas.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly AppDBContext context;
        private readonly IMapper mapper;

        public CustomBaseController(AppDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class
        {
            var entidades = await context.Set<TEntidad>().AsNoTracking().ToListAsync();
            var dtos = mapper.Map<List<TDTO>>(entidades);

            return dtos;
        }

        protected async Task<ActionResult<List<TLecturaDTO>>> Get<TEntidad, TLecturaDTO>(PaginacionDTO paginacion) where TEntidad : class
        {
            var queryable = context.Set<TEntidad>().AsQueryable();
            await HttpContext.InsertarParametrosPaginacion(queryable, paginacion.CantidadRegistrosPorPagina);
            var entidades = await queryable.Paginar(paginacion).ToListAsync();

            return mapper.Map<List<TLecturaDTO>>(entidades);
        }

        protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id) where TEntidad : class, IId
        {
            var entidad = await context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            return mapper.Map<TDTO>(entidad);
        }

        protected async Task<ActionResult<TLecturaDTO>> Post<TCreacionDTO, TEntidad, TLecturaDTO>(TCreacionDTO creacionDTO, string ruta) where TEntidad : class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacionDTO);

            context.Set<TEntidad>().Add(entidad);
            await context.SaveChangesAsync();

            var dto = mapper.Map<TLecturaDTO>(entidad);

            return CreatedAtRoute(ruta, new { id = entidad.Id }, dto);
        }

        protected async Task<ActionResult> Put<TCreacionDTO, TEntidad>(TCreacionDTO creacionDTO, int id) where TEntidad : class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacionDTO);
            entidad.Id = id;
            context.Entry(entidad).State = EntityState.Modified;

            await context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad : class, IId
        {
            var entidad = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            context.Set<TEntidad>().Remove(entidad);
            await context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntidad, TPatchDTO>(JsonPatchDocument<TPatchDTO> patchDocument, int id) where TEntidad : class, IId where TPatchDTO : class
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entidad = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<TPatchDTO>(entidad);

            patchDocument.ApplyTo(dto, ModelState);

            var esValido = TryValidateModel(dto);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(dto, entidad);

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
