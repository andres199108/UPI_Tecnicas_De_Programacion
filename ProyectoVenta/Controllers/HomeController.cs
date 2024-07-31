using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoVenta.Datos;
using ProyectoVenta.Models;
using System.Xml.Linq;





namespace ProyectoVenta.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        DA_Producto oProducto = new DA_Producto();
        DA_Venta _daVenta = new DA_Venta();
        DA_Producto _daproducto = new DA_Producto();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DetalleVenta()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public JsonResult AutoCompleteProducto(string search)
        {
            List<Autocomplete> autocomplete = oProducto.Listar()
                .Where(x => string.Concat(x.Codigo.ToUpper(), x.oCategoria.Descripcion.ToUpper(), x.Descripcion.ToUpper()).Contains(search.ToUpper()))
                .Select(m => new Autocomplete
                {
                    label = $"{m.Codigo} - {m.oCategoria.Descripcion} - {m.Descripcion}",
                    value = m.IdProducto
                }).ToList();

            return Json(autocomplete);
        }

        [HttpGet]
        public JsonResult ObtenerProducto(int idproducto)
        {
            Producto _daProducto = _daproducto.Listar().FirstOrDefault(x => x.IdProducto == idproducto);
            return Json(_daProducto);
        }

        [HttpPost]
        public JsonResult RegistrarVenta([FromBody] Venta body)
        {
            string rpta = "";
            {
                XElement venta = new XElement("Venta",
                    new XElement("TipoPago", body.TipoPago),
                    new XElement("NumeroDocumento", "0"),
                    new XElement("DocumentoCliente", body.DocumentoCliente),
                    new XElement("NombreCliente", body.NombreCliente),
                    new XElement("MontoPagoCon", body.MontoPagoCon),
                    new XElement("MontoCambio", body.MontoCambio),
                    new XElement("MontoSubTotal", body.MontoSubTotal),
                    new XElement("MontoIGV", body.MontoIGV),
                    new XElement("MontoTotal", body.MontoTotal)
                );
                XElement oDetalleVenta = new XElement("Detalle_Venta");
                foreach (Detalle_Venta item in body.oDetalleVenta)
                {
                    oDetalleVenta.Add(new XElement("Item",
                        new XElement("IdProducto", item.oProducto),
                        //new XElement("IdProducto", item.IdProducto),
                        new XElement("PrecioVenta", item.PrecioVenta),
                        new XElement("Cantidad", item.Cantidad),
                        new XElement("Total", item.Total)
                    ));
                }

                venta.Add(oDetalleVenta);
                rpta = _daVenta.Registrar(venta.ToString());

                return Json(new { respuesta = rpta });
            }
        }

        [HttpGet]
       public JsonResult ObtenerVenta(string nrodocumento)// estaba como publico y lo pasamos a privado
        {
            Venta oVenta = _daVenta.Detalle(nrodocumento);
            return Json(oVenta);
        }
    }
}
