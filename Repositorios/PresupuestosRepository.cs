using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Tienda.Models;

namespace Tienda.Repositorios
{
    public class ProductoRepository
    {
        private readonly string connectionString = "Data Source=tienda.db";

        // 🔹 Crear un nuevo producto
        public void Crear(Producto producto)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var query = "INSERT INTO Productos (Descripcion, Precio) VALUES (@descripcion, @precio)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@precio", producto.Precio);
                    command.ExecuteNonQuery();
                }
            }
        }

        // 🔹 Modificar un producto existente
        public void Modificar(int id, Producto producto)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var query = "UPDATE Productos SET Descripcion = @descripcion, Precio = @precio WHERE IdProducto = @id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@precio", producto.Precio);
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        // 🔹 Listar todos los productos
        public List<Producto> Listar()
        {
            var lista = new List<Producto>();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT IdProducto, Descripcion, Precio FROM Productos";
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var producto = new Producto
                        {
                            IdProducto = reader.GetInt32(0),
                            Descripcion = reader.GetString(1),
                            Precio = reader.GetInt32(2)
                        };
                        lista.Add(producto);
                    }
                }
            }

            return lista;
        }

        // 🔹 Obtener un producto por ID
        public Producto ObtenerPorId(int id)
        {
            Producto producto = null;

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT IdProducto, Descripcion, Precio FROM Productos WHERE IdProducto = @id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
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

            return producto;
        }

        // 🔹 Eliminar un producto por ID
        public void Eliminar(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var query = "DELETE FROM Productos WHERE IdProducto = @id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
