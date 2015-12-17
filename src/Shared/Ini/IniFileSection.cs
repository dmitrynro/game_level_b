using System.Collections.Generic;

namespace Shared.Ini
{
    public class IniFileSection
    {
        public Dictionary<string, string> Values = new Dictionary<string, string>();

        public bool TryGetValue(string value, out string result)
        {
            if (Values.ContainsKey(value))
            {
                result = Values[value];
                return true;
            }
            else
            {
                result = "";
                return false;
            }
        }
    }
}
