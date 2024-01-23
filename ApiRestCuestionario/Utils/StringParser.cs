using System.Globalization;
using System.Text;

namespace ApiRestCuestionario.Utils
{
    public class StringParser
    {
        public static string NormalizeString(string str)
        {
            string normalizedString = str.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString().Trim().ToLower().Replace(" ", "_");
        }
        public static Dictionary<string, int> CheckColumnItems(List<string> columns)
        {
            Dictionary<string, int> itemsCounter = new Dictionary<string, int>();

            foreach (var x in columns)
            {
                var items = x.Trim().Split("_");
                var isNumeric = int.TryParse(items.Last(), out int n);

                if (items.Length > 1 && isNumeric)
                {
                    var verificationString = string.Join("_", items.Take(items.Length - 1));
                    if (itemsCounter.ContainsKey(verificationString) && n > itemsCounter[verificationString])
                    {
                        itemsCounter[verificationString] = n;
                    }
                    else
                    {
                        itemsCounter[x] = 1;
                    }
                }
                else
                {
                    itemsCounter[x] = 1;
                }
            }
            return itemsCounter;
        }
    }
}
