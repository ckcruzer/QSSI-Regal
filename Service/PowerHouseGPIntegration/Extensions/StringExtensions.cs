namespace BSP.PowerHouse.DynamicsGP.Integration.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxChars)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length > maxChars)
            {
                return value.Substring(0, maxChars);
            }
            else if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            return value;
        }
    }
}
