using System.Data;

namespace Negocio
{
    // Anexo A02: un formato de salida. ExportadorPDF vive en Visual porque iText es una librería
    // de presentación y Negocio no depende de ella; un exportador a Excel sería otra clase que
    // implemente esta interfaz, sin tocar ningún reporte. Devuelve la ruta del archivo generado.
    public interface IExportador
    {
        string Exportar(Reporte r, DataTable datos, string ruta);
    }
}
