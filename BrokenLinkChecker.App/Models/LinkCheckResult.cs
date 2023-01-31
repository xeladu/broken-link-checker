namespace BrokenLinkChecker.App.Models;
internal class LinkCheckResult
{
    public required CheckResult Result { get; init; }

    public required string Error { get; init; }

    public override string ToString()
    {
        return $"{Result}{(string.IsNullOrEmpty(Error) ? "" : " (" + Error + ")")}";
    }
}
