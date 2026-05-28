using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GisWinFormsNet8App.Models;
using GisWinFormsNet8App.Utils;

namespace GisWinFormsNet8App
{
    public class GeoCoordinateOverlayManager
    {
        private readonly GMapControl _map;
        private readonly GMapOverlay _overlay;
        private readonly Dictionary<GeoCoordinate, GMapMarker> _markers = new();

        public GeoCoordinateOverlayManager(GMapControl map)
        {
            _map = map;
            _overlay = new GMapOverlay("GeoCoordinateLayer");
            _map.Overlays.Add(_overlay);
        }

        public void Register(GeoCoordinate coord)
        {
            var (lat, lon) = Twd97Converter.ToWgs84(coord.EastX, coord.NorthY);
            var marker = new GMarkerGoogle(new PointLatLng(lat, lon), GMarkerGoogleType.blue)
            {
                ToolTipText = $"{coord.Name}\n高程: {coord.BaseElevation} m",
                ToolTipMode = MarkerTooltipMode.OnMouseOver,
                IsVisible = false
            };
            _overlay.Markers.Add(marker);
            _markers[coord] = marker;
        }

        public void SetVisible(GeoCoordinate coord, bool visible)
        {
            if (_markers.TryGetValue(coord, out var marker))
            {
                marker.IsVisible = visible;
                _map.Refresh();
            }
        }
    }
}
