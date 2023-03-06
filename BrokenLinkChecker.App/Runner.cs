using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using BrokenLinkChecker.Utils;
using BrokenLinkChecker.Utils.Models;
using BrokenLinkChecker.Utils.ProgressReporting;

using Microsoft.Extensions.DependencyInjection;

namespace BrokenLinkChecker.App;
internal class Runner : ProgressReporter
{
    private readonly LinkCollector _linkCollector;
    private readonly LinkChecker _linkChecker;
    private readonly AppSettings _appSettings;
    private readonly FileWriter _fileWriter;
    private readonly Result _result;

    public Runner(IServiceProvider services)
    {
        _linkCollector = services.GetService<LinkCollector>() ?? throw new ArgumentNullException(nameof(_linkCollector));
        _linkChecker = services.GetService<LinkChecker>() ?? throw new ArgumentNullException(nameof(_linkChecker));
        _appSettings = services.GetService<AppSettings>() ?? throw new ArgumentNullException(nameof(_appSettings));
        _fileWriter = services.GetService<FileWriter>() ?? throw new ArgumentNullException(nameof(_fileWriter));
        _result = services.GetService<Result>() ?? throw new ArgumentNullException(nameof(_result));
    }

    public async Task RunAsync()
    {
        SetupProgressReporting();

        var sw = new Stopwatch();
        sw.Start();

        ReportProgress($"Collecting links of {_appSettings.BaseUrl} ...");

        await _linkCollector.CollectAsync(_appSettings);

        ReportProgress($"Checking availability of {_linkCollector.Links.Count} links ...");

        await Parallel.ForEachAsync(_linkCollector.Links, async (link, _) => await _linkChecker.CheckAsync(link, _appSettings.ExcludeStatusCodes));

        _result.TotalLinkCount = _linkCollector.Links.Count();
        _result.BrokenLinkCount = _linkCollector.Links.Count(x => x.Status?.Result == CheckResult.Broken);
        _result.BrokenLinks = _linkCollector.Links.Where(x => x.Status?.Result == CheckResult.Broken);

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

        ReportBrokenLinks();
        ReportAllLinks();

        if (brokenLinkCount == 0)
        {
            ReportProgress(
            """

            ✅ No issues found!

            """);
        }
    }

    private void ReportBrokenLinks()
    {
        var brokenLinks = _linkCollector.Links.Where(x => x.Status?.Result == CheckResult.Broken).OrderBy(x => x.Target).ToList();
        if (brokenLinks.Count == 0)
            return;

        ReportProgress(
            """

            ----------------------------
            --- List of broken links ---
            ----------------------------

            """);

        for (var i = 1; i <= brokenLinks.Count; i++)
        {
            var link = brokenLinks[i - 1];
            ReportProgress($"{i}: {link}");
            ReportProgress($"  Sources: {link.Sources.Count}");
            ReportProgress($"  {link.Sources.Aggregate((x, y) => x + "\r\n  " + y)}\r\n");
        }
    }

    private void ReportAllLinks()
    {
        var allLinks = _linkCollector.Links.OrderBy(x => x.Target).ToList();
        if (allLinks.Count == 0)
            return;

        ReportProgressVerbose(
            """

            -------------------------
            --- List of all links ---
            -------------------------

            """);

        for (var i = 1; i <= allLinks.Count; i++)
        {
            var link = allLinks[i - 1];
            ReportProgressVerbose($"{i}: {link}");
            ReportProgressVerbose($"  Sources: {link.Sources.Count}");
            ReportProgressVerbose($"  {link.Sources.Aggregate((x, y) => x + "\r\n  " + y)}\r\n");
        }
    }

    private void SetupProgressReporting()
    {
        if (_appSettings.DetailedLogMessages && !string.IsNullOrEmpty(_appSettings.OutputPath))
        {
            _linkCollector.OnReportProgressVerbose += WriteToConsoleAndOutput;
            _linkChecker.OnReportProgressVerbose += WriteToConsoleAndOutput;
            OnReportProgressVerbose += WriteToConsoleAndOutput;
        }
        else if (_appSettings.DetailedLogMessages)
        {
            _linkCollector.OnReportProgressVerbose += WriteToConsole;
            _linkChecker.OnReportProgressVerbose += WriteToConsole;
            OnReportProgressVerbose += WriteToConsole;
        }
        else if (!string.IsNullOrEmpty(_appSettings.OutputPath))
        {
            _linkCollector.OnReportProgress += WriteToConsoleAndOutput;
            _linkChecker.OnReportProgress += WriteToConsoleAndOutput;
            OnReportProgress += WriteToConsoleAndOutput;
        }
        else
        {
            _linkCollector.OnReportProgress += WriteToConsole;
            _linkChecker.OnReportProgress += WriteToConsole;
            OnReportProgress += WriteToConsole;
        }
    }

    private void WriteToConsole(object? sender, ProgressEventArgs? e)
    {
        if (e == null)
            return;

        Console.WriteLine(e.Message);
    }

    private void WriteToConsoleAndOutput(object? sender, ProgressEventArgs? e)
    {
        if (e == null)
            return;

        Console.WriteLine(e.Message);
        _fileWriter.Log(e.Message);
    }
}