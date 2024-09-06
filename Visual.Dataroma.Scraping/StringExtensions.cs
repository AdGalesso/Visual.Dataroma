namespace Visual.Datarama
{
    public static class StringExtensions
    {
        public static decimal MoneyToDecimal(this string text)
        {
            decimal result = 0;

            if (string.IsNullOrEmpty(text))
                return result;

            if (text.EndsWith('B'))
            {
                text = text.Replace("B", "").Replace("$", "").Replace(",", "").Trim();

                if (decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tempValue))
                {
                    result = tempValue * 1_000_000_000;
                }
            }
            else if (text.EndsWith("M"))
            {
                text = text.Replace("M", "").Replace("$", "").Replace(",", "").Trim();

                if (decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var tempValue))
                {
                    result = tempValue * 1_000_000;
                }
            }
            else
            {
                text = text.Replace("$", "").Replace(",", "").Trim();

                decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
            }

            return result;
        }
    }
}
