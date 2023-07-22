namespace WebApiPeliculas.Services
{
    public interface IAlmacenadorArchivos
    {
        Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contentType);
        Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string contentType, string ruta);
        Task BorrarArchivo(string contenedor, string ruta);
    }
}
