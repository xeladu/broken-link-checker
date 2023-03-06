using System.Net;

namespace BrokenLinkChecker.Utils.Models;
public class LinkCheckResult
{
    public required CheckResult Result { get; init; }

    public HttpStatusCode StatusCode { get; set; }

    public required string Error { get; init; }

    public override string ToString()
    {
        return $"{Result} ({(int)StatusCode}) {(string.IsNullOrEmpty(Error) ? "" : "(" + Error + ")")}";
    }
}
