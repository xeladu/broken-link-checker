using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BrokenLinkChecker.App;
internal class FileWriter
{
    private readonly StringBuilder _logs = new StringBuilder();

    public void Log(string message)
    {
        _logs.AppendLine(message);
    }

    public async Task WriteToFileAsync(string filePath)
    {
        await File.WriteAllTextAsync(Path.Combine(filePath, "blc.txt"), _logs.ToString());
    }
}
