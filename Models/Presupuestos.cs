using System;
using System.Collections.Generic;
using System.Linq;

namespace Tienda.Models
{
    public class Presupuesto
    {
        public int IdPresupuesto { get; set; }
        public string NombreDestinatario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<PresupuestoDetalle> Detalle { get; set; }

        public double MontoPresupuesto()
        {
            return Detalle.Sum(d => d.Subtotal);
        }

        public double MontoPresupuestoConIva()
        {
            return MontoPresupuesto() * 1.21;
        }

        public int CantidadProductos()
        {
            return Detalle.Sum(d => d.Cantidad);
        }
    }
}
