using Microsoft.AspNetCore.Mvc;
using Tienda.Models;
using Tienda.Repositorios;
using System.Collections.Generic;

namespace Tienda.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // La ruta base será /api/Presupuesto
    public class PresupuestosController : ControllerBase
    {
        private readonly PresupuestosRepository _presupuestosRepo;

        // ============================================================
        // CONSTRUCTOR
        // ------------------------------------------------------------
        // Se inicializa el repositorio para acceder a la base de datos.
        // ============================================================
        public PresupuestosController()
        {
            _presupuestosRepo = new PresupuestosRepository();
        }

        // ============================================================
        // POST /api/Presupuesto
        // ------------------------------------------------------------
        // Crea un nuevo presupuesto (sin detalles o con detalles incluidos).
        // Recibe un objeto Presupuesto en formato JSON.
        // ============================================================
        [HttpPost]
        public ActionResult CrearPresupuesto([FromBody] Presupuesto nuevoPresupuesto)
        {
            if (nuevoPresupuesto == null || string.IsNullOrEmpty(nuevoPresupuesto.NombreDestinatario))
            {
                return BadRequest("Debe ingresar un nombre de destinatario válido.");
            }

            int nuevoId = _presupuestosRepo.Crear(nuevoPresupuesto);
            return Ok($"Presupuesto creado correctamente con ID {nuevoId}");
        }

        // ============================================================
        // POST /api/Presupuesto/{id}/ProductoDetalle
        // ------------------------------------------------------------
        // Agrega un producto existente al presupuesto indicado.
        // Recibe por body el idProducto y la cantidad.
        // ============================================================
        [HttpPost("{id}/ProductoDetalle")]
        public ActionResult AgregarProductoAlPresupuesto(int id, [FromBody] PresupuestoDetalle nuevoDetalle)
        {
            if (nuevoDetalle == null || nuevoDetalle.Producto == null)
            {
                return BadRequest("Debe enviar un producto y cantidad válidos.");
            }

            _presupuestosRepo.AgregarProducto(id, nuevoDetalle.Producto.IdProducto, nuevoDetalle.Cantidad);
            return Ok($"Producto agregado al presupuesto con ID {id}");
        }

        // ============================================================
        // GET /api/Presupuesto/{id}
        // ------------------------------------------------------------
        // Obtiene los detalles completos de un presupuesto, incluyendo
        // los productos asociados y cantidades.
        // ============================================================
        [HttpGet("{id}")]
        public ActionResult<Presupuesto> ObtenerPresupuestoPorId(int id)
        {
            var presupuesto = _presupuestosRepo.ObtenerPorId(id);

            if (presupuesto == null)
                return NotFound($"No se encontró el presupuesto con ID {id}");

            return Ok(presupuesto);
        }

        // ============================================================
        // GET /api/Presupuesto
        // ------------------------------------------------------------
        // Lista todos los presupuestos existentes (sin detalles).
        // ============================================================
        [HttpGet]
        public ActionResult<List<Presupuesto>> ListarPresupuestos()
        {
            var lista = _presupuestosRepo.Listar();
            return Ok(lista);
        }

        // ============================================================
        // DELETE /api/Presupuesto/{id}
        // ------------------------------------------------------------
        // Elimina un presupuesto y todos sus detalles asociados.
        // ============================================================
        [HttpDelete("{id}")]
        public ActionResult EliminarPresupuesto(int id)
        {
            var presupuesto = _presupuestosRepo.ObtenerPorId(id);
            if (presupuesto == null)
                return NotFound($"No existe el presupuesto con ID {id}");

            _presupuestosRepo.Eliminar(id);
            return NoContent(); // HTTP 204: Eliminación exitosa sin contenido
        }
    }
}
