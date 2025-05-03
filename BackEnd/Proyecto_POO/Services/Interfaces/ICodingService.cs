namespace Proyecto_POO.Services.Interfaces
{
    public interface IGeocodingService
    {
        Task<(double latitude, double longitude)> ObtenerCoordenadas(string direccion);
    }
}
