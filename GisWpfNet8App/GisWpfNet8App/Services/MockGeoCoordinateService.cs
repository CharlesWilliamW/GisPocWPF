using GisWinFormsNet8App.Models;

namespace GisWinFormsNet8App.Services
{
    /// <summary>
    /// PoC 階段 Mock 實作。
    /// 未來換成真實 API 時，新增 ApiGeoCoordinateService : IGeoCoordinateService 取代此類別即可。
    /// </summary>
    public class MockGeoCoordinateService : IGeoCoordinateService
    {
        public Task<List<GeoCoordinate>> GetCoordinatesAsync()
        {
            var data = new List<GeoCoordinate>
            {
                // 三點皆在地圖預設中心 (25.0330, 121.5654) zoom=13 可見範圍內
                // 參考基準：Taipei 101 ≈ E303923 / N2769867
                new GeoCoordinate
                {
                    CoordinateId  = "SOL-001",
                    Name          = "太陽能設施 A",
                    EastX         = 303500.0,   // 中心偏西北 ~600 m
                    NorthY        = 2770300.0,
                    BaseElevation = 45.0
                },
                new GeoCoordinate
                {
                    CoordinateId  = "WIN-001",
                    Name          = "風能設施 B",
                    EastX         = 305000.0,   // 中心偏東 ~1000 m
                    NorthY        = 2769800.0,
                    BaseElevation = 320.0
                },
                new GeoCoordinate
                {
                    CoordinateId  = "ADM-001",
                    Name          = "行政區 C",
                    EastX         = 304000.0,   // 中心偏南 ~700 m
                    NorthY        = 2769100.0,
                    BaseElevation = 12.0
                }
            };

            return Task.FromResult(data);
        }
    }
}
