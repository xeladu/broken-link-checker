using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BrokenLinkChecker.App;

internal static class Program
{
    static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((services) =>
            {
                services.AddSingleton<Runner>();
                services.AddTransient<LinkCollector>();
                services.AddTransient<LinkChecker>();
            })
            .Build();

        await host.Services.GetService<Runner>().RunAsync("https://quickcoder.org");
    }
}