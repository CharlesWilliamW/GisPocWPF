using System;
using System.Collections.Generic;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace GisWinFormsNet8App
{
    public class MapMeasurementService
    {
        private readonly GMapControl _mapControl;
        private readonly GMapOverlay _measureOverlay;
        private readonly GMapOverlay _bufferOverlay;
        private PointLatLng? _startPoint = null;

        public MapMeasurementService(GMapControl mapControl)
        {
            _mapControl = mapControl ?? throw new ArgumentNullException(nameof(mapControl));
            _measureOverlay = new GMapOverlay("measure_overlay");
            _mapControl.Overlays.Add(_measureOverlay);
            _bufferOverlay = new GMapOverlay("buffer_overlay");
            _mapControl.Overlays.Add(_bufferOverlay);
        }

        public string HandleMapClick(PointLatLng clickedPoint)
        {
            if (_startPoint == null)
            {
                ClearMeasurement();
                _startPoint = clickedPoint;

                var startMarker = new GMarkerGoogle(clickedPoint, GMarkerGoogleType.green)
                {
                    ToolTipText = "起點",
                    ToolTipMode = MarkerTooltipMode.Always
                };
                _measureOverlay.Markers.Add(startMarker);

                return "距離：請點擊第二點...";
            }
            else
            {
                var endMarker = new GMarkerGoogle(clickedPoint, GMarkerGoogleType.blue);
                _measureOverlay.Markers.Add(endMarker);

                double distanceKm = CalculateHaversineDistance(_startPoint.Value, clickedPoint);

                endMarker.ToolTipText = $"終點\n距離: {distanceKm:F2} 公里";
                endMarker.ToolTipMode = MarkerTooltipMode.Always;

                var points = new List<PointLatLng> { _startPoint.Value, clickedPoint };
                var route = new GMapRoute(points, "measure_line")
                {
                    Stroke = new Pen(Color.Blue, 3)
                };
                _measureOverlay.Routes.Add(route);

                System.Windows.MessageBox.Show(
                    $"量測完成！\n點對點真實地理距離為：{distanceKm:F2} 公里",
                    "距離量測提示",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);

                _startPoint = null;
                _mapControl.Refresh();
                return $"距離：{distanceKm:F2} 公里";
            }
        }

        public void ClearMeasurement()
        {
            _startPoint = null;
            _measureOverlay.Markers.Clear();
            _measureOverlay.Routes.Clear();
            _mapControl.Refresh();
        }

        public void SetBufferVisibility(bool isOn, double radius)
        {
            _bufferOverlay.IsVisibile = isOn;
            if (!isOn)
            {
                _bufferOverlay.Polygons.Clear();
                _bufferOverlay.Markers.Clear();
            }
            _mapControl.Refresh();
        }

        public void CreateBuffer(PointLatLng center, double radiusMeters)
        {
            _bufferOverlay.Polygons.Clear();
            _bufferOverlay.Markers.Clear();

            var circlePoints = GenerateCirclePoints(center, radiusMeters);
            var polygon = new GMapPolygon(circlePoints, "buffer_circle")
            {
                Fill = new SolidBrush(Color.FromArgb(60, 30, 111, 235)),
                Stroke = new Pen(Color.FromArgb(200, 30, 111, 235), 2)
            };
            _bufferOverlay.Polygons.Add(polygon);

            var centerMarker = new GMarkerGoogle(center, GMarkerGoogleType.red)
            {
                ToolTipText = $"緩衝區中心\n半徑: {radiusMeters:F0} 公尺",
                ToolTipMode = MarkerTooltipMode.Always
            };
            _bufferOverlay.Markers.Add(centerMarker);

            _mapControl.Refresh();
        }

        private List<PointLatLng> GenerateCirclePoints(PointLatLng center, double radiusMeters)
        {
            var points = new List<PointLatLng>();
            const int numPoints = 64;
            double radiusDeg = radiusMeters / 111320.0;
            double cosLat = Math.Cos(center.Lat * Math.PI / 180.0);

            for (int i = 0; i < numPoints; i++)
            {
                double angle = 2 * Math.PI * i / numPoints;
                double lat = center.Lat + radiusDeg * Math.Cos(angle);
                double lng = center.Lng + radiusDeg * Math.Sin(angle) / cosLat;
                points.Add(new PointLatLng(lat, lng));
            }
            return points;
        }

        private double CalculateHaversineDistance(PointLatLng pos1, PointLatLng pos2)
        {
            double R = 6371.0;
            double lat1Rad = ToRadians(pos1.Lat);
            double lat2Rad = ToRadians(pos2.Lat);
            double deltaLat = ToRadians(pos2.Lat - pos1.Lat);
            double deltaLng = ToRadians(pos2.Lng - pos1.Lng);
            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLng / 2) * Math.Sin(deltaLng / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double ToRadians(double val) => val * Math.PI / 180.0;
    }
}
