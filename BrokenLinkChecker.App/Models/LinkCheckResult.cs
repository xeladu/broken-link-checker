using System.Net;

namespace BrokenLinkChecker.App.Models;
internal class LinkCheckResult
{
    public required CheckResult Result { get; init; }

    public HttpStatusCode StatusCode { get; set; }

    public required string Error { get; init; }

    public override string ToString()
    {
        return $"{Result} ({(int)StatusCode}) {(string.IsNullOrEmpty(Error) ? "" : "(" + Error + ")")}";
    }
}
