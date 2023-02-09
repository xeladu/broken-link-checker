using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BrokenLinkChecker.App.Models;
using BrokenLinkChecker.App.ProgressReporting;

using HtmlAgilityPack;

using Microsoft.Extensions.DependencyInjection;

namespace BrokenLinkChecker.App;

internal class LinkCollector : ProgressReporter
{
    private readonly AppSettings _appSettings;
    private readonly string _host;

    public List<Link> Links { get; } = new List<Link>();

    public LinkCollector(IServiceProvider services)
    {
        _appSettings = services.GetService<AppSettings>() ?? throw new ArgumentNullException(nameof(_appSettings));
        _host = new Uri(_appSettings.BaseUrl).Host;
    }

    public async Task CollectAsync()
    {
        var initialUri = _appSettings.BaseUrl.EndsWith("/") ? _appSettings.BaseUrl.Substring(0, _appSettings.BaseUrl.Length - 1) : _appSettings.BaseUrl;
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
                    if (_appSettings.FollowInternalLinks)
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
