using ClosedXML.Excel;
using ProyectoVenta.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ProyectoVenta.Datos
{
    public class DA_Producto
    {
        private readonly string filePath;

        public DA_Producto()
            {
            var cn = new Conexion("Productos");
            filePath = cn.GetArchivoExcelPath();
            }

        public List<Producto> Listar()
        {
            var oLista = new List<Producto>();

            

            if (!File.Exists(filePath))
                return oLista;

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed();

                foreach (var row in rows)
                {
                    if (row.RowNumber() == 1) continue; // Skip header row

                    oLista.Add(new Producto()
                    {
                        IdProducto = row.Cell(1).GetValue<int>(),
                        Codigo = row.Cell(2).GetValue<string>(),
                        oCategoria = new Categoria()
                        {
                            IdCategoria = row.Cell(3).GetValue<int>(),
                            Descripcion = row.Cell(4).GetValue<string>()
                        },
                        Descripcion = row.Cell(5).GetValue<string>(),
                        PrecioCompra = row.Cell(6).GetValue<decimal>(),
                        PrecioVenta = row.Cell(7).GetValue<decimal>(),
                        Stock = row.Cell(8).GetValue<int>()
                    });
                }
            }

            return oLista;
        }

        public bool Guardar(Producto obj)
        {
            bool respuesta = false;

            try
            {
                var workbook = File.Exists(filePath) ? new XLWorkbook(filePath) : new XLWorkbook();
                var worksheet = workbook.Worksheets.Count == 0 ? workbook.AddWorksheet("Productos") : workbook.Worksheet(1);

                if (worksheet.RowsUsed().Count() == 0)
                {
                    worksheet.Cell(1, 1).Value = "IdProducto";
                    worksheet.Cell(1, 2).Value = "Codigo";
                    worksheet.Cell(1, 3).Value = "IdCategoria";
                    worksheet.Cell(1, 4).Value = "Descripcion";
                    worksheet.Cell(1, 5).Value = "Descripcion";
                    worksheet.Cell(1, 6).Value = "PrecioCompra";
                    worksheet.Cell(1, 7).Value = "PrecioVenta";
                    worksheet.Cell(1, 8).Value = "Stock";
                }

                var lastRow = worksheet.LastRowUsed().RowNumber() + 1;

                worksheet.Cell(lastRow, 1).Value = lastRow - 1; // Assuming IdProducto is auto-incremented
                worksheet.Cell(lastRow, 2).Value = obj.Codigo;
                worksheet.Cell(lastRow, 3).Value = obj.oCategoria.IdCategoria;
                worksheet.Cell(lastRow, 4).Value = obj.oCategoria.Descripcion;
                worksheet.Cell(lastRow, 5).Value = obj.Descripcion;
                worksheet.Cell(lastRow, 6).Value = obj.PrecioCompra;
                worksheet.Cell(lastRow, 7).Value = obj.PrecioVenta;
                worksheet.Cell(lastRow, 8).Value = obj.Stock;

                workbook.SaveAs(filePath);
                respuesta = true;
            }
            catch (Exception)
            {
                respuesta = false;
            }

            return respuesta;
        }

        public bool Editar(Producto obj)
        {
            bool respuesta = false;

            try
            {
                if (!File.Exists(filePath))
                    return respuesta;

                var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheet(1);

                foreach (var row in worksheet.RowsUsed())
                {
                    if (row.RowNumber() == 1) continue; // Skip header row

                    if (row.Cell(1).GetValue<int>() == obj.IdProducto)
                    {
                        row.Cell(2).Value = obj.Codigo;
                        row.Cell(3).Value = obj.oCategoria.IdCategoria;
                        row.Cell(4).Value = obj.oCategoria.Descripcion;
                        row.Cell(5).Value = obj.Descripcion;
                        row.Cell(6).Value = obj.PrecioCompra;
                        row.Cell(7).Value = obj.PrecioVenta;
                        row.Cell(8).Value = obj.Stock;
                        break;
                    }
                }

                workbook.SaveAs(filePath);
                respuesta = true;
            }
            catch (Exception)
            {
                respuesta = false;
            }

            return respuesta;
        }

        public bool Eliminar(int idProducto)
        {
            bool respuesta = false;

            try
            {
                if (!File.Exists(filePath))
                    return respuesta;

                var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheet(1);

                foreach (var row in worksheet.RowsUsed())
                {
                    if (row.RowNumber() == 1) continue; // Skip header row

                    if (row.Cell(1).GetValue<int>() == idProducto)
                    {
                        row.Delete();
                        break;
                    }
                }

                workbook.SaveAs(filePath);
                respuesta = true;
            }
            catch (Exception)
            {
                respuesta = false;
            }

            return respuesta;
        }
    }
}
