using System.Collections.Generic;

namespace Shared.Ini
{
    public class IniFile
    {
        public Dictionary<string, IniFileSection> Sections = new Dictionary<string, IniFileSection>();

        public IniFileSection Global { get { return Sections["global"]; } }

        public IniFile()
        {
            Sections.Add("global", new IniFileSection());
        }

        public bool TryGetValue(string section, string value, out string result)
        {
            if (Sections.ContainsKey(section))
                return Sections[section].TryGetValue(value, out result);
            else
            {
                result = "";
                return false;
            }
        }

        public bool TryGetValue(string value, out string result)
        {
            return TryGetValue("global", value, out result);
        }
    }
}
