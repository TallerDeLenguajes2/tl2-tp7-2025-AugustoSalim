using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Tienda.Models;
using Tienda.Repositorios;

namespace Tienda.Controllers
{
    // Indica que esta clase es un controlador de tipo API
    [ApiController]

    // Define la ruta base de los endpoints de este controlador.
    // Ejemplo: si el controlador se llama "ProductoController", la ruta será "/api/Producto"
    [Route("api/[controller]")]

    public class ProductoController : ControllerBase
    {
        // Repositorio que usaremos para acceder a la base de datos
        private ProductoRepository productoRepository;

        // Constructor: se ejecuta al crear el controlador.
        // Instanciamos el repositorio para poder usarlo en los métodos.
        public ProductoController()
        {
            productoRepository = new ProductoRepository();
        }

        // ================================================================
        // ENDPOINT: POST /api/Producto
        // ---------------------------------------------------------------
        // Crea un nuevo producto en la base de datos.
        // El objeto Producto llega en formato JSON en el cuerpo del request.
        // Ejemplo de JSON a enviar desde Postman:
        // { "descripcion": "Mouse inalámbrico", "precio": 3500 }
        // ================================================================
        [HttpPost]
        public ActionResult<string> CrearProducto([FromBody] Producto nuevoProducto)
        {
            // Validamos datos básicos
            if (nuevoProducto == null || string.IsNullOrWhiteSpace(nuevoProducto.Descripcion))
            {
                return BadRequest("Debe ingresar una descripción y un precio válidos.");
            }

            productoRepository.Crear(nuevoProducto);
            return Ok("✅ Producto creado correctamente.");
        }

        // ================================================================
        // ENDPOINT: PUT /api/Producto/{id}
        // ---------------------------------------------------------------
        // Modifica un producto existente según su ID.
        // El ID se recibe por parámetro en la URL y el nuevo producto en el cuerpo.
        // Ejemplo:
        // PUT /api/Producto/3
        // { "descripcion": "Teclado mecánico", "precio": 9500 }
        // ================================================================
        [HttpPut("{id}")]
        public ActionResult<string> ModificarProducto(int id, [FromBody] Producto productoActualizado)
        {
            var existente = productoRepository.ObtenerPorId(id);

            if (existente == null)
            {
                return NotFound($"No se encontró el producto con ID {id}.");
            }

            productoRepository.Modificar(id, productoActualizado);
            return Ok("✅ Producto modificado correctamente.");
        }

        // ================================================================
        // ENDPOINT: GET /api/Producto
        // ---------------------------------------------------------------
        // Devuelve la lista de todos los productos en la base de datos.
        // ================================================================
        [HttpGet]
        public ActionResult<List<Producto>> ListarProductos()
        {
            var lista = productoRepository.Listar();
            return Ok(lista); // Devuelve un JSON con todos los productos
        }

        // ================================================================
        // ENDPOINT: GET /api/Producto/{id}
        // ---------------------------------------------------------------
        // Devuelve un producto específico según su ID.
        // ================================================================
        [HttpGet("{id}")]
        public ActionResult<Producto> ObtenerProductoPorId(int id)
        {
            var producto = productoRepository.ObtenerPorId(id);

            if (producto == null)
            {
                return NotFound($"No se encontró el producto con ID {id}.");
            }

            return Ok(producto);
        }

        // ================================================================
        // ENDPOINT: DELETE /api/Producto/{id}
        // ---------------------------------------------------------------
        // Elimina un producto existente por su ID.
        // ================================================================
        [HttpDelete("{id}")]
        public ActionResult EliminarProducto(int id)
        {
            var producto = productoRepository.ObtenerPorId(id);

            if (producto == null)
            {
                return NotFound($"No se encontró el producto con ID {id} para eliminar.");
            }

            productoRepository.Eliminar(id);
            return NoContent(); // HTTP 204 → Eliminación exitosa sin contenido
        }
    }
}
