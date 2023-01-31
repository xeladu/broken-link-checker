namespace BrokenLinkChecker.App.Models;

internal class Link
{
    public required string Target { get; init; }

    public required bool IsExternal { get; init; }

    public bool Processed { get; set; }

    public LinkCheckResult? Status { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Link l && l.Target.Equals(Target);
    }

    public override int GetHashCode()
    {
        return Target.GetHashCode();
    }

    public override string ToString()
    {
        return Target + (IsExternal ? " (ext)" : "");
    }
}
