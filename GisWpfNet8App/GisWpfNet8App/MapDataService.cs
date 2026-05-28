using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace GisWinFormsNet8App
{
    // 定義與 API 欄位對接的資料實體類別
    public class DisasterPoint
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class MapDataService
    {
        /// <summary>
        /// 模擬非同步從 API 獲取災害觀測點資料
        /// </summary>
        private async Task<string> FetchMapDataAsync()
        {
            try
            {
                // 3. 模擬呼叫政府 API 獲取地理資料 (未來串接時只需替換此網址)
                string apiUrl = "https://api.mock_government.gov.tw/v1/disaster";
                // 【未來對接解鎖碼】：等業主把正式 API Key 申請下來後，把下面兩行解開即可直接通訊
                // HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                // return await response.Content.ReadAsStringAsync();

                // 【PoC 原型階段 Mock】：我們先用標準格式假裝拿到資料
                // 模擬網路延遲 800 毫秒
                await Task.Delay(800);

                // 回傳模擬的台北市災害觀測點資料
                //List<DisasterPoint>
                //return new List<DisasterPoint>
                //{
                //    new DisasterPoint { Name = "觀測點 A (大安)", Latitude = 25.026, Longitude = 121.543, Description = "積水警戒區域" },
                //    new DisasterPoint { Name = "觀測點 B (信義)", Latitude = 25.033, Longitude = 121.564, Description = "強風觀測站" },
                //    new DisasterPoint { Name = "觀測點 C (中山)", Latitude = 25.052, Longitude = 121.532, Description = "土石流潛勢溪流監控" }
                //};

                return @"
                [
                    {""Name"":""大安區積水觀測站 A"", ""Latitude"":25.0339, ""Longitude"":121.5434, ""Status"":""注意"", ""Description"":""積水警戒區域""},
                    {""Name"":""信義區水位監測點 B"", ""Latitude"":25.0375, ""Longitude"":121.5662, ""Status"":""正常"", ""Description"":""強風觀測站""},
                    {""Name"":""松山區淹水警戒區 C"", ""Latitude"":25.0524, ""Longitude"":121.5540, ""Status"":""危險"", ""Description"":""土石流潛勢溪流監控""}
                ]";
            }
            catch
            {
                throw new Exception("外部圖資伺服器連線逾時");
            }
        }

        public async Task<List<DisasterPoint>> FetchDisasterPoints()
        {
            string jsonResult = await FetchMapDataAsync();
            // 4. 解析符合政府規格的 JSON 資料結構
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<DisasterPoint> points = JsonSerializer.Deserialize<List<DisasterPoint>>(jsonResult, options);
            return points;
        }
    }
}
