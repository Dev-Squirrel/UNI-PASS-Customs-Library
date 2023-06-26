using System.Text.Json;

namespace UNI_PASS_Customs_Library
{
    public class UniPassApi
    {
        private static readonly HttpClient client = new();

        private static readonly Dictionary<string, string> additionalHeaders = new()
        {
            { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36" },
            { "X-Requested-With", "XMLHttpRequest" }
        };

        private static async Task<string> MakeApiRequest(string apiUrl, Dictionary<string, string> parameters)
        {
            try
            {
                foreach (var header in additionalHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                var requestContent = new FormUrlEncodedContent(parameters);
                var response = await client.PostAsync(apiUrl, requestContent);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"API call failed: {ex.Message}");
                return ex.Message;
            }
        }

        private static async Task<string> RetrieveOvrsDrtPuchPrgsInfoLstAsync(string persEcm, string startDate, string endDate, string pagePerRecord = "100")
        {
            string apiUrl = "https://unipass.customs.go.kr/csp/myc/bsopspptinfo/cscllgstinfo/ImpCargPrgsInfoMtCtr/retrieveOvrsDrtPuchPrgsInfoLstLst.do";

            var parameters = new Dictionary<string, string>
            {
                { "firstIndex", "0" },
                { "page", "1" },
                { "pageIndex", "1" },
                { "pageSize", pagePerRecord },
                { "pageUnit", pagePerRecord },
                { "recordCountPerPage", pagePerRecord },
                { "ovrsDrtPuchFnm", "" },
                { "persEcm", persEcm },
                { "dclrStrtDt", startDate },
                { "dclrEndDt", endDate },
                { "scrnId", "" }
            };

            return await MakeApiRequest(apiUrl, parameters);
        }

        public static async Task RetrieveOvrsDrtPuchPrgsInfoLstGetInfo(string persEcm, string startDate, string endDate)
        {
            var ovrsDrtPuchPrgsInfoResult = await RetrieveOvrsDrtPuchPrgsInfoLstAsync(persEcm, startDate, endDate);

            using JsonDocument doc = JsonDocument.Parse(ovrsDrtPuchPrgsInfoResult);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("error", out JsonElement errorElement) && errorElement.ValueKind == JsonValueKind.True)
            {
                string errorMessage = root.GetProperty("message").GetString() ?? "";
                Console.WriteLine("An error occurred: " + errorMessage);
            }
            else
            {
                int count = root.GetProperty("count").GetInt32();

                if (count > 0)
                {
                    JsonElement resultList = root.GetProperty("resultList");

                    Console.WriteLine("Total count: " + count);
                    Console.WriteLine("No\t수입신고수리일\t통관형태\t신고(제출)번호\t\tM B/L(Master BL) - H B/L(House BL)\t처리상태");

                    for (int i = 0; i < count; i++)
                    {
                        JsonElement resultItem = resultList[i];

                        string acptDt = resultItem.GetProperty("acptDt").GetString() ?? "";
                        string csclForm = resultItem.GetProperty("csclForm").GetString() ?? "";
                        string dclrNo = resultItem.GetProperty("dclrNo").GetString() ?? "";
                        string mblNo = resultItem.GetProperty("mblNo").GetString() ?? "";
                        string hblNo = resultItem.GetProperty("hblNo").GetString() ?? "";
                        string prcsStts = resultItem.GetProperty("prcsStts").GetString() ?? "";

                        Console.WriteLine($"{(i + 1):000}\t{acptDt.PadLeft(8, '*')}\t{csclForm}\t{dclrNo,16}\t{mblNo,16} - {hblNo,-16}\t{prcsStts}");
                    }
                }
            }
        }
    }
}
