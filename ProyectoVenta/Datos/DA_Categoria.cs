using ClosedXML.Excel;
using ProyectoVenta.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProyectoVenta.Datos
{
    public class DA_Categoria
    {
        private readonly string filePath;

        public DA_Categoria()
        {
            var cn = new Conexion("Categorias");
            filePath = cn.GetArchivoExcelPath();
        }

        public List<Categoria> Listar()
        {
            var oLista = new List<Categoria>();

            if (!File.Exists(filePath))
                return oLista;

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed();

                foreach (var row in rows)
                {
                    if (row.RowNumber() == 1) continue; // Skip header row

                    oLista.Add(new Categoria()
                    {
                        IdCategoria = row.Cell(1).GetValue<int>(),
                        Descripcion = row.Cell(2).GetValue<string>()
                    });
                }
            }

            return oLista;
        }

        public bool Guardar(Categoria obj)
        {
            bool respuesta = false;

            try
            {
                var workbook = File.Exists(filePath) ? new XLWorkbook(filePath) : new XLWorkbook();
                var worksheet = workbook.Worksheets.Count == 0 ? workbook.AddWorksheet("Categorias") : workbook.Worksheet(1);

                if (worksheet.RowsUsed().Count() == 0)
                {
                    worksheet.Cell(1, 1).Value = "IdCategoria";
                    worksheet.Cell(1, 2).Value = "Descripcion";
                }

                var lastRow = worksheet.LastRowUsed().RowNumber() + 1;

                worksheet.Cell(lastRow, 1).Value = lastRow - 1; // Assuming IdCategoria is auto-incremented
                worksheet.Cell(lastRow, 2).Value = obj.Descripcion;

                workbook.SaveAs(filePath);
                respuesta = true;
            }
            catch (Exception)
            {
                respuesta = false;
            }

            return respuesta;
        }

        public bool Editar(Categoria obj)
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

                    if (row.Cell(1).GetValue<int>() == obj.IdCategoria)
                    {
                        row.Cell(2).Value = obj.Descripcion;
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

        public bool Eliminar(int idCategoria)
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

                    if (row.Cell(1).GetValue<int>() == idCategoria)
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
