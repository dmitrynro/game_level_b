using System.Collections.Generic;

namespace Shared.Ini
{
    public class IniFileSection
    {
        public Dictionary<string, string> Values = new Dictionary<string, string>();

        public bool TryGetValue(string value, out string result, string defaultValue = "")
        {
            if (Values.ContainsKey(value))
            {
                result = Values[value];
                return true;
            }
            else
            {
                result = defaultValue;
                return false;
            }
        }
    }
}
