using System.Net;
using System.Threading.Tasks;

using BrokenLinkChecker.App.Models;

namespace BrokenLinkChecker.App;
internal class LinkChecker
{
    public async Task CheckAsync(Link link)
    {
        var request = (HttpWebRequest)WebRequest.Create(link.Target);
        request.Timeout = 15000;
        request.Method = "HEAD";
        try
        {
            using (var response = (HttpWebResponse) await request.GetResponseAsync())
            {
                link.Status = new LinkCheckResult
                {
                    Result = response.StatusCode == HttpStatusCode.OK ? CheckResult.Online : CheckResult.Broken,
                    Error = response.StatusCode == HttpStatusCode.OK ? string.Empty : response.StatusDescription
                };
            }
        }
        catch (WebException ex)
        {
            link.Status = new LinkCheckResult
            {
                Result = CheckResult.Broken,
                Error = ex.Message
            };
        }
    }
}
