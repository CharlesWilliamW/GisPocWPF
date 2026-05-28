using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GisWinFormsNet8App;
using GisWinFormsNet8App.Models;
using GisWinFormsNet8App.Services;

namespace GisWpfApp
{
    public partial class MainWindow : Window
    {
        private readonly MapDataService _dataService;
        private readonly DisasterOverlayManager _disasterManager;
        private readonly MapMeasurementService _measurementService;

        private readonly IGeoCoordinateService _geoService;
        private readonly GeoCoordinateOverlayManager _geoManager;

        // GMap WinForms 控件，透過 WindowsFormsHost 嵌入
        private readonly GMapControl gMapControl1;

        private List<GeoCoordinate> _geoData = new();

        // 追蹤災害圖層 Toggle 狀態
        private bool _isDisasterShown = false;

        public MainWindow()
        {
            InitializeComponent();

            // 建立 WinForms GMapControl 並掛入 WindowsFormsHost
            gMapControl1 = new GMapControl();
            mapHost.Child = gMapControl1;

            InitializeGisMap();

            // 初始化服務，傳入 WinForms GMapControl 實例
            _dataService        = new MapDataService();
            _disasterManager    = new DisasterOverlayManager(gMapControl1);
            _measurementService = new MapMeasurementService(gMapControl1);
            _geoService         = new MockGeoCoordinateService();
            _geoManager         = new GeoCoordinateOverlayManager(gMapControl1);

            // 綁定 GMap 滑鼠事件
            gMapControl1.MouseClick += GMapControl1_MouseClick;

            // 即時顯示游標座標
            gMapControl1.MouseMove += GMapControl1_MouseMove;

            // 預載設施點位資料
            _ = LoadGeoPointsAsync();
        }

        // ──────────────────────────────────────────────────────
        // 地圖初始化
        // ──────────────────────────────────────────────────────
        private void InitializeGisMap()
        {
            try
            {
                gMapControl1.MapProvider      = GMapProviders.WikiMapiaMap;
                gMapControl1.Position         = new PointLatLng(25.0330, 121.5654); // 台北市
                gMapControl1.MinZoom          = 5;
                gMapControl1.MaxZoom          = 18;
                gMapControl1.Zoom             = 13;
                gMapControl1.DragButton       = System.Windows.Forms.MouseButtons.Left;
                gMapControl1.MouseWheelZoomEnabled = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"地圖引擎初始化失敗：{ex.Message}", "錯誤",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ──────────────────────────────────────────────────────
        // GMap 事件：點擊量測
        // ──────────────────────────────────────────────────────
        private void GMapControl1_MouseClick(object sender,
            System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            PointLatLng clickedPoint = gMapControl1.FromLocalToLatLng(e.X, e.Y);

            // 情況 A：測距模式
            if (chkMeasureMode.IsChecked == true)
            {
                string result = _measurementService.HandleMapClick(clickedPoint);

                // 更新底部狀態列（需跨執行緒 Dispatch）
                Dispatcher.Invoke(() =>
                {
                    lblMeasureResult.Text = result;
                    lblMapHint.Text       = "測距模式 · 點擊第二點完成量測";
                });
            }

            // 情況 B：緩衝區模式
            if (chkBufferMode.IsChecked == true)
            {
                double radius = TryParseRadius();
                _measurementService.CreateBuffer(clickedPoint, radius);
            }
        }

        // ──────────────────────────────────────────────────────
        // GMap 事件：滑鼠移動 → 即時顯示座標
        // ──────────────────────────────────────────────────────
        private void GMapControl1_MouseMove(object sender,
            System.Windows.Forms.MouseEventArgs e)
        {
            PointLatLng pos = gMapControl1.FromLocalToLatLng(e.X, e.Y);
            Dispatcher.Invoke(() =>
            {
                lblCoordinates.Text = $"Lat {pos.Lat:F5}  Lng {pos.Lng:F5}";
            });
        }

        // ──────────────────────────────────────────────────────
        // 災害觀測點 ToggleButton
        // ──────────────────────────────────────────────────────
        private async void btnToggleDisaster_Checked(object sender, RoutedEventArgs e)
        {
            _isDisasterShown         = true;
            btnToggleDisaster.Content  = "隱藏災害觀測點";
            btnToggleDisaster.IsEnabled = false;

            try
            {
                var data = await _dataService.FetchDisasterPoints();
                _disasterManager.ToggleVisibility(true, data);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"資料載入失敗：{ex.Message}", "錯誤",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                btnToggleDisaster.IsChecked = false;
            }
            finally
            {
                btnToggleDisaster.IsEnabled = true;
            }
        }

        private void btnToggleDisaster_Unchecked(object sender, RoutedEventArgs e)
        {
            _isDisasterShown         = false;
            btnToggleDisaster.Content  = "顯示災害觀測點";
            _disasterManager.ToggleVisibility(false);
        }

        // ──────────────────────────────────────────────────────
        // 設施點位 CheckBox
        // ──────────────────────────────────────────────────────
        private async Task LoadGeoPointsAsync()
        {
            _geoData = await _geoService.GetCoordinatesAsync();
            foreach (var coord in _geoData)
                _geoManager.Register(coord);

            if (_geoData.Count > 0) chkGeoA.Content = _geoData[0].Name;
            if (_geoData.Count > 1) chkGeoB.Content = _geoData[1].Name;
            if (_geoData.Count > 2) chkGeoC.Content = _geoData[2].Name;
        }

        private void chkGeo_Changed(object sender, RoutedEventArgs e)
        {
            if (sender == chkGeoA && _geoData.Count > 0)
                _geoManager.SetVisible(_geoData[0], chkGeoA.IsChecked == true);
            else if (sender == chkGeoB && _geoData.Count > 1)
                _geoManager.SetVisible(_geoData[1], chkGeoB.IsChecked == true);
            else if (sender == chkGeoC && _geoData.Count > 2)
                _geoManager.SetVisible(_geoData[2], chkGeoC.IsChecked == true);
        }

        // ──────────────────────────────────────────────────────
        // 緩衝區 CheckBox
        // ──────────────────────────────────────────────────────
        private void chkBufferMode_Changed(object sender, RoutedEventArgs e)
        {
            bool isOn = chkBufferMode.IsChecked == true;
            _measurementService.SetBufferVisibility(isOn, TryParseRadius());
            gMapControl1.Refresh();

            lblMapHint.Text = isOn
                ? "緩衝區模式 · 點擊地圖生成影響範圍圓"
                : "左鍵拖曳平移 · 滾輪縮放";
        }

        // ──────────────────────────────────────────────────────
        // 清除量測按鈕
        // ──────────────────────────────────────────────────────
        private void btnClearMeasure_Click(object sender, RoutedEventArgs e)
        {
            _measurementService.ClearMeasurement();
            lblMeasureResult.Text = "—";
            lblMapHint.Text       = "左鍵拖曳平移 · 滾輪縮放";
        }

        // ──────────────────────────────────────────────────────
        // 工具方法：解析半徑輸入欄位，預設 500m
        // ──────────────────────────────────────────────────────
        private double TryParseRadius()
        {
            if (double.TryParse(txtBufferRadius.Text, out double r) && r > 0)
                return r;
            return 500;
        }
    }
}
