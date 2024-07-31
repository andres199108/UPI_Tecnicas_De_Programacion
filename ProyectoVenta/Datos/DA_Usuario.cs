using ClosedXML.Excel;
using ProyectoVenta.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ProyectoVenta.Datos
{
    public class DA_Usuario
    {
        private readonly string filePath;

        public DA_Usuario()
        {
            var cn = new Conexion("Usuarios");
            filePath = cn.GetArchivoExcelPath();
        }

        public List<Usuario> Listar()
        {
            var oLista = new List<Usuario>();

            if (!File.Exists(filePath))
                return oLista;

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet("Usuarios");
                var rows = worksheet.RowsUsed();

                foreach (var row in rows)
                {
                    if (row.RowNumber() == 1) continue; // Skip header row

                    oLista.Add(new Usuario()
                    {
                        IdUsuario = row.Cell(1).GetValue<int>(),
                        NombreCompleto = row.Cell(2).GetValue<string>(),
                        Correo = row.Cell(3).GetValue<string>(),
                        Clave = row.Cell(4).GetValue<string>()
                    });
                }
            }

            return oLista;
        }

        public bool Guardar(Usuario obj)
        {
            bool respuesta = false;

            try
            {
                var workbook = File.Exists(filePath) ? new XLWorkbook(filePath) : new XLWorkbook();
                var worksheet = workbook.Worksheets.Count == 0 ? workbook.AddWorksheet("Usuarios") : workbook.Worksheet("Usuarios");

                if (worksheet.RowsUsed().Count() == 0)
                {
                    worksheet.Cell(1, 1).Value = "IdUsuario";
                    worksheet.Cell(1, 2).Value = "NombreCompleto";
                    worksheet.Cell(1, 3).Value = "Correo";
                    worksheet.Cell(1, 4).Value = "Clave";
                }

                var lastRow = worksheet.LastRowUsed().RowNumber() + 1;

                worksheet.Cell(lastRow, 1).Value = lastRow - 1; // Assuming IdUsuario is auto-incremented
                worksheet.Cell(lastRow, 2).Value = obj.NombreCompleto;
                worksheet.Cell(lastRow, 3).Value = obj.Correo;
                worksheet.Cell(lastRow, 4).Value = obj.Clave;

                workbook.SaveAs(filePath);
                respuesta = true;
            }
            catch (Exception)
            {
                respuesta = false;
            }

            return respuesta;
        }

        public bool Editar(Usuario obj)
        {
            bool respuesta = false;

            try
            {
                if (!File.Exists(filePath))
                    return respuesta;

                var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheet("Usuarios");

                foreach (var row in worksheet.RowsUsed())
                {
                    if (row.RowNumber() == 1) continue; // Skip header row

                    if (row.Cell(1).GetValue<int>() == obj.IdUsuario)
                    {
                        row.Cell(2).Value = obj.NombreCompleto;
                        row.Cell(3).Value = obj.Correo;
                        row.Cell(4).Value = obj.Clave;
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

        public bool Eliminar(int idUsuario)
        {
            bool respuesta = false;

            try
            {
                if (!File.Exists(filePath))
                    return respuesta;

                var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheet("UsuariosIL");

                foreach (var row in worksheet.RowsUsed())
                {
                    if (row.RowNumber() == 1) continue; // Skip header row

                    if (row.Cell(1).GetValue<int>() == idUsuario)
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

