using System.Net;

using BrokenLinkChecker.Utils.Models;
using BrokenLinkChecker.Utils.ProgressReporting;

namespace BrokenLinkChecker.Utils;

public class LinkChecker : ProgressReporter
{
    public async Task CheckAsync(Link link, IEnumerable<int> excludedStatusCodes)
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.41");

        var message = new HttpRequestMessage
        {
            Method = HttpMethod.Head,
            RequestUri = new Uri(link.Target)
        };

        try
        {
            using (var response = await client.SendAsync(message))
            {
                link.Status = new LinkCheckResult
                {
                    Result = IsLinkOnline(response.StatusCode, excludedStatusCodes) ? CheckResult.Online : CheckResult.Broken,
                    StatusCode = response.StatusCode,
                    Error = response.ReasonPhrase ?? ""
                };
            }
        }
        catch (HttpRequestException ex)
        {
            link.Status = new LinkCheckResult
            {
                Result = CheckResult.Broken,
                StatusCode = ex.StatusCode ?? 0,
                Error = ex.Message
            };
        }

        if (link.Status?.Result == CheckResult.Broken)
        {
            ReportProgress($"{link}");
        }
        else
        {
            ReportProgressVerbose($"{link}");
        }
    }

    private static bool IsLinkOnline(HttpStatusCode code, IEnumerable<int> excludeStatusCodes)
    {
        return code == HttpStatusCode.OK || excludeStatusCodes.ToList().Contains((int)code);
    }
}
