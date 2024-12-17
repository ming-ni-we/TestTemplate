using System.IO;

namespace TestItemTemplate.Helpers
{
    internal class IniFile
    {
        private Dictionary<string, Dictionary<string, string>> sections;
        /// <summary>
        /// 初始化
        /// </summary>
        public IniFile()
        {
            sections = new Dictionary<string, Dictionary<string, string>>();
        }
        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <exception cref="FileNotFoundException">文件未找到</exception>
        public void Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            string currentSection = null;
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            foreach (string line in File.ReadLines(filePath))
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith(";"))
                {
                    // Ignore empty lines and comments.
                    continue;
                }
                else if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    // New section
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    sections[currentSection] = new Dictionary<string, string>();
                }
                else if (currentSection != null && trimmedLine.Contains("="))
                {
                    // Key-value pair
                    string[] parts = trimmedLine.Split(new char[] { '=' }, 2);
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    sections[currentSection][key] = value;
                }
            }
        }

        /// <summary>
        /// 写入string数组
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="arrayName"></param>
        /// <param name="values"></param>
        public void WriteIniArray(string section, string arrayName, List<string> values)
        {
            if (!sections.ContainsKey(section))
            {
                sections[section] = new Dictionary<string, string>();
            }

            sections[section][arrayName + "Number"] = values.Count.ToString();
            for (int i = 0; i < values.Count; i++)
            {
                string keyName = $"{arrayName}[{i}]";
                sections[section][keyName] = values[i];
            }
        }
        /// <summary>
        /// 写入string数组
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="arrayName"></param>
        /// <param name="values"></param>
        public List<string>? ReadIniArray(string section, string arrayName)
        {
            if (sections.ContainsKey(section))
            {

                List<string> resArry = new List<string>();
                int lenth = int.Parse(sections[section][arrayName + "Number"]);

                for (int i = 0; i < lenth; i++)
                {
                    string keyName = $"{arrayName}[{i}]";
                    if (sections[section].ContainsKey(keyName))
                    {
                        resArry.Add(sections[section][keyName]);
                    }
                }
                return resArry;
            }
            return null;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="section">文件中的节</param>
        /// <param name="key">对应的key</param>
        /// <returns>返回key对应的值</returns>
        public string? GetValue(string section, string key)
        {
            if (sections.ContainsKey(section) && sections[section].ContainsKey(key))
            {
                return sections[section][key];
            }
            return null;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="section">文件中的节</param>
        /// <param name="key">对应的key</param>
        /// <param name="value">对应的value</param>
        public void SetValue(string section, string key, string value)
        {
            if (!sections.ContainsKey(section))
            {
                sections[section] = new Dictionary<string, string>();
            }
            sections[section][key] = value;
        }
        /// <summary>
        /// 文件保存
        /// </summary>
        /// <param name="filePath">保存的位置</param>
        public void Save(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var section in sections)
                {
                    writer.WriteLine($"[{section.Key}]");
                    foreach (var keyValue in section.Value)
                    {
                        writer.WriteLine($"{keyValue.Key} = {keyValue.Value}");
                    }
                }
            }
        }
    }
}
