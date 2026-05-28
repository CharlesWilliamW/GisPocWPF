using System.Collections.Generic;
using System.Windows;
using GisWinFormsNet8App.Models;

namespace GisWpfApp
{
    public partial class CompareWindow : Window
    {
        public CompareWindow(GeoCoordinate a, GeoCoordinate b)
        {
            InitializeComponent();

            dgCompare.Columns[1].Header = a.Name;
            dgCompare.Columns[2].Header = b.Name;

            dgCompare.ItemsSource = new List<CompareRow>
            {
                new() { Field = "設施名稱",            ValueA = a.Name,                      ValueB = b.Name },
                new() { Field = "設施 ID",             ValueA = a.CoordinateId,              ValueB = b.CoordinateId },
                new() { Field = "TWD97 東座標 X (m)",  ValueA = $"{a.EastX:N3}",            ValueB = $"{b.EastX:N3}" },
                new() { Field = "TWD97 北座標 Y (m)",  ValueA = $"{a.NorthY:N3}",           ValueB = $"{b.NorthY:N3}" },
                new() { Field = "海拔高度 (m)",        ValueA = $"{a.BaseElevation:N1}",    ValueB = $"{b.BaseElevation:N1}" },
            };
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) => Close();
    }

    internal class CompareRow
    {
        public string Field  { get; set; } = "";
        public string ValueA { get; set; } = "";
        public string ValueB { get; set; } = "";
        public bool IsDifferent => ValueA != ValueB;
    }
}
