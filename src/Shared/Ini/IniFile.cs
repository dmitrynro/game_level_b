using System.Collections.Generic;

namespace Shared.Ini
{
    public class IniFile : IniFileSection
    {
        public Dictionary<string, IniFileSection> Sections = new Dictionary<string, IniFileSection>();

        public IniFile()
        {
            Sections["global"] = this;
        }

        public bool TryGetValue(string section, string value, out string result, string defaultValue = "")
        {
            if (Sections.ContainsKey(section))
                return Sections[section].TryGetValue(value, out result, defaultValue);
            else
            {
                result = defaultValue;
                return false;
            }
        }
    }
}
