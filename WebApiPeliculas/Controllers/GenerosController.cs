using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPeliculas.DTOs;
using WebApiPeliculas.Models;

namespace WebApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController : CustomBaseController
    {
        //private readonly AppDBContext context;
        //private readonly IMapper mapper;

        public GenerosController(AppDBContext context, IMapper mapper) : base(context, mapper)
        {
            //this.context = context;
            //this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> Get()
        {
            //var generos = await context.Generos.ToListAsync();

            //return mapper.Map<List<GeneroDTO>>(generos);


            //HEREDANDO DEL CUSTOMBASECONTROLLER
            return await Get<Genero, GeneroDTO>();
        }

        [HttpGet("{id:int}", Name = "GenerosGetById")]
        public async Task<ActionResult<GeneroDTO>> GetById(int id)
        {
            //var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            //if (genero == null)
            //{
            //    return NotFound();
            //}

            //return mapper.Map<GeneroDTO>(genero);


            //HEREDANDO DEL CUSTOMBASECONTROLLER
            return await Get<Genero, GeneroDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult<GeneroDTO>> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            //var genero = mapper.Map<Genero>(generoCreacionDTO);

            //context.Generos.Add(genero);
            //await context.SaveChangesAsync();

            //var generoDTO = mapper.Map<GeneroDTO>(genero);

            //return CreatedAtRoute("GenerosGetById", new { id = generoDTO.Id }, generoDTO);


            //HEREDANDO DEL CUSTOMBASECONTROLLER
            return await Post<GeneroCreacionDTO, Genero, GeneroDTO>(generoCreacionDTO, "GenerosGetById");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<GeneroDTO>> Put([FromBody] GeneroCreacionDTO generoCreacionDTO, int id)
        {
            //var genero = mapper.Map<Genero>(generoCreacionDTO);
            //genero.Id = id;
            //context.Entry(genero).State = EntityState.Modified;

            //await context.SaveChangesAsync();

            //return NoContent();

            //HEREDANDO DEL CUSTOMBASECONTROLLER
            return await Put<GeneroCreacionDTO, Genero>(generoCreacionDTO, id);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            //var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            //if (genero == null)
            //{
            //    return NotFound();
            //}

            //context.Generos.Remove(genero);
            //await context.SaveChangesAsync();

            //return NoContent();


            //HEREDANDO DEL CUSTOMBASECONTROLLER
            return await Delete<Genero>(id);
        }
    }
}
