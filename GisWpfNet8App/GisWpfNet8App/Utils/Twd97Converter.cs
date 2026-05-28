namespace GisWinFormsNet8App.Utils
{
    /// <summary>
    /// TWD97 TM2（橫麥卡托二度分帶）→ WGS84 反投影
    /// 中央經線 121°E，假橫坐標 250,000 m，比例因子 0.9999
    /// </summary>
    public static class Twd97Converter
    {
        private const double A    = 6378137.0;
        private const double F    = 1.0 / 298.257222101;
        private const double K0   = 0.9999;
        private const double Lon0 = 121.0 * Math.PI / 180.0;
        private const double E0   = 250_000.0;

        public static (double Lat, double Lon) ToWgs84(double east, double north)
        {
            double b   = A * (1 - F);
            double e2  = (A * A - b * b) / (A * A);
            double ep2 = (A * A - b * b) / (b * b);

            double x  = east - E0;
            double M  = north / K0;
            double mu = M / (A * (1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * Math.Pow(e2, 3) / 256));

            double e1   = (1 - Math.Sqrt(1 - e2)) / (1 + Math.Sqrt(1 - e2));
            double phi1 = mu
                + (3 * e1 / 2        - 27 * Math.Pow(e1, 3) / 32) * Math.Sin(2 * mu)
                + (21 * e1 * e1 / 16 - 55 * Math.Pow(e1, 4) / 32) * Math.Sin(4 * mu)
                + (151 * Math.Pow(e1, 3) / 96)                     * Math.Sin(6 * mu)
                + (1097 * Math.Pow(e1, 4) / 512)                   * Math.Sin(8 * mu);

            double sinP = Math.Sin(phi1), cosP = Math.Cos(phi1), tanP = Math.Tan(phi1);
            double N1   = A / Math.Sqrt(1 - e2 * sinP * sinP);
            double T1   = tanP * tanP;
            double C1   = ep2 * cosP * cosP;
            double R1   = A * (1 - e2) / Math.Pow(1 - e2 * sinP * sinP, 1.5);
            double D    = x / (N1 * K0);

            double lat = phi1 - (N1 * tanP / R1) * (
                D * D / 2
                - (5 + 3 * T1 + 10 * C1 - 4 * C1 * C1 - 9 * ep2)          * Math.Pow(D, 4) / 24
                + (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1 - 252 * ep2 - 3 * C1 * C1) * Math.Pow(D, 6) / 720);

            double lon = Lon0 + (
                D
                - (1 + 2 * T1 + C1)                                                   * Math.Pow(D, 3) / 6
                + (5 - 2 * C1 + 28 * T1 - 3 * C1 * C1 + 8 * ep2 + 24 * T1 * T1)     * Math.Pow(D, 5) / 120
            ) / cosP;

            return (lat * 180.0 / Math.PI, lon * 180.0 / Math.PI);
        }
    }
}
