namespace Tienda.Models
{
    public class PresupuestoDetalle
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }

        public int Subtotal
        {
            get { return Producto.Precio * Cantidad; }
        }
    }
}
