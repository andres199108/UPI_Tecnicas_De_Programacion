using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace ProyectoVenta.Datos
{
    public class Conexion
    {
        private string archivoExcelPath = string.Empty;

        public Conexion(string P_ArchivoExcel)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            archivoExcelPath =  Path.Combine(AppDomain.CurrentDomain.BaseDirectory) + builder.GetSection("ArchivoExcelPath:" + P_ArchivoExcel).Value; ;
            
        }

        public string GetArchivoExcelPath()
        {
            return archivoExcelPath;
        }
    }
}



class Conexion { private string archivoExcelPath = string.Empty; public Conexion() { var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build(); archivoExcelPath = builder.GetSection("ConnectionStrings:ArchivoExcelPath").Value; 
    } public string getArchivoExcelPath(string nombreArchivo = "") 
    { return string.IsNullOrEmpty(nombreArchivo) ? archivoExcelPath : Path.Combine(archivoExcelPath, nombreArchivo + ".xlsx");
    } 
}
