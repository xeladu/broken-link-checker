﻿using System.Linq;

using BrokenLinkChecker.Utils.Models;

namespace BrokenLinkChecker.App.ArgumentParsing;
public static class ArgumentParser
{
    private const string ARG_FOLLOW_INTERNAL_LINKS = "--follow-internal-links";
    private const string ARG_OUTPUT = "--output";
    private const string ARG_VERBOSE = "--verbose";
    private const string ARG_VERBOSE_SHORT = "-v";
    private const string ARG_JSON = "--json";
    private const string ARG_EXCLUDE_STATUS_CODES = "--exclude-status-codes";

    public static AppSettings Parse(string[] args)
    {
        var baseUrl = "";
        var followInternalLinks = false;
        var outputPath = "";
        var detailedLogMessages = false;
        var json = false;
        var excludeStatusCodes = Enumerable.Empty<int>();

        if (args.Length == 0)
            throw new ArgumentParseException("Provide a website to check!");

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var val = args.Length > i + 1 ? args[i + 1] : null;

            // first argument is the website
            if (i == 0)
            {
                if (arg.StartsWith("-"))
                    throw new ArgumentParseException("The first argument must be the target website!");

                baseUrl = arg;
                continue;
            }

            if (arg.Equals(ARG_EXCLUDE_STATUS_CODES))
            {
                if (val != null)
                {
                    excludeStatusCodes = val.Split(",").Select(int.Parse);
                    i++;
                    continue;
                }
                else
                {
                    throw new ArgumentParseException("A list of HTTP status codes was expected but not found!");
                }
            }

            if (arg.Equals(ARG_FOLLOW_INTERNAL_LINKS))
            {
                followInternalLinks = true;
                continue;
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
                detailedLogMessages = true;
                continue;
            }

            if (arg.Equals(ARG_JSON))
            {
                json = true;
                continue;
            }

            throw new ArgumentParseException($"Invalid argument {arg} found!");
        }

        if (json && string.IsNullOrEmpty(outputPath))
            throw new ArgumentParseException($"Argument {ARG_JSON} requires {ARG_OUTPUT} to be set!");

        return new AppSettings(baseUrl, followInternalLinks, outputPath, detailedLogMessages, json, excludeStatusCodes);
    }
}
