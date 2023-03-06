using BrokenLinkChecker.Utils.Models;
using BrokenLinkChecker.Utils.ProgressReporting;

using HtmlAgilityPack;

namespace BrokenLinkChecker.Utils;

public class LinkCollector : ProgressReporter
{
    private string _host = "";

    public List<Link> Links { get; } = new List<Link>();

    public async Task CollectAsync(AppSettings settings)
    {
        _host = new Uri(settings.BaseUrl).Host;

        var initialUri = settings.BaseUrl.EndsWith("/") ? settings.BaseUrl.Substring(0, settings.BaseUrl.Length - 1) : settings.BaseUrl;
        Links.Add(new Link { IsExternal = false, Target = initialUri, Sources = new List<string> { initialUri } });

        var linkCount = Links.Count;
        for (var i = 0; i < linkCount; i++)
        {
            var currentLink = Links[i];

            // only index internal pages
            if (currentLink.IsExternal)
                continue;

            ReportProgressVerbose($"Processing {currentLink.Target}...");

            var newLinksFound = 0;
            foreach (var newLink in await GrabLinksFromUrl(currentLink.Target))
            {
                if (!Links.Contains(newLink))
                {
                    Links.Add(newLink);
                    newLinksFound++;

                    // By not increasing the counter, we can prevent additional links from being followed.
                    // Only the link of the base url are used.
                    if (settings.FollowInternalLinks)
                    {
                        linkCount++;
                    }
                }
                else
                {
                    var existingLink = Links.SingleOrDefault(x => x.Equals(newLink));
                    if (existingLink != null && !existingLink.Sources.Contains(currentLink.Target))
                        existingLink.Sources.Add(currentLink.Target);
                }
            }

            ReportProgressVerbose($"{newLinksFound} new links found");
        }

        ReportProgress($"{Links.Count} unique links found");
    }

    private async Task<IEnumerable<Link>> GrabLinksFromUrl(string url)
    {
        var web = new HtmlWeb();
        try
        {
            var doc = await web.LoadFromWebAsync(url);
            return doc.DocumentNode.SelectNodes("//a[@href]")
                // only find http links
                .Where(link => link.GetAttributeValue("href", "").StartsWith("http"))
                .Select(link => link.GetAttributeValue("href", ""))
                .Select(link => new Link
                {
                    // remove trailing slashes
                    Target = link.EndsWith("/") ? link.Substring(0, link.Length - 1) : link,
                    Sources = new List<string> { url },
                    IsExternal = !new Uri(link).Host.Equals(_host),
                    Status = null
                });
        }
        catch
        {
            return Enumerable.Empty<Link>();
        }
    }
}
