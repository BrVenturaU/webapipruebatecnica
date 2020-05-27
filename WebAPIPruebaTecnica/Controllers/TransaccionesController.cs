using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebAPIPruebaTecnica.Contexts;
using WebAPIPruebaTecnica.Entities;
using WebAPIPruebaTecnica.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly:ApiConventionType(typeof(DefaultApiConventions))]
namespace WebAPIPruebaTecnica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TransaccionesController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public TransaccionesController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        /// <summary>
        /// Accion del API que genera un listado de transacciones registradas.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpGet("ListaTransacciones")]
        public async Task<ActionResult<IEnumerable<TransaccionDTO>>> Get()
        {
            var transacciones = await context.Transacciones.ToListAsync();
            var transaccionesDTO = mapper.Map<List<TransaccionDTO>>(transacciones);
            return transaccionesDTO;
        }
        /// <summary>
        /// Accion del API que muestra una transaccion seleccionada por su ID.
        /// </summary>
        /// <param name="id">ID (identificador) de la transacción para ser mostrada.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [HttpGet("MostrarTransaccion/{id}", Name = "MostrarTransaccion")]
        public async Task<ActionResult<TransaccionDTO>> Get(int id)
        {
            var transaccion = await context.Transacciones.FirstOrDefaultAsync(x => x.Id == id);
            if (transaccion == null)
            {
                return NotFound();
            }
            var transaccionDTO = mapper.Map<TransaccionDTO>(transaccion);
            return transaccionDTO;
        }

        /// <summary>
        /// Accion del API que permite registrar los datos de una lista de transacciones.
        /// </summary>
        /// <param name="transaccionesCreacion">Datos de las transacciones enviados por el cuerpo de la petición.</param>
        /// <returns></returns>
        [HttpPost]
        [HttpPost("Sincronizar")]
        public async Task<ActionResult> Post([FromBody] TransaccionCreacionDTO[] transaccionesCreacion)
        {
            int numTransacciones = transaccionesCreacion.Count();            
            if (numTransacciones > 0)
            {
                foreach(TransaccionCreacionDTO creacionDTO in transaccionesCreacion)
                {
                    
                    var transaccion = mapper.Map<Transaccion>(creacionDTO);
                    context.Transacciones.Add(transaccion);
                    await context.SaveChangesAsync();
                }
                return Created(this.RouteData.ToString(),"Sincronizacion Correcta");
                //var transaccionDTO = mapper.Map<TransaccionDTO>(transaccion);
                //return new CreatedAtRouteResult("MostrarTransaccion", new { id = transaccion.Id }, transaccionDTO);

            }
            else
            {
                return BadRequest("Error al Sincronizar");
            }
        }

        /// <summary>
        /// Accion del API que permite actualizar todos los datos una transaccion seleccionada por su ID.
        /// </summary>
        /// <param name="id">ID (identificador) de la transaccion que se requiere actualizar.</param>
        /// <param name="transaccionActualizacion">Datos de la transaccion enviados por el cuerpo de la petición</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] TransaccionCreacionDTO transaccionActualizacion)
        {

            var transaccion = mapper.Map<Transaccion>(transaccionActualizacion);
            transaccion.Id = id;

            context.Entry(transaccion).State = EntityState.Modified;

            var saved = false;
            while (!saved)
            {
                try
                {
                    // Attempt to save changes to the database
                    await context.SaveChangesAsync();
                    saved = true;
                    return NoContent();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is Transaccion)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            foreach (var property in proposedValues.Properties)
                            {
                                var proposedValue = proposedValues[property];
                                var databaseValue = databaseValues[property];

                                // TODO: decide which value should be written to database
                                // proposedValues[property] = <value to be saved>;
                            }

                            // Refresh original values to bypass next concurrency check
                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for "
                                + entry.Metadata.Name);
                        }
                    }
                }
            }
            return NotFound();
        }

        /// <summary>
        /// Accion del API que permite actualizar parte de los datos una transaccion seleccionado por su ID.
        /// </summary>
        /// <param name="id">ID (identificador) de la transaccion para ser mostrado.</param>
        /// <param name="patchDocument">Elemento que identifica el/los datos a actualizar</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<TransaccionCreacionDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }
            var transaccionDB = await context.Transacciones.FirstOrDefaultAsync(x => x.Id == id);
            if (transaccionDB == null)
            {
                return NotFound();
            }

            var transaccionDTO = mapper.Map<TransaccionCreacionDTO>(transaccionDB);
            patchDocument.ApplyTo(transaccionDTO, ModelState);
            mapper.Map(transaccionDTO, transaccionDB);

            var isValid = TryValidateModel(transaccionDB);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        /* No deberia eliminarse una transaccion de la DB. Para mantener la integridad de los datos se recomienda un borrado logico*/
        /// <summary>
        /// Accion del API que permite eliminar el registro de la transaccion
        /// </summary>
        /// <param name="id">ID (identificador) de la transaccion que se requiere eliminar.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Transaccion>> Delete(int id)
        {
            var transaccionId = await context.Transacciones.Select(x => x.Id).FirstOrDefaultAsync(x => x == id);
            if (transaccionId == default(int))
            {
                return NotFound();
            }
            context.Transacciones.Remove(new Transaccion { Id = transaccionId });
            var saved = false;
            while (!saved)
            {
                try
                {
                    // Attempt to save changes to the database
                    await context.SaveChangesAsync();
                    saved = true;
                    return NoContent();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is Transaccion)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            foreach (var property in proposedValues.Properties)
                            {
                                var proposedValue = proposedValues[property];
                                var databaseValue = databaseValues[property];

                                // TODO: decide which value should be written to database
                                // proposedValues[property] = <value to be saved>;
                            }

                            // Refresh original values to bypass next concurrency check
                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for "
                                + entry.Metadata.Name);
                        }
                    }
                }
            }
            return BadRequest();
        }
    }
}
