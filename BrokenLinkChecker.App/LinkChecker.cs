using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using BrokenLinkChecker.App.Models;
using BrokenLinkChecker.App.ProgressReporting;

namespace BrokenLinkChecker.App;

internal class LinkChecker : ProgressReporter
{
    public async Task CheckAsync(Link link)
    {
        ReportProgressVerbose($"Checking link {link}...");
        
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        var message = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(link.Target)
        };

        try
        {
            using (var response = await client.SendAsync(message))
            {
                link.Status = new LinkCheckResult
                {
                    Result = response.StatusCode == HttpStatusCode.OK ? CheckResult.Online : CheckResult.Broken,
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
}
