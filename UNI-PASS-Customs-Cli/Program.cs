using UNI_PASS_Customs_Library;

namespace UnipassCli
{
    class Program
    {
        static async Task Main()
        {
            while (true)
            {
                Console.Write("개인통관고유부호 입력: ");
                string persEcmStr = Console.ReadLine() ?? "";

                // 조회 시작 날짜와 끝 날짜 (최대 90일)
                DateTime endDateTime = DateTime.Today;
                string endDateStr = endDateTime.ToString("yyyyMMdd");

                DateTime startDateTime = endDateTime.AddDays(-90);
                string startDateStr = startDateTime.ToString("yyyyMMdd");

                Console.Write($"시작 날짜 입력 (예 - {startDateStr}): ");
                string startDateInput = Console.ReadLine() ?? startDateStr;
                startDateStr = !string.IsNullOrEmpty(startDateInput) ? startDateInput : startDateStr;

                Console.Write($"종료 날짜 입력 (예 - {endDateStr}): ");
                string endDateInput = Console.ReadLine() ?? endDateStr;
                endDateStr = !string.IsNullOrEmpty(endDateInput) ? endDateInput : endDateStr;
                Console.WriteLine();

                if (UniPassUtil.IsValidPersEcm(persEcmStr) && UniPassUtil.ValidateDateRange(startDateStr, endDateStr))
                {
                    Console.WriteLine("해외직구 통관정보조회");
                    await UniPassApi.RetrieveOvrsDrtPuchPrgsInfoLstGetInfo(persEcmStr, startDateStr, endDateStr);

                    break;
                }

                Console.WriteLine();
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
