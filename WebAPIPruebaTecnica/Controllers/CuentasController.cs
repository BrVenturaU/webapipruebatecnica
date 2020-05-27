using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIPruebaTecnica.Contexts;
using WebAPIPruebaTecnica.Entities;
using WebAPIPruebaTecnica.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace WebAPIPruebaTecnica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CuentasController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public CuentasController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        /// <summary>
        /// Accion del API que retorna un listado de cuentas.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CuentaDTO>>> Get()
        {
            var cuentas = await context.Cuentas.ToListAsync();
            var cuentasDTO = mapper.Map<List<CuentaDTO>>(cuentas);
            return cuentasDTO;
        }

        /// <summary>
        /// Accion del API que muestra un cliente seleccionado por su ID y sus respectivas cuentas.
        /// </summary>
        /// <param name="id">ID (identificador) del cliente para ser mostrado.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [HttpGet("MostrarCuentasCliente/{id}", Name = "MostrarCuentasCliente")]
        public async Task<ActionResult<ClienteDTO>> Get(int id)
        {
            var cliente = await context.Clientes.Include(x => x.CuentasCliente).FirstOrDefaultAsync(x => x.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }
            var clienteDTO = mapper.Map<ClienteDTO>(cliente);
            return clienteDTO;
        }

        /// <summary>
        /// Accion del API que guarda una cuenta en la base de datos.
        /// </summary>
        /// <param name="cuentaCreacion">Datos de la cuenta enviados por el cuerpo de la petición</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CuentaCreacionDTO cuentaCreacion)
        {
            var cuenta = mapper.Map<Cuenta>(cuentaCreacion);
            context.Cuentas.Add(cuenta);
            await context.SaveChangesAsync();
            var cuentaDTO = mapper.Map<CuentaDTO>(cuenta);
            return new CreatedAtRouteResult("MostrarCuentasCliente", new { id = cuenta.ClienteId }, cuentaDTO);
        }


        /// <summary>
        /// Accion de API que permite actualizar todos los datos de una cuenta.
        /// </summary>
        /// <param name="id">ID(identificador) de la cuenta que se desea actualizar</param>
        /// <param name="cuentaActualizacion">Datos de la cuenta enviados por el cuerpo de la petición</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CuentaCreacionDTO cuentaActualizacion)
        {

            var cuenta = mapper.Map<Cuenta>(cuentaActualizacion);
            cuenta.Id = id;

            context.Entry(cuenta).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Accion del API que permite actualizar parte de los datos una cuenta seleccionado por su ID.
        /// </summary>
        /// <param name="id">ID (identificador) de la cuenta para ser mostrado.</param>
        /// <param name="patchDocument">Elemento que identifica el/los datos a actualizar</param>
        /// <returns></returns>

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<CuentaCreacionDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }
            var cuentaDB = await context.Cuentas.FirstOrDefaultAsync(x => x.Id == id);
            if (cuentaDB == null)
            {
                return NotFound();
            }

            var cuentaDTO = mapper.Map<CuentaCreacionDTO>(cuentaDB);
            patchDocument.ApplyTo(cuentaDTO, ModelState);
            mapper.Map(cuentaDTO, cuentaDB);

            var isValid = TryValidateModel(cuentaDB);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        /* No deberia eliminarse una cuenta de la DB. Para mantener la integridad de los datos se recomienda un borrado logico*/
        /// <summary>
        /// Accion del API que permite eliminar una cuenta de la Base de datos
        /// </summary>
        /// <param name="id">ID(identificador) de la cuenta que se desea eliminar</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Cuenta>> Delete(int id)
        {
            var cuentaId = await context.Cuentas.Select(x => x.Id).FirstOrDefaultAsync(x => x == id);
            if (cuentaId == default(int))
            {
                return NotFound();
            }
            context.Cuentas.Remove(new Cuenta { Id = cuentaId });
            await context.SaveChangesAsync();
            //var clienteDTO = mapper.Map<ClienteDTO>(cliente);
            return NoContent();
        }
    }
}
