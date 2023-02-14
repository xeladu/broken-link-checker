using System;
using System.Threading.Tasks;

using BrokenLinkChecker.App.ArgumentParsing;
using BrokenLinkChecker.App.Models;

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
                    services.AddSingleton<Result>();
                    services.AddSingleton(settings);
                })
                .Build();

            var runner = host.Services.GetService<Runner>();
            if (runner != null)
                await runner.RunAsync();

            if (!string.IsNullOrEmpty(settings.OutputPath))
            {
                if (settings.Json)
                {

                }

                var fileWriter = host.Services.GetService<FileWriter>();
                if (fileWriter != null)
                {
                    if (settings.Json)
                    {
                        var result = host.Services.GetService<Result>();
                        if (result != null)
                            await fileWriter.WriteJsonToFileAsync(settings.OutputPath, result);
                    }
                    else
                    {
                        await fileWriter.WriteToFileAsync(settings.OutputPath);
                    }
                }
            }
        }
        catch (ArgumentParseException e)
        {
            Console.WriteLine(
                """
                usage: blc <url> [--verbose | -v] [--no-follow-internal-links] [--output <path>] [--json] [--exclude-status-codes 403,503]

                    url        
                    A valid http(s) web url to check

                    --verbose | -v
                    Outputs detailed program information

                    --no-follow-internal-links
                    Indicates that sublinks of the url with the same host should not be checked

                    --output
                    Specify a folder that will contain the program's output in a text file blc.txt

                    --json
                    Program output will be stored in JSON format (requires --output)

                    --exclude-status-code
                    Expects a list of comma-separated HTTP status codes that will be treated as online

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