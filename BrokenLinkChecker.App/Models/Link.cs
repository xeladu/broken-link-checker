using System.Collections.Generic;

namespace BrokenLinkChecker.App.Models;

internal class Link
{
    public List<string> Sources { get; set; } = new List<string>();

    public required string Target { get; init; }

    public required bool IsExternal { get; init; }

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
        return Target + (IsExternal ? " (ext)" : "") + (Status != null ? "\r\n" + Status : "");
    }
}
