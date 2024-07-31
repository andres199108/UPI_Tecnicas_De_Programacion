using ProyectoVenta.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ClosedXML.Excel;
using System.Xml.Linq;

namespace ProyectoVenta.Datos
{
    public class DA_Venta
    {


        private readonly string filePath;

        public DA_Venta()
        {
            var cn = new Conexion("Ventas");
            filePath = cn.GetArchivoExcelPath();
        }

        public string Registrar(string venta_xml)
        {
            string respuesta = "";
            var cn = new Conexion("Ventas,Reportes");
            try
            {
                if (!File.Exists(filePath))
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Ventas");
                        worksheet.Cell(1, 1).Value = "NroDocumento";
                        worksheet.Cell(1, 2).Value = "VentaXML";
                        workbook.SaveAs(filePath);
                    }
                }

                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var lastRow = worksheet.LastRowUsed().RowNumber() + 1;

                    string nroDocumento = lastRow.ToString("D6"); // Generate a document number
                    worksheet.Cell(lastRow, 1).Value = nroDocumento;
                    worksheet.Cell(lastRow, 2).Value = venta_xml;

                    workbook.SaveAs(filePath);
                    respuesta = nroDocumento;
                }
            }
            catch (Exception ex)
            {
                respuesta = "";
            }

            return respuesta;
        }

        public Venta Detalle(string nrodocumento)
        {

            Venta? oVenta = new Venta();
            var cn = new Conexion("Ventas");
            try
            {
                if (!File.Exists(filePath))
                    return oVenta;

                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    foreach (var row in worksheet.RowsUsed())
                    {
                        if (row.RowNumber() == 1) continue; // Skip header row

                        if (row.Cell(1).GetValue<string>() == nrodocumento)
                        {
                            string ventaXml = row.Cell(2).GetValue<string>();
                            //oDocument doc = ODocument.Parse(ventaXml);
                            XDocument doc = XDocument.Parse(ventaXml);

                            if (doc.Element("Venta") != null)
                            {
                                var datosVenta = doc.Element("Venta");
                                oVenta = new Venta()
                                {
                                    TipoPago = datosVenta.Element("TipoPago").Value,
                                    NumeroDocumento = datosVenta.Element("NumeroDocumento").Value,
                                    DocumentoCliente = datosVenta.Element("DocumentoCliente").Value,
                                    NombreCliente = datosVenta.Element("NombreCliente").Value,
                                    MontoPagoCon = Convert.ToDecimal(datosVenta.Element("MontoPagoCon").Value, new CultureInfo("es-CR")),
                                    MontoCambio = Convert.ToDecimal(datosVenta.Element("MontoCambio").Value, new CultureInfo("es-CR")),
                                    MontoSubTotal = Convert.ToDecimal(datosVenta.Element("MontoSubTotal").Value.ToString(), new CultureInfo("es-CR")),
                                    MontoIGV = Convert.ToDecimal(datosVenta.Element("MontoIGV").Value.ToString(), new CultureInfo("es-CR")),
                                    MontoTotal = Convert.ToDecimal(datosVenta.Element("MontoTotal").Value.ToString(), new CultureInfo("es-CR")),
                                    oDetalleVenta = new List<Detalle_Venta>()
                                };

                                if (datosVenta.Element("Detalle_Venta") != null)
                                {

                                    foreach (var elemento in datosVenta.Element("Detalle_Venta").Elements())
                                    {
                                        Console.WriteLine(elemento.Name);
                                        var detalleVenta = new Detalle_Venta();
                                        detalleVenta.oProducto = new Producto()
                                        {
                                            Descripcion = elemento.Element("Descripcion") != null ? elemento.Element("Descripcion")!.Value : "Sin descripción"
                                        };
                                        detalleVenta.Cantidad = int.Parse(elemento.Element("Cantidad").Value);
                                        detalleVenta.PrecioVenta = decimal.Parse(elemento.Element("PrecioVenta").Value);
                                        detalleVenta.Total = decimal.Parse(elemento.Element("Total").Value);
                                        oVenta.oDetalleVenta.Add(detalleVenta);
                                    }
                                }

                            }


                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oVenta = new Venta();

            }

            return oVenta;
        }
    }
}



