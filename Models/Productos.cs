namespace Tienda.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int Precio { get; set; }
    }
}
