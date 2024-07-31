using ClosedXML.Excel;
using ProyectoVenta.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ProyectoVenta.Datos
{
    public class DA_Reporte
    {
        private readonly string filePath;

        public DA_Reporte()
        {
            var cn = new Conexion("Reportes");
            filePath = cn.GetArchivoExcelPath();
        }

        public List<Reporte> Listar(string fechaInicio, string fechaFin)
        {
            var oLista = new List<Reporte>();

            if (!File.Exists(filePath))
                return oLista;

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);// estaba en "Reportes" y cambiamos a un 1
                var rows = worksheet.RowsUsed();

                DateTime startDate = DateTime.Parse(fechaInicio);
                DateTime endDate = DateTime.Parse(fechaFin);

                foreach (var row in rows)
                {
                    if (row.RowNumber() == 1) continue; // Skip header row

                    DateTime fechaRegistro = DateTime.Parse(row.Cell(4).GetValue<string>());

                    if (fechaRegistro >= startDate && fechaRegistro <= endDate)
                    {
                        oLista.Add(new Reporte()
                        {
                            TipoPago = row.Cell(1).GetValue<string>(),
                            NumeroDocumento = row.Cell(2).GetValue<string>(),
                            MontoTotal = row.Cell(3).GetValue<decimal>(),
                            FechaRegistro = row.Cell(4).GetValue<string>(),
                            DesProducto = row.Cell(5).GetValue<string>(),
                            Cantidad = row.Cell(6).GetValue<int>(),
                            PrecioVenta = row.Cell(7).GetValue<decimal>(),
                            Total = row.Cell(8).GetValue<decimal>()
        


                        });
                    }
                }
            }

            return oLista;
        }
    }
}
