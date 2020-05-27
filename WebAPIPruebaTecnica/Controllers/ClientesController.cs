using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace WebAPIPruebaTecnica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public ClientesController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        /// <summary>
        /// Accion del API que genera un listado de clientes registrados y sus cuentas.
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListaClientesYCuentas")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> Get()
        {
            var clientes = await context.Clientes.Include(x => x.CuentasCliente).ToListAsync();
            var clientesDTO = mapper.Map<List<ClienteDTO>>(clientes);
            return clientesDTO;
        }

        /// <summary>
        /// Accion del API que muestra un cliente seleccionado por su ID y sus respectivas cuentas.
        /// </summary>
        /// <param name="id">ID (identificador) del cliente para ser mostrado.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [HttpGet("MostrarCuentasPorCliente/{id}", Name ="MostrarCuentasPorCliente")]
        public async Task<ActionResult<ClienteDTO>>Get(int id)
        {
            var cliente = await context.Clientes.Include(x => x.CuentasCliente).FirstOrDefaultAsync(x => x.Id == id);
            if(cliente == null)
            {
                return NotFound();
            }
            var clienteDTO = mapper.Map<ClienteDTO>(cliente);
            return clienteDTO;
        }

        /// <summary>
        /// Accion del API que permite registrar los datos de un cliente.
        /// </summary>
        /// <param name="clienteCreacion">Datos del cliente enviados por el cuerpo de la petición.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ClienteCreacionDTO clienteCreacion)
        {
            var cliente = mapper.Map<Cliente>(clienteCreacion);
            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();
            var clienteDTO = mapper.Map<ClienteDTO>(cliente);
            return new CreatedAtRouteResult("MostrarCuentasPorCliente", new { id = cliente.Id }, clienteDTO);
        }

        /// <summary>
        /// Accion del API que permite actualizar todos los datos un cliente seleccionado por su ID.
        /// </summary>
        /// <param name="id">ID (identificador) del cliente que se requiere actualizar.</param>
        /// <param name="clienteActualizacion">Datos del cliente enviados por el cuerpo de la petición</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ClienteCreacionDTO clienteActualizacion)
        {

            var cliente = mapper.Map<Cliente>(clienteActualizacion);
            cliente.Id = id;
            byte[] timeStamp = GetTimestamp(DateTime.Now);
            cliente.TimeStamp = timeStamp;

            context.Entry(cliente).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        public static byte[] GetTimestamp(DateTime value)
        {
            byte[] bytes =Encoding.ASCII.GetBytes(value.ToString("yyyyMMddHHmmssffff"));
            return bytes;
        }

        /// <summary>
        /// Accion del API que permite actualizar parte de los datos un cliente seleccionado por su ID.
        /// </summary>
        /// <param name="id">ID (identificador) del cliente para ser mostrado.</param>
        /// <param name="patchDocument">Elemento que identifica el/los datos a actualizar</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ClienteCreacionDTO> patchDocument)
        {
            if(patchDocument == null)
            {
                return BadRequest();
            }
            var clienteDB = await context.Clientes.FirstOrDefaultAsync(x => x.Id == id);
            if(clienteDB == null)
            {
                return NotFound();
            }

            var clienteDTO = mapper.Map<ClienteCreacionDTO>(clienteDB);
            patchDocument.ApplyTo(clienteDTO, ModelState);
            mapper.Map(clienteDTO, clienteDB);

            var isValid = TryValidateModel(clienteDB);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        /* No deberia eliminarse un cliente de la DB. Para mantener la integridad de los datos se recomienda un borrado logico*/
        /// <summary>
        /// Accion del API que permite eliminar el registro del cliente
        /// </summary>
        /// <param name="id">ID (identificador) del cliente que se requiere eliminar.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Cliente>> Delete(int id)
        {
            var clienteId = await context.Clientes.Select(x=>x.Id).FirstOrDefaultAsync(x => x == id);
            if(clienteId == default(int))
            {
                return NotFound();
            }
            context.Clientes.Remove(new Cliente { Id = clienteId });
            await context.SaveChangesAsync();
            //var clienteDTO = mapper.Map<ClienteDTO>(cliente);
            return NoContent();
        }
    }
}
