using System;
using System.Globalization;

namespace DoAnCoSo.Helpers
{
    public static class FormatHelper
    {
        // Tiền tệ: 1500000 -> 1.500.000 đ
        public static string ToVnd(this decimal amount)
        {
            var cultureInfo = CultureInfo.GetCultureInfo("vi-VN");
            return amount.ToString("N0", cultureInfo) + " đ";
        }

        // Ngày giờ: 09/06/2026 18:30
        public static string ToVnDateTime(this DateTime? dateTime)
        {
            if (!dateTime.HasValue) return "N/A";
            return dateTime.Value.ToString("dd/MM/yyyy HH:mm");
        }

        // Ngày thường: 09/06/2026
        public static string ToVnDate(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}