using System;
using System.Threading.Tasks;

using BrokenLinkChecker.App.ArgumentParsing;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BrokenLinkChecker.App;

internal static class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine(
                """
                ---------------------------
                --- Broken Link Checker ---
                ---------------------------

                """);
        try
        {
            var settings = ArgumentParser.Parse(args);

            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((services) =>
                {
                    services.AddSingleton<Runner>();
                    services.AddSingleton<FileWriter>();
                    services.AddTransient<LinkCollector>();
                    services.AddTransient<LinkChecker>();
                    services.AddSingleton(settings);
                })
                .Build();

            var runner = host.Services.GetService<Runner>();
            if (runner != null)
                await runner.RunAsync();

            if (!string.IsNullOrEmpty(settings.OutputPath))
            {
                var fileWriter = host.Services.GetService<FileWriter>();
                if (fileWriter != null)
                    await fileWriter.WriteToFileAsync(settings.OutputPath);
            }
        }
        catch (ArgumentParseException e)
        {
            Console.WriteLine(
                """
                usage: blc <url> [--verbose | -v] [--follow-internal-links <true|false>] [--output <path>]

                    url        
                    A valid http(s) web url to check

                    --verbose | -v
                    Outputs detailed program information

                    --follow-internal-links
                    Indicates if sublinks of the url with the same host should be checked, too (default: true)

                    --output
                    Specify a folder that will contain the program's output in a text file blc.txt

                """);

            Console.WriteLine($"ERROR: {e.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            Environment.Exit(1);
        }
    }
}