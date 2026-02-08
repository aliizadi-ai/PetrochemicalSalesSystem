using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrochemicalSalesSystem.Utilities
{
    // 📄 ValidationHelper.cs
    public static class ValidationHelper
    {
        public static bool IsValidDecimal(string value)
        {
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        public static decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            value = value.Replace(",", "");
            decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result);
            return result;
        }
    }
}
