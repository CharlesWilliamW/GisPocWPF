using GisWinFormsNet8App.Models;

namespace GisWinFormsNet8App.Services
{
    public interface IGeoCoordinateService
    {
        Task<List<GeoCoordinate>> GetCoordinatesAsync();
    }
}
