// Importamos los espacios de nombres necesarios
using System;                        // Para tipos básicos de .NET (ej: Convert, DateTime, etc.)
using System.Collections.Generic;    // Para poder usar List<T>
using Microsoft.Data.Sqlite;         // Librería que nos permite conectarnos a SQLite
using Tienda.Models;                 // Referencia a nuestras clases de modelo (Producto, etc.)

namespace Tienda.Repositorios
{
    // Esta clase implementa el PATRÓN REPOSITORIO.
    // Su función es actuar como intermediario entre la base de datos y el resto de la aplicación.
    // Así evitamos que los controladores trabajen directamente con SQL.
    public class ProductoRepository
    {
        // Cadena de conexión: indica dónde está la base de datos (archivo .db)
        // "Data Source=tienda.db" significa que el archivo está en la raíz del proyecto.
        private readonly string connectionString = "Data Source=tienda.db";

        // ================================================================
        // MÉTODO: Crear
        // ---------------------------------------------------------------
        // Inserta un nuevo producto en la base de datos.
        // Recibe un objeto Producto y guarda sus datos.
        // ================================================================
        public void Crear(Producto producto)
        {
            int nuevoId = 0; // Variable para guardar el ID que genera SQLite automáticamente

            // using: garantiza que la conexión se cierre automáticamente al salir del bloque
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open(); // Abrimos la conexión con la base de datos

                // Consulta SQL para insertar un nuevo registro
                // El comando "SELECT last_insert_rowid()" devuelve el último ID insertado (autoincremental)
                var query = "INSERT INTO Productos (Descripcion, Precio) VALUES (@descripcion, @precio); SELECT last_insert_rowID();";

                // Creamos el comando SQL que se ejecutará sobre la conexión
                using (var command = new SqliteCommand(query, connection))
                {
                    // Agregamos los parámetros a la consulta.
                    // Esto evita inyección SQL y convierte los datos al tipo correcto.
                    command.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@precio", producto.Precio);

                    // ExecuteScalar ejecuta la consulta y devuelve un único valor (en este caso, el nuevo ID)
                    nuevoId = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            // Nota: acá podrías asignar el ID al objeto producto (producto.IdProducto = nuevoId)
            // si quisieras usarlo después.
        }

        // ================================================================
        // MÉTODO: Modificar
        // ---------------------------------------------------------------
        // Actualiza los datos de un producto existente.
        // Recibe un ID (para saber cuál producto actualizar) y un objeto con los nuevos valores.
        // ================================================================
        public void Modificar(int id, Producto producto)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Consulta SQL de actualización
                var query = "UPDATE Productos SET Descripcion = @descripcion, Precio = @precio WHERE IdProducto = @id";

                using (var command = new SqliteCommand(query, connection))
                {
                    // Asignamos los parámetros con los nuevos valores
                    command.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@precio", producto.Precio);
                    command.Parameters.AddWithValue("@id", id);

                    // ExecuteNonQuery se usa cuando la consulta no devuelve datos (solo modifica)
                    command.ExecuteNonQuery();
                }
            }
        }

        // ================================================================
        // MÉTODO: Listar
        // ---------------------------------------------------------------
        // Devuelve una lista con todos los productos registrados.
        // Ideal para mostrar en una tabla o catálogo.
        // ================================================================
        public List<Producto> Listar()
        {
            var lista = new List<Producto>(); // Lista que devolveremos con todos los productos

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Consulta SQL para obtener todos los productos
                var query = "SELECT IdProducto, Descripcion, Precio FROM Productos";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader()) // reader nos permite leer fila por fila el resultado
                {
                    while (reader.Read()) // Mientras haya filas disponibles...
                    {
                        // Creamos un objeto Producto con los datos de cada fila
                        var producto = new Producto
                        {
                            IdProducto = reader.GetInt32(0),    // Columna 1 → IdProducto
                            Descripcion = reader.GetString(1),  // Columna 2 → Descripcion
                            Precio = reader.GetInt32(2)         // Columna 3 → Precio
                        };

                    // Por seguridad, usamos nombres de columnas en lugar de índices
                    // Esto evita errores si el orden de las columnas cambia.
                    // var producto = new Producto
                    // {
                    //     IdProducto = reader.GetInt32(reader.GetOrdinal("IdProducto")),
                    //     Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                    //     Precio = reader.GetInt32(reader.GetOrdinal("Precio"))
                    // };

                        // Lo agregamos a la lista
                        lista.Add(producto);
                    }
                }
            }

            // Devolvemos la lista completa
            return lista;
        }

        // ================================================================
        // MÉTODO: ObtenerPorId
        // ---------------------------------------------------------------
        // Busca un producto por su ID.
        // Devuelve el objeto Producto si lo encuentra, o null si no existe.
        // ================================================================
        public Producto ObtenerPorId(int id)
        {
            Producto producto = null; // Por defecto, asumimos que no lo encontramos

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT IdProducto, Descripcion, Precio FROM Productos WHERE IdProducto = @id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        // Si hay una fila, leemos los datos
                        if (reader.Read())
                        {
                            producto = new Producto
                            {
                                IdProducto = reader.GetInt32(0),
                                Descripcion = reader.GetString(1),
                                Precio = reader.GetInt32(2)
                            };
                        }
                    }
                }
            }

            // Si no se encontró nada, se devuelve null
            return producto;
        }

        // ================================================================
        // MÉTODO: Eliminar
        // ---------------------------------------------------------------
        // Borra un producto de la base de datos según su ID.
        // ================================================================
        public void Eliminar(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var query = "DELETE FROM Productos WHERE IdProducto = @id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery(); // No devuelve resultados, solo ejecuta la eliminación
                }
            }
        }
    }
}
