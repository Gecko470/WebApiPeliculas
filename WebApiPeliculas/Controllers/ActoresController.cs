using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPeliculas.DTOs;
using WebApiPeliculas.Models;
using WebApiPeliculas.Services;

namespace WebApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController : CustomBaseController
    {
        private readonly AppDBContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";

        public ActoresController(AppDBContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacion)
        {
            //var actores = await context.Actores.ToListAsync();

            //return mapper.Map<List<ActorDTO>>(actores);


            //HEREDANDO DE CUSTOMBASECONTROLLER
            return await Get<Actor, ActorDTO>(paginacion);
        }

        [HttpGet("{id:int}", Name = "ActoresGetById")]
        public async Task<ActionResult<ActorDTO>> GetById(int id)
        {
            //var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            //if (actor == null)
            //{
            //    return NotFound();
            //}

            //return mapper.Map<ActorDTO>(actor);


            //HEREDANDO DE CUSTOMBASECONTROLLER
            return await Get<Actor, ActorDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = mapper.Map<Actor>(actorCreacionDTO);

            if (actorCreacionDTO.Foto != null)
            {
                using (var ms = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(ms);
                    var contenido = ms.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    actor.Foto = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, actorCreacionDTO.Foto.ContentType);
                }
            }

            context.Add(actor);
            await context.SaveChangesAsync();

            var actorDTO = mapper.Map<ActorDTO>(actor);

            return CreatedAtRoute("ActoresGetById", new { id = actor.Id }, actorDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromForm] ActorCreacionDTO actorCreacionDTO, int id)
        {
            //DE ESTA FORMA ACTUALIZAMOS SIEMPRE TODOS LOS CAMPOS

            //var actor = mapper.Map<Actor>(actorCreacionDTO);
            //actor.Id = id;

            //context.Entry(actor).State = EntityState.Modified;
            //TAMBIEN PODEMOS USAR context.update(actor);

            //await context.SaveChangesAsync();


            //DE ESTA FORMA ACTUALIZAMOS SOLO LOS CAMPOS QUE HAN CAMBIADO

            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            actor = mapper.Map(actorCreacionDTO, actor);

            if (actorCreacionDTO.Foto != null)
            {
                using (var ms = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(ms);
                    var contenido = ms.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    actor.Foto = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, actorCreacionDTO.Foto.ContentType, actor.Foto);
                }
            }

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch([FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument, int id)
        {
            //if (patchDocument == null)
            //{
            //    return BadRequest();
            //}

            //var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            //if (actor == null)
            //{
            //    return NotFound();
            //}

            //var actorDTO = mapper.Map<ActorPatchDTO>(actor);

            //patchDocument.ApplyTo(actorDTO, ModelState);

            //var esValido = TryValidateModel(actorDTO);

            //if (!esValido)
            //{
            //    return BadRequest(ModelState);
            //}

            //mapper.Map(actorDTO, actor);

            //await context.SaveChangesAsync();

            //return NoContent();


            //HEREDANDO DE CUSTOMBASECONTROLLER
            return await Patch<Actor, ActorPatchDTO>(patchDocument, id);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            //var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            //if (actor == null)
            //{
            //    return NotFound();
            //}

            //context.Actores.Remove(actor);
            //await context.SaveChangesAsync();

            //return NoContent();


            //HEREDANDO DE CUSTOMBASECONTROLLER
            return await Delete<Actor>(id);
        }
    }
}