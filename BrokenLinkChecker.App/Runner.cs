using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using BrokenLinkChecker.App.Models;
using BrokenLinkChecker.App.ProgressReporting;

using Microsoft.Extensions.DependencyInjection;

namespace BrokenLinkChecker.App;
internal class Runner : ProgressReporter
{
    private readonly LinkCollector _linkCollector;
    private readonly LinkChecker _linkChecker;
    private readonly AppSettings _appSettings;

    public Runner(IServiceProvider services)
    {
        _linkCollector = services.GetService<LinkCollector>() ?? throw new ArgumentNullException(nameof(_linkCollector));
        _linkChecker = services.GetService<LinkChecker>() ?? throw new ArgumentNullException(nameof(_linkChecker));
        _appSettings = services.GetService<AppSettings>() ?? throw new ArgumentNullException(nameof(_appSettings));
    }

    public async Task RunAsync()
    {
        SetupProgressReporting();

        var sw = new Stopwatch();
        sw.Start();

        ReportProgress($"Collecting links of {_appSettings.BaseUrl} ...");

        await _linkCollector.CollectAsync();

        ReportProgress($"Checking availability of {_linkCollector.Links.Count} links ...");

        await Parallel.ForEachAsync(_linkCollector.Links, async (link, _) =>
        {
            await _linkChecker.CheckAsync(link);
        });

        ReportProgressVerbose($"Availability check completed");

        ShowSummary();

        sw.Stop();
        ReportProgressVerbose($"App runtime was {sw.ElapsedMilliseconds / 1000} seconds");
    }

    private void ShowSummary()
    {
        ReportProgress(
            """

            ---------------
            --- Summary ---
            ---------------

            """);

        var linkCount = _linkCollector.Links.Count;
        var brokenLinkCount = _linkCollector.Links.Count(x => x.Status?.Result == CheckResult.Broken);
        var onlineLinkCount = linkCount - brokenLinkCount;

        ReportProgress($"Links checked: {linkCount,4}");
        ReportProgress($"Links online:  {onlineLinkCount,4}");
        ReportProgress($"Links broken:  {brokenLinkCount,4}");

        ReportProgress(
            """

            ----------------------------
            --- List of broken links ---
            ----------------------------

            """);

        var brokenLinks = _linkCollector.Links.Where(x => x.Status?.Result == CheckResult.Broken).OrderBy(x => x.Target).ToList();
        for (var i = 1; i <= brokenLinks.Count; i++)
        {
            var link = brokenLinks[i - 1];
            ReportProgress($"{i - 1}: {link}");
            ReportProgress($"  Sources: {link.Sources.Count}");
            ReportProgress($"  {link.Sources.Aggregate((x, y) => x + "\r\n  " + y)}\r\n");
        }

        ReportProgressVerbose(
        """

            -------------------------
            --- List of all links ---
            -------------------------

            """);

        var allLinks = _linkCollector.Links.OrderBy(x => x.Target).ToList();
        for (var i = 1; i <= allLinks.Count; i++)
        {
            var link = allLinks[i - 1];
            ReportProgressVerbose($"{i - 1}: {link}");
            ReportProgressVerbose($"  Sources: {link.Sources.Count}");
            ReportProgressVerbose($"  {link.Sources.Aggregate((x, y) => x + "\r\n  " + y)}\r\n");
        }
    }

    private void SetupProgressReporting()
    {
        if (_appSettings.DetailedLogMessages)
        {
            _linkCollector.OnReportProgressVerbose += (s, e) => Console.WriteLine(e.Message);
            _linkChecker.OnReportProgressVerbose += (s, e) => Console.WriteLine(e.Message);
            OnReportProgressVerbose += (s, e) => Console.WriteLine(e.Message);
        }
        else
        {
            _linkCollector.OnReportProgress += (s, e) => Console.WriteLine(e.Message);
            _linkChecker.OnReportProgress += (s, e) => Console.WriteLine(e.Message);
            OnReportProgress += (s, e) => Console.WriteLine(e.Message);
        }
    }
}