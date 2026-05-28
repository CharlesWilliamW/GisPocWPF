namespace GisWinFormsNet8App.Models
{
    /// <summary>
    /// 地理座標資料 — TWD97 座標系統
    /// </summary>
    public class GeoCoordinate
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>設施之唯一空間識別 ID</summary>
        public string CoordinateId { get; set; } = string.Empty;

        /// <summary>TWD97 橫座標 (X / EAST)，單位：公尺</summary>
        public double EastX { get; set; }

        /// <summary>TWD97 縱座標 (Y / NORTH)，單位：公尺</summary>
        public double NorthY { get; set; }

        /// <summary>基地海拔高度，單位：公尺</summary>
        public double BaseElevation { get; set; }

        public override string ToString() => Name;
    }
}
