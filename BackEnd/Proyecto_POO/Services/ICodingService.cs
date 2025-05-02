namespace Proyecto_POO.Services
{
    public interface IGeocodingService
    {
        Task<(double latitude, double longitude)> ObtenerCoordenadas(string direccion);
    }
}
