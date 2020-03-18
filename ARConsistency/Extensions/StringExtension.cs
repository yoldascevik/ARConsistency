using System;
using System.Text.Json;

namespace ARConsistency.Extensions
{
    public static class StringExtension
    {
        public static bool IsValidJson(this string text)
        {
            text = text.Trim();
            if ((text.StartsWith("{") && text.EndsWith("}")) ||
                (text.StartsWith("[") && text.EndsWith("]")))
            {
                try
                {
                    object obj = JsonDocument.Parse(text);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
