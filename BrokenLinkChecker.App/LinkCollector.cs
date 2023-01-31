using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BrokenLinkChecker.App.Models;

using HtmlAgilityPack;

namespace BrokenLinkChecker.App;
internal class LinkCollector
{
    private List<string> _processedTargets = new List<string>();
    private string? _baseUrl;

    public List<Link> Links { get; } = new List<Link>();

    public async Task CollectAsync(string baseUrl)
    {
        _baseUrl = baseUrl;

        var initialLinks = (await GrabLinksFromUrl(_baseUrl)).Distinct();
        Links.AddRange(initialLinks.Where(link => !_processedTargets.Contains(link.Target)));

        var linkCount = Links.Count;
        for (var i = 0; i < linkCount; i++)
        {
            Console.WriteLine($"{i}/{linkCount - 1}");

            var link = Links[i];

            // don't search twice
            if (link.Processed)
                continue;

            // don't search duplicate targets
            if (_processedTargets.Contains(link.Target))
                continue;

            Console.WriteLine($"Processing {link.Target}...");
            _processedTargets.Add(link.Target);

            foreach (var newLink in await GrabLinksFromUrl(_baseUrl))
            {
                if (!Links.Contains(newLink) && !_processedTargets.Contains(newLink.Target))
                {
                    Links.Add(newLink);
                    linkCount++;
                }
            }
        }
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
                    IsExternal = !link.StartsWith(_baseUrl),
                    Processed = false,
                    Status = null
                });
        }
        catch
        {
            return Enumerable.Empty<Link>();
        }
    }
}
