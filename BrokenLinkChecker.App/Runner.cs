using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace BrokenLinkChecker.App;
internal class Runner
{
    private readonly LinkCollector _linkCollector;
    private readonly LinkChecker _linkChecker;

    public Runner(IServiceProvider services)
    {
        _linkCollector = services.GetService<LinkCollector>() ?? throw new ArgumentNullException(nameof(_linkCollector));
        _linkChecker = services.GetService<LinkChecker>() ?? throw new ArgumentNullException(nameof(_linkChecker));
    }

    public async Task RunAsync(string baseUrl)
    {
        var sw = new Stopwatch();
        sw.Start();
        
        await _linkCollector.CollectAsync(baseUrl);

        await Parallel.ForEachAsync(_linkCollector.Links, async (link, _) =>
        {
            await _linkChecker.CheckAsync(link);
            Console.WriteLine($"Checking {link} ... {link.Status}");
        });
        sw.Stop();

        Console.WriteLine(sw.ElapsedMilliseconds / 1000);
    }
}
