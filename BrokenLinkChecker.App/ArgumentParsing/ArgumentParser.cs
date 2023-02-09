using BrokenLinkChecker.App.Models;

namespace BrokenLinkChecker.App.ArgumentParsing;
public static class ArgumentParser
{
    private const string ARG_FOLLOW_INTERNAL_LINKS = "--follow-internal-links";
    private const string ARG_OUTPUT = "--output";
    private const string ARG_VERBOSE = "--verbose";
    private const string ARG_VERBOSE_SHORT = "-v";

    public static AppSettings Parse(string[] args)
    {
        var baseUrl = "";
        var followInternalLinks = true;
        var outputPath = "";
        var detailledLogMessages = false;

        if (args.Length == 0)
            throw new ArgumentParseException("Provide a website to check!");

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var val = args.Length > i + 1 ? args[i + 1] : null;

            // first argument is the website
            if (i == 0)
            {
                baseUrl = arg;
                continue;
            }

            if (arg.Equals(ARG_FOLLOW_INTERNAL_LINKS))
            {
                if (bool.TryParse(val, out var res))
                {
                    followInternalLinks = res;
                    i++;
                    continue;
                }
                else
                {
                    throw new ArgumentParseException($"{val} is not a valid boolean property!");
                }
            }

            if (arg.Equals(ARG_OUTPUT))
            {
                if (val != null)
                {
                    outputPath = val;
                    i++;
                    continue;
                }
                else
                {
                    throw new ArgumentParseException("The output argument must not be empty!");
                }
            }

            if (arg.Equals(ARG_VERBOSE) || arg.Equals(ARG_VERBOSE_SHORT))
            {
                detailledLogMessages = true;
                continue;
            }

            throw new ArgumentParseException($"Invalid argument {arg} found!");
        }

        return new AppSettings(baseUrl, followInternalLinks, outputPath, detailledLogMessages);
    }
}
