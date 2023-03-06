namespace BrokenLinkChecker.Utils.Models;
public class Result
{
    public int BrokenLinkCount { get; set; }
    public int TotalLinkCount { get; set; }
    public IEnumerable<Link> BrokenLinks { get; set; } = Enumerable.Empty<Link>();
}
