using System.Collections.Generic;
using GisWinFormsNet8App.Models;
using GisWinFormsNet8App.Utils;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace GisWinFormsNet8App
{
    public class GeoCoordinateOverlayManager
    {
        private readonly GMapControl _mapControl;
        private readonly List<GMapOverlay> _overlays = new List<GMapOverlay>();

        public GeoCoordinateOverlayManager(GMapControl mapControl)
        {
            _mapControl = mapControl;
        }

        public void LoadPoints(List<GeoCoordinate> points)
        {
            foreach (var overlay in _overlays)
                _mapControl.Overlays.Remove(overlay);
            _overlays.Clear();

            for (int i = 0; i < points.Count; i++)
            {
                var pt = points[i];
                var (lat, lon) = Twd97Converter.ToWgs84(pt.EastX, pt.NorthY);

                var overlay = new GMapOverlay($"GeoPoint_{pt.CoordinateId}");
                var marker = new GMarkerGoogle(new PointLatLng(lat, lon), GMarkerGoogleType.blue)
                {
                    ToolTipText = $"{pt.Name}\n高程: {pt.BaseElevation} m",
                    ToolTipMode = MarkerTooltipMode.OnMouseOver
                };

                overlay.Markers.Add(marker);
                overlay.IsVisibile = false;
                _mapControl.Overlays.Add(overlay);
                _overlays.Add(overlay);
            }

            _mapControl.Refresh();
        }

        public void SetPointVisibility(int index, bool show)
        {
            if (index < 0 || index >= _overlays.Count) return;
            _overlays[index].IsVisibile = show;
            _mapControl.Refresh();
        }
    }
}
