using System.Collections.Generic;

namespace BrokenLinkChecker.App.Models;

public class AppSettings
{
    public string BaseUrl { get; }
    public bool FollowInternalLinks { get; }
    public string OutputPath { get; }
    public bool DetailedLogMessages { get; }
    public bool Json { get; }
    public IEnumerable<int> ExcludeStatusCodes { get; }

    public AppSettings(string baseUrl, bool followInternalLinks, string outputPath, bool detailedLogMessages, bool json, IEnumerable<int> excludeStatusCodes)
    {
        BaseUrl = baseUrl;
        FollowInternalLinks = followInternalLinks;
        OutputPath = outputPath;
        DetailedLogMessages = detailedLogMessages;
        Json = json;
        ExcludeStatusCodes = excludeStatusCodes;
    }
}
