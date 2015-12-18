
namespace Shared.Ini
{
    public static class IniParser
    {
        public static IniFile Parse(string[] source)
        {
            IniFile file = new IniFile();
            IniFileSection currentSection = file;

            foreach (string str in source)
            {
                int index;
                string s = str.Trim();

                if (s.Length == 0 || s.StartsWith(";") || s.StartsWith("#"))
                    continue;
                else if (s.StartsWith("[") && s.EndsWith("]"))
                {
                    string sectionName = s.Substring(1, s.Length - 2);

                    if (!file.Sections.ContainsKey(sectionName))
                    {
                        currentSection = new IniFileSection();
                        file.Sections.Add(sectionName, currentSection);
                    }
                    else
                        currentSection = file.Sections[sectionName];
                }
                else if ((index = s.IndexOf('=')) > 0)
                {
                    string key = s.Substring(0, index).Trim();
                    string value = s.Substring(index + 1).Trim();

                    if ((index = value.IndexOfAny(new[] { ';', '#' })) > 0)
                        value = value.Substring(0, index).Trim();

                    if (!currentSection.Values.ContainsKey(key))
                        currentSection.Values.Add(key, value);
                    else
                        currentSection.Values[key] = value;
                }
            }

            return file;
        }
    }
}
