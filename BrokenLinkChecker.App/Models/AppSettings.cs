namespace BrokenLinkChecker.App.Models;

public class AppSettings
{
    public string BaseUrl { get; }
    public bool FollowInternalLinks { get; }
    public string OutputPath { get; }
    public bool DetailedLogMessages { get; }

    public AppSettings(string baseUrl, bool followInternalLinks, string outputPath, bool detailedLogMessages)
    {
        BaseUrl = baseUrl;
        FollowInternalLinks = followInternalLinks;
        OutputPath = outputPath;
        DetailedLogMessages = detailedLogMessages;
    }
}
