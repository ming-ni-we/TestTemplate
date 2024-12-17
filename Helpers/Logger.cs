using System.IO;
using System.Reflection;

namespace TestItemTemplate.Helpers
{
    public class Logger
    {
        private string _logPath;
        private string _fileName;

        public Logger(string sn)
        {
#pragma warning disable CS8604 // 引用类型参数可能为 null。
            _logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "log");
#pragma warning restore CS8604 // 引用类型参数可能为 null。
            _logPath = Path.Combine(_logPath, DateTime.Now.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
            _fileName = sn + ".log";
        }

        public void Log(string message)
        {
            string filePath = Path.Combine(_logPath, _fileName);
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + message);
            }
        }
    }
}
