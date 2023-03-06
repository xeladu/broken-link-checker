using System.IO;
using System.Text;
using System.Threading.Tasks;

using BrokenLinkChecker.Utils.Models;

using Newtonsoft.Json;

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

    public async Task WriteJsonToFileAsync(string filePath, Result result)
    {
        await File.WriteAllTextAsync(Path.Combine(filePath, "blc.json"), JsonConvert.SerializeObject(result));
    }
}
