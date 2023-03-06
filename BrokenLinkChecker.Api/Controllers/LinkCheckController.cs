using BrokenLinkChecker.Utils;
using BrokenLinkChecker.Utils.Models;

using Microsoft.AspNetCore.Mvc;

namespace BrokenLinkChecker.Api.Controllers;

/// <summary>
/// Checks for broken links
/// </summary>
[ApiController]
[Route("CheckWebsite", Name = "CheckWebsite")]
public class LinkCheckController : ControllerBase
{
    private readonly ILogger<LinkCheckController> _logger;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">ILogger instance for logging</param>
    /// <param name="provider">IServiceProvider instance for dependency injection</param>
    public LinkCheckController(ILogger<LinkCheckController> logger, IServiceProvider provider)
    {
        _logger = logger;
        _serviceProvider = provider;
    }

    /// <summary>
    ///     Collects and checks all links on a given website if they are online or not. Online means that a checked link returns a HTTP status code of 200. To configure these codes, use the excludeStatusCodes argument.
    /// </summary>
    /// <param name="url">The website to check.</param>
    /// <param name="followInternalLinks">Indicates if internal links should be followed or not. When set to true, the app will follow internal links with the same host and collect additional links. This will lead to a longer runtime. No link is checked twice!</param>
    /// <param name="excludeStatusCodes">A comma-separated list of HTTP status codes that will be treated as online. To mark the status codes 403 and 401 as online, pass them like this: 403,401.</param>
    /// <returns>A list of Link objects containing the detailed information about all found links on the given website.</returns>
    [HttpGet]
    public async Task<IEnumerable<Link>> CheckWebsiteAsync(
        [FromQuery] string url,
        [FromQuery] string excludeStatusCodes = "",
        [FromQuery] bool followInternalLinks = false)
    {
        var settings = new AppSettings(url, followInternalLinks, ConvertCommaListToStatusCodes(excludeStatusCodes));

        _logger.LogInformation(url);
        _logger.LogInformation(followInternalLinks.ToString());
        _logger.LogInformation(excludeStatusCodes);

        var linkCollector = _serviceProvider.GetService<LinkCollector>() ?? throw new ArgumentNullException(nameof(LinkCollector));
        var linkChecker = _serviceProvider.GetService<LinkChecker>() ?? throw new ArgumentNullException(nameof(LinkChecker));

        await linkCollector.CollectAsync(settings);
        await Parallel.ForEachAsync(linkCollector.Links, async (link, _) => await linkChecker.CheckAsync(link, settings.ExcludeStatusCodes));

        return linkCollector.Links;
    }

    private List<int> ConvertCommaListToStatusCodes(string excludeStatusCodes)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(excludeStatusCodes))
                return Enumerable.Empty<int>().ToList();

            return excludeStatusCodes.Split(',').Select(x => int.Parse(x.Trim())).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Cannot convert status codes '{excludeStatusCodes}'. Please provide the codes as comma-separated numbers only!", ex);
        }
    }
}
