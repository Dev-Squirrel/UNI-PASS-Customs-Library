using System.Globalization;

namespace UNI_PASS_Customs_Library
{
    public class UniPassUtil
    {
        public static bool ValidateDateRange(string startDate, string endDate)
        {
            if (!DateTime.TryParseExact(startDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStartDate) ||
                !DateTime.TryParseExact(endDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedEndDate) ||
                (parsedEndDate - parsedStartDate).TotalDays > 90)
            {
                Console.WriteLine("Invalid date range. The maximum allowed range is 3 months.");
                return false;
            }

            return true;
        }

        public static bool IsValidPersEcm(string persEcm)
        {
            if (string.IsNullOrEmpty(persEcm) || persEcm.Length != 13 || !persEcm[1..].All(char.IsDigit) || !persEcm.StartsWith("P"))
            {
                Console.WriteLine("Invalid persEcm value. The personal customs clearance number you entered is invalid. Please enter it again.");
                return false;
            }

            return true;
        }
    }
}
