using System.Collections.Generic;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace GisWinFormsNet8App
{
    public class DisasterOverlayManager
    {
        private readonly GMapControl _mapControl;
        private readonly GMapOverlay _disasterOverlay;
        private bool _isDataLoaded = false;

        public DisasterOverlayManager(GMapControl mapControl)
        {
            _mapControl = mapControl;

            // 在 GIS 系統中，所有點、線、面都必須放在「圖層 (Overlay)」上，建立獨立的災害圖層，再把圖層加進地圖
            _disasterOverlay = new GMapOverlay("DisasterLayer");
            _mapControl.Overlays.Add(_disasterOverlay);

            // 預設為不顯示
            _disasterOverlay.IsVisibile = false;
        }

        /// <summary>
        /// 切換圖層顯示狀態，如果尚未載入資料，則透過傳入的資料進行載入
        /// </summary>
        public void ToggleVisibility(bool show, List<DisasterPoint>? points = null)
        {
            // 1. 如果要開啟顯示，且尚未載入過大頭針，則進行繪製
            if (show && !_isDataLoaded && points != null)
            {
                RenderPoints(points);
                _isDataLoaded = true;
            }

            // 2. 切換圖層的可見性
            _disasterOverlay.IsVisibile = show;

            // 3. 刷新地圖畫面
            _mapControl.Refresh();
        }

        /// <summary>
        /// 將資料轉換成大頭針並加入圖層
        /// </summary>
        private void RenderPoints(List<DisasterPoint> points)
        {
            _disasterOverlay.Markers.Clear();

            foreach (var pt in points)
            {
                PointLatLng position = new PointLatLng(pt.Latitude, pt.Longitude);

                // 使用內建的橘色大頭針代表災害點
                GMarkerGoogle marker = new GMarkerGoogle(position, GMarkerGoogleType.orange)
                {
                    ToolTipText = $"{pt.Name}\n目前狀態: {pt.Description}",
                    ToolTipMode = MarkerTooltipMode.Always
                };

                _disasterOverlay.Markers.Add(marker);
            }
        }
    }
}