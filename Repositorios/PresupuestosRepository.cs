using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Tienda.Models;

namespace Tienda.Repositorios
{
    public class PresupuestosRepository
    {
        private readonly string connectionString = "Data Source=tienda.db";

        // ============================================================
        // MÉTODO: Crear un nuevo presupuesto
        // ------------------------------------------------------------
        // Inserta un presupuesto nuevo y devuelve su Id generado.
        // ============================================================
        public int Crear(Presupuesto presupuesto)
        {
            int nuevoId = 0;

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Insertamos el presupuesto (sin detalles todavía)
                var query = @"INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion)
                              VALUES (@nombre, @fecha);
                              SELECT last_insert_rowid();";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombre", presupuesto.NombreDestinatario);
                    command.Parameters.AddWithValue("@fecha", presupuesto.FechaCreacion.ToString("yyyy-MM-dd HH:mm:ss"));

                    // Obtenemos el ID autogenerado
                    nuevoId = Convert.ToInt32(command.ExecuteScalar());
                }

                // Si el presupuesto tiene detalles, los agregamos
                if (presupuesto.Detalle != null && presupuesto.Detalle.Count > 0)
                {
                    foreach (var item in presupuesto.Detalle)
                    {
                        var queryDetalle = @"INSERT INTO PresupuestoDetalle (IdPresupuesto, IdProducto, Cantidad)
                                             VALUES (@idPresupuesto, @idProducto, @cantidad)";
                        using (var commandDetalle = new SqliteCommand(queryDetalle, connection))
                        {
                            commandDetalle.Parameters.AddWithValue("@idPresupuesto", nuevoId);
                            commandDetalle.Parameters.AddWithValue("@idProducto", item.Producto.IdProducto);
                            commandDetalle.Parameters.AddWithValue("@cantidad", item.Cantidad);
                            commandDetalle.ExecuteNonQuery();
                        }
                    }
                }
            }

            return nuevoId;
        }

        // ============================================================
        // MÉTODO: Listar todos los presupuestos (sin detalles)
        // ------------------------------------------------------------
        // Devuelve una lista con los presupuestos básicos.
        // ============================================================
        public List<Presupuesto> Listar()
        {
            var lista = new List<Presupuesto>();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT IdPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var presupuesto = new Presupuesto
                        {
                            IdPresupuesto = reader.GetInt32(0),
                            NombreDestinatario = reader.GetString(1),
                            FechaCreacion = DateTime.Parse(reader.GetString(2))
                        };

                        lista.Add(presupuesto);
                    }
                }
            }

            return lista;
        }

        // ============================================================
        // MÉTODO: Obtener presupuesto por ID (con sus productos)
        // ------------------------------------------------------------
        // Devuelve un presupuesto completo con su lista de detalles.
        // ============================================================
        public Presupuesto ObtenerPorId(int id)
        {
            Presupuesto presupuesto = null;

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Obtenemos el presupuesto base
                var query = @"SELECT IdPresupuesto, NombreDestinatario, FechaCreacion 
                              FROM Presupuestos WHERE IdPresupuesto = @id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            presupuesto = new Presupuesto
                            {
                                IdPresupuesto = reader.GetInt32(0),
                                NombreDestinatario = reader.GetString(1),
                                FechaCreacion = DateTime.Parse(reader.GetString(2)),
                                Detalle = new List<PresupuestoDetalle>()
                            };
                        }
                    }
                }

                // Si no existe, devolvemos null
                if (presupuesto == null) return null;

                // Obtenemos los productos asociados a ese presupuesto
                var queryDetalle = @"SELECT pd.IdProducto, p.Descripcion, p.Precio, pd.Cantidad
                                     FROM PresupuestoDetalle pd
                                     JOIN Productos p ON pd.IdProducto = p.IdProducto
                                     WHERE pd.IdPresupuesto = @idPresupuesto";

                using (var commandDetalle = new SqliteCommand(queryDetalle, connection))
                {
                    commandDetalle.Parameters.AddWithValue("@idPresupuesto", id);

                    using (var reader = commandDetalle.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var producto = new Producto
                            {
                                IdProducto = reader.GetInt32(0),
                                Descripcion = reader.GetString(1),
                                Precio = reader.GetInt32(2)
                            };

                            var detalle = new PresupuestoDetalle
                            {
                                Producto = producto,
                                Cantidad = reader.GetInt32(3)
                            };

                            presupuesto.Detalle.Add(detalle);
                        }
                    }
                }
            }

            return presupuesto;
        }

        // ============================================================
        // MÉTODO: Agregar un producto a un presupuesto existente
        // ------------------------------------------------------------
        // Inserta una nueva línea de detalle.
        // ============================================================
        public void AgregarProducto(int idPresupuesto, int idProducto, int cantidad)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var query = @"INSERT INTO PresupuestoDetalle (IdPresupuesto, IdProducto, Cantidad)
                              VALUES (@idPresupuesto, @idProducto, @cantidad)";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                    command.Parameters.AddWithValue("@idProducto", idProducto);
                    command.Parameters.AddWithValue("@cantidad", cantidad);
                    command.ExecuteNonQuery();
                }
            }
        }

        // ============================================================
        // MÉTODO: Eliminar un presupuesto por ID
        // ------------------------------------------------------------
        // Borra tanto los detalles como el presupuesto en sí.
        // ============================================================
        public void Eliminar(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Primero eliminamos los detalles asociados (por FK)
                var queryDetalle = "DELETE FROM PresupuestoDetalle WHERE IdPresupuesto = @id";
                using (var commandDetalle = new SqliteCommand(queryDetalle, connection))
                {
                    commandDetalle.Parameters.AddWithValue("@id", id);
                    commandDetalle.ExecuteNonQuery();
                }

                // Luego eliminamos el presupuesto principal
                var queryPresupuesto = "DELETE FROM Presupuestos WHERE IdPresupuesto = @id";
                using (var commandPresupuesto = new SqliteCommand(queryPresupuesto, connection))
                {
                    commandPresupuesto.Parameters.AddWithValue("@id", id);
                    commandPresupuesto.ExecuteNonQuery();
                }
            }
        }
    }
}
