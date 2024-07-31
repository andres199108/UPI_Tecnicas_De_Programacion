using ProyectoVenta.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ClosedXML.Excel;
using System.Xml.Linq;

namespace ProyectoVenta.Datos
{
    public class DA_Autocomplete
    {
        private readonly string filePath;

        public DA_Autocomplete()
        {
            var cn = new Conexion("autocomplete");
            filePath = cn.GetArchivoExcelPath();
        }

        public List<Autocomplete> Listar()
        {
            var oLista = new List<Autocomplete>();

            if (!File.Exists(filePath))
                return oLista;

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet("Autocomplete");

                var rows = worksheet.RowsUsed().Skip(1); // Skip header row

                foreach (var row in rows)
                {
                    oLista.Add(new Autocomplete()
                    {
                        label = row.Cell(1).GetValue<string>(),
                        value = row.Cell(2).GetValue<int>()
                    });
                }
            }

            return oLista;
        }

        public bool Guardar(Autocomplete obj)
        {
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet("Autocomplete");
                    var newRow = worksheet.LastRowUsed().RowBelow();
                    newRow.Cell(1).Value = obj.label;
                    newRow.Cell(2).Value = obj.value;
                    workbook.Save();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Editar(Autocomplete obj)
        {
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet("Autocomplete");
                    var row = worksheet.RowsUsed().FirstOrDefault(r => r.Cell(2).GetValue<int>() == obj.value);
                    if (row != null)
                    {
                        row.Cell(1).Value = obj.label;
                        workbook.Save();
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool Eliminar(int value)
        {
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet("Autocomplete");
                    var row = worksheet.RowsUsed().FirstOrDefault(r => r.Cell(2).GetValue<int>() == value);
                    if (row != null)
                    {
                        row.Delete();
                        workbook.Save();
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
